using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;
using System.Text.RegularExpressions;
using Dependencies;
using System.IO;
using System.Xml;

namespace SS
{
    // MODIFIED PARAGRAPHS 1-3 AND ADDED PARAGRAPH 4 FOR PS6
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of a regular expression (called IsValid below) and an infinite 
    /// number of named cells.
    /// 
    /// A string is a valid cell name if and only if (1) s consists of one or more letters, 
    /// followed by a non-zero digit, followed by zero or more digits AND (2) the C#
    /// expression IsValid.IsMatch(s.ToUpper()) is true.
    /// 
    /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names, so long as they also
    /// are accepted by IsValid.  On the other hand, "Z", "X07", and "hello" are not valid cell 
    /// names, regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized by converting all letters to upper case before it is used by this 
    /// this spreadsheet.  For example, the Formula "x3+a5" should be normalize to "X3+A5" before 
    /// use.  Similarly, all cell names and Formulas that are returned or written to a file must also
    /// be normalized.
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

        private bool hasChanged;

        private Regex isValid;

        /// <summary>
        /// Zero argument constructor that creates
        /// a new empty spreadsheet
        /// </summary>
        public Spreadsheet()
        {
            this.Cells = new Dictionary<string, Cell>();
            this.dependencyGraph = new DependencyGraph();
            hasChanged = false;
            isValid = new Regex(@".*");
        }

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid 
        /// regular expression is provided as the parameter
        /// </summary>
        /// <param name="isValid"></param>
        public Spreadsheet(Regex isValid) : this()
        {
            this.isValid = isValid;
        }

        /// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
        ///
        /// See the AbstractSpreadsheet.Save method and Spreadsheet.xsd for the file format 
        /// specification.  
        ///
        /// If there's a problem reading source, throws an IOException.
        ///
        /// Else if the contents of source are not consistent with the schema in Spreadsheet.xsd, 
        /// throws a SpreadsheetReadException.  
        ///
        /// Else if the IsValid string contained in source is not a valid C# regular expression, throws
        /// a SpreadsheetReadException.  (If the exception is not thrown, this regex is referred to
        /// below as oldIsValid.)
        ///
        /// Else if there is a duplicate cell name in the source, throws a SpreadsheetReadException.
        /// (Two cell names are duplicates if they are identical after being converted to upper case.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a 
        /// SpreadsheetReadException.  (Use oldIsValid in place of IsValid in the definition of 
        /// cell name validity.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a
        /// SpreadsheetVersionException.  (Use newIsValid in place of IsValid in the definition of
        /// cell name validity.)
        ///
        /// Else if there's a formula that causes a circular dependency, throws a SpreadsheetReadException. 
        ///
        /// Else, create a Spreadsheet that is a duplicate of the one encoded in source except that
        /// the new Spreadsheet's IsValid regular expression should be newIsValid.
        public Spreadsheet(TextReader source, Regex newisValid)
        {
            String line;

            try
            {
                while((line = source.ReadLine()) != null)
                {

                }
            }
            catch(IOException)
            {
                throw new IOException("Problem occured while reading source.");
            }
        }

        // ADDED FOR PS6
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get { return hasChanged; }

            protected set
            {
                if(hasChanged)
                {
                    hasChanged = false;
                }
                else
                {
                    hasChanged = true;
                }
            }
        }

        // ADDED FOR PS6
        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the IsValid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        public override void Save(TextWriter dest)
        {
            using (XmlWriter writer = XmlWriter.Create(dest))
            {
                Cell cell = new Cell();
                IEnumerable <String> listofCells = this.GetNamesOfAllNonemptyCells();

                writer.WriteStartDocument();
                writer.WriteStartElement("", "spreadsheet", "urn:cell-schema");
                writer.WriteAttributeString("isValid", this.isValid.ToString());

                foreach (String key in listofCells)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteAttributeString("name", key);

                    Cells.TryGetValue(key, out cell);

                    if(cell.getContents is Formula)
                    {
                        writer.WriteAttributeString("contents", cell.getContents.ToString());
                    }
                    else if(cell.getContents is Double)
                    {
                        writer.WriteAttributeString("contents", cell.getContents.ToString());
                    }
                    else
                    {
                        String str = (String) cell.getContents;
                        writer.WriteAttributeString("contents", str);
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        
        }

        // ADDED FOR PS6
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            Double value;

            Regex varPattern = new Regex("[a-zA-Z]+[1-9]+[1-9]*");

            if (content == null)
            {
                throw new ArgumentNullException();
            }
            else if (name == null || !isValid.IsMatch(name.ToUpper()) || !varPattern.IsMatch(name))
            {
                throw new InvalidNameException();
            }

            if(Double.TryParse(content, out value) == true)
            {
                return this.SetCellContents(name, value);
            }
            else if(content.IndexOf('=') == 0)
            {
                Normalizer normalizer = s => s.ToUpper();
                Validator validator = s => isValid.IsMatch(s.ToUpper());
                try
                {
                    Formula formula = new Formula(content.Substring(1), normalizer, validator);
                    return this.SetCellContents(name, formula);
                }
                catch(Exception ex) when (ex is FormulaFormatException || ex is CircularException)
                {
                    if (ex is FormulaFormatException)
                    {
                        throw new FormulaFormatException("Formula could not be parsed.");
                    }
                    else
                    {
                        throw new CircularException();
                    }
                }
            }
            else
            {
                return this.SetCellContents(name, content);
            }
        }

        // ADDED FOR PS6
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            object value;

            if(name == null | !isValid.IsMatch(name.ToUpper()))
            {
                throw new InvalidNameException();
            }

            return value;
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
            Regex varPattern = new Regex("[a-zA-Z]+[1-9]+[1-9]*");

            //empty cell
            Cell cell = new Cell();

            //if name is null or regex is not a match
            //throw exception
            if(name == null || !varPattern.IsMatch(name))
            {
                throw new InvalidNameException();
            }

            if (Cells.ContainsKey(name))
            {
                //get the contents of the cell
                Cells.TryGetValue(name, out cell);

                //return the cells
                return cell.getContents;
            }
            else
            {
                return "";
            }
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

                //NEW if cell is an empty string
                //it is considered empty
                if(cell.getContents is String)
                {
                    if(!cell.getContents.Equals(""))
                    {
                        set.Add(name);
                    }
                }
                //if the value is not null add it to the set
                else if(cell.getContents != null)
                {
                    set.Add(name);
                }
            }
            //return the set
            return set;
        }

        // MODIFIED PROTECTION FOR PS6
        /// <summary>
        /// Requires that all of the variables in formula are valid cell names.
        /// 
        /// If name is null or invalid, throws an InvalidNameException.
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
        protected override ISet<String> SetCellContents(String name, Formula formula)
        {
            //regex pattern
            Regex varPattern = new Regex("[a-zA-Z]+[1-9]+[1-9]*");

            // new cell that contains formula as contents
            Cell newCell = new Cell(formula);

            //Old Cell
            Cell oldCell = new Cell();

            //OldCell Contents
            Object oldContent;

            //if name is null or if the reges is not a match then throw
            //exception
            if (name == null || !varPattern.IsMatch(name))
            {
                throw new InvalidNameException();
            }

            //Grab old cell data
            if(Cells.ContainsKey(name))
            {
                Cells.TryGetValue(name, out oldCell);

                oldContent = oldCell.getContents;
            }

            //non-intialized hashset
            HashSet<String> cellsRecalculated;


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
                    Cells.Add(name, newCell);
                }
                //else add the name
                else
                {
                    Cells.Add(name, newCell);
                }

                //ALMOST BIG MISTAKE. RECALCULATE AT THE END
                cellsRecalculated = new HashSet<String>(GetCellsToRecalculate(name));

                return cellsRecalculated;
            }
            catch (CircularException)
            {
                //revert the dependencies back to old ones
                dependencyGraph.ReplaceDependees(name, oldDependencies);
                //Revert old cell values
                Cells.Remove(name);
                Cells.Add(name, oldCell);
                throw new CircularException();
            }
        }

        // MODIFIED PROTECTION FOR PS6
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
        protected override ISet<String> SetCellContents(String name, String text)
        {
            //Var pattern for for our name check
            Regex varPattern = new Regex("[a-zA-Z]+[1-9]+[1-9]*");

            // empty set for our new cell
            HashSet<String> emptySet = new HashSet<String>();

            //if either name is null or text is null or regex is not a match
            //throw an exception
            if (text == null)
            {
                throw new ArgumentNullException();
            }
            else if(name == null || !varPattern.IsMatch(name))
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

        // MODIFIED PROTECTION FOR PS6
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
        protected override ISet<String> SetCellContents(String name, double number)
        {
            Regex varPattern = new Regex("[a-zA-Z]+[1-9]+[1-9]*");

            //empty set for our name
            HashSet<String> emptySet = new HashSet<string>();

            //create new cell with number as contents
            Cell cell = new Cell(number);

            //if name is null or does not match the regex then throw exception
            if (name == null || !varPattern.IsMatch(name))
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
            private object value;

            /// <summary>
            /// Zero argument constructor to represent an empty
            /// Cell
            /// </summary>
            public Cell()
            {
                this.contents = null;
                this.value = null;
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
                this.value = contents;
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
