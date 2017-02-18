using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;
using System.Text.RegularExpressions;
using Dependencies;

namespace SS
{
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string s is a valid cell name if and only if it consists of one or more letters, 
    /// followed by a non-zero digit, followed by zero or more digits.
    /// 
    /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names.  On the other hand, 
    /// "Z", "X07", and "hello" are not valid cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important, and it is important that you understand the distinction and use
    /// the right term when writing code, writing comments, and asking questions.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In an empty spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
    /// The value of a Formula, of course, can depend on the values of variables.  The value 
    /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
    /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
    /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
    /// is a double, as specified in Formula.Evaluate.
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private DependencyGraph dependencyGraph;

        private Dictionary<String, Cell> Cells;

        /// <summary>
        /// Zero argument constructor that creates
        /// a new empty spreadsheet
        /// </summary>
        public Spreadsheet()
        {
            this.Cells = new Dictionary<string, Cell>();
            this.dependencyGraph = new DependencyGraph();
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            //Regex patter for our name
            String varPattern = @"^[a-zA-Z][0-9a-zA-Z]*";

            //empty cell
            Cell cell = new Cell();

            //if name is null or regex is not a match
            //throw exception
            if(name == null || !Regex.IsMatch(name, varPattern))
            {
                throw new InvalidNameException();
            }

            //get the contents of the cell
            Cells.TryGetValue(name, out cell);

            //return the cells
            return cell.getContents;
        }


        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //create set for our cells
            HashSet<string> set =  new HashSet<string>();
            //new empty cell
            Cell cell = new Cell();

            //for each key in cells...
            foreach(String name in Cells.Keys)
            {
                //try to get the value
                Cells.TryGetValue(name, out cell);
                //if the value is not null add it to the set
                if(cell.getContents != null)
                {
                    set.Add(name);
                }
            }
            //return the set
            return set;
        }


        /// <summary>
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            //regex pattern
            String varPattern = @"^[a-zA-Z][0-9a-zA-Z]*";

            // new cell that contains formula as contents
            Cell cell = new Cell(formula);

            //non-intialized hashset
            HashSet<String> cellsRecalculated;

            //if name is null or if the reges is not a match then throw
            //exception
            if (name == null || !Regex.IsMatch(name, varPattern))
            {
                throw new InvalidNameException();
            }

            //store the old dependencies in the enum
            IEnumerable<String> oldDependencies = dependencyGraph.GetDependees(name);

            try
            {
                //replace dependencies with the variables of the formula
                dependencyGraph.ReplaceDependees(name, formula.GetVariables());
              

                //if the Cells contains the name replace the
                //contents
                if (Cells.ContainsKey(name))
                {
                    Cells.Remove(name);
                    Cells.Add(name, cell);
                }
                //else add the name
                else
                {
                    Cells.Add(name, cell);
                }

                //ALMOST BIG MISTAKE. RECALCULATE AT THE END
                cellsRecalculated = new HashSet<String>(GetCellsToRecalculate(name));

                return cellsRecalculated;
            }
            catch (CircularException)
            {
                //revert the dependencies back to old ones
                dependencyGraph.ReplaceDependees(name, oldDependencies);
                throw new CircularException();
            }
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, string text)
        {
            //Var pattern for for our name check
            String varPattern = @"^[a-zA-Z][0-9a-zA-Z]*";

            // empty set for our new cell
            HashSet<String> emptySet = new HashSet<string>();

            //if either name is null or text is null or regex is not a match
            //throw an exception
            if (text == null)
            {
                throw new ArgumentNullException();
            }
            else if(name == null || !Regex.IsMatch(name, varPattern))
            {
                throw new InvalidNameException();
            }

            //create a new cell with the text as contents
            Cell cell = new Cell(text);

            //if the cell contains name remove it then add it with
            //the contents
            if(Cells.ContainsKey(name))
            {
                Cells.Remove(name);
                Cells.Add(name, cell);
            }
            //else add the cell to the Cells hashset
            else
            {
                Cells.Add(name, cell);
            }

            //replace the dependees of the name with the empty set
            dependencyGraph.ReplaceDependees(name, emptySet);
            //ALMOST BIG MISTAKE. RECALCULATE AT THE END
            HashSet<String> cellsRecalculated = new HashSet<String>(GetCellsToRecalculate(name));

            return cellsRecalculated;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, double number)
        {
            String varPattern = @"^[a-zA-Z][0-9a-zA-Z]*";
            //empty set for our name
            HashSet<String> emptySet = new HashSet<string>();

            //create new cell with number as contents
            Cell cell = new Cell(number);

            //if name is null or does not match the regex then throw exception
            if (name == null || !Regex.IsMatch(name, varPattern))
            {
                throw new InvalidNameException();
            }

            //if cell contains key, remove it then add it back
            if(Cells.ContainsKey(name))
            {
                Cells.Remove(name);
                Cells.Add(name, cell);
            }
            //else just add it to the cells
            else
            {
                Cells.Add(name, cell);
            }

            //replace the dependencies with the empty set
            dependencyGraph.ReplaceDependees(name, emptySet);

            //ALMOST BIG MISTAKE. RECALCULATE AT THE END
            HashSet<String> cellsRecalculated = new HashSet<String>(GetCellsToRecalculate(name));

            return cellsRecalculated;
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            //if cells does not contain key throw exception
            if(!Cells.ContainsKey(name))
            {
                throw new InvalidNameException();
            }
            // else if name == null then throw exception
            else if(name == null)
            {
                throw new ArgumentNullException();
            }
            //return dependents of name
            return dependencyGraph.GetDependents(name);
        }

        /// <summary>
        /// Representation for our spreadsheet cells
        /// </summary>
        private class Cell
        {
            private object contents;

            /// <summary>
            /// Zero argument constructor to represent an empty
            /// Cell
            /// </summary>
            public Cell()
            {
                this.contents = null;
            }

            /// <summary>
            /// One argument constructor that takes a formula
            /// </summary>
            /// <param name="contents"></param>
            public Cell(Formula contents)
            {
                this.contents = contents;
            }

            /// <summary>
            /// One argument constructor that takes a double
            /// </summary>
            /// <param name="contents"></param>
            public Cell(Double contents)
            {
                this.contents = contents;
            }

            /// <summary>
            /// One argument constructor that takes a String
            /// </summary>
            /// <param name="contents"></param>
            public Cell(String contents)
            {
                this.contents = contents;
            }

            /// <summary>
            /// Public getter to return contents
            /// </summary>
            public object getContents
            {
                get { return contents; }
            }
        }
    }
}
