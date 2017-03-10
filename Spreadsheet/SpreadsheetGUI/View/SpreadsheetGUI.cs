using System;
using System.Windows.Forms;
using SSGui;
using System.Collections.Generic;

namespace SpreadsheetGUI
{
    public partial class SpreadSheetGUI : Form, ISpreadsheetView
    {
        public string Title
        {
            set
            {
                label1.Text = value;
            }
        }

        public string message
        {
            set
            {
                label2.Text = value;
            }
        }

        public string cellValue
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public string cellContents
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Fired when a file is chosen
        /// </summary>
        public event Func<string, string> FileChosen;

        /// <summary>
        /// Fired when the spreadsheet is saved
        /// </summary>
        public event Func<string, string> SaveSpreadsheet;

        /// <summary>
        /// Fired when Cell content is needed
        /// </summary>
        public event Func<String, String> GetCellContent;

        /// <summary>
        /// Sets the contents of any given cell
        /// </summary>
        public event Func<string, string, ISet<string>> SetContentsofCell;

        /// <summary>
        /// Get the Value of any given cell
        /// </summary>
        public event Func<string, string> GetCellValue;

        /// <summary>
        /// Handles opening of a new window
        /// </summary>
        public event Action NewWindow;

        /// <summary>
        /// Creates a top-level view of the Spreadsheet
        /// </summary>
        public SpreadSheetGUI()
        {
            InitializeComponent();
            string value;
            spreadsheetPanel.SelectionChanged += displaySelection;
            spreadsheetPanel.SetSelection(0, 0);
            spreadsheetPanel.GetValue(0, 0, out value);
            CellValueLabel.Text = CellName(0, 0, value);
        }

        /// <summary>
        /// Every time the selection changes, this method is called with the
        /// Spreadsheet as its parameter.  We display the current time in the cell.
        /// </summary>
        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            String value;
            ss.GetSelection(out col, out row);
            ss.GetValue(col, row, out value);
            CellValueLabel.Text = CellName(row, col, value);

            string result;

            Char c = (Char)(col + 97);

            result = c + "" + (row + 1);

            SetCellContentsTextBox.Text = GetCellContent(result);
        }

        /// <summary>
        /// Calculates the cell name for any given cell
        /// on the grid and cell value.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private string CellName(int row, int col, string value)
        {
            string result;

            Char c = (Char)(col + 97);

            result = c + " " + (row + 1);

            return "Cell name: " + result.ToUpper() + "  Cell Value: " + value;
        }

        /// <summary>
        /// Displays openFile dialog when the user wants to
        /// open a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemOpen_Click(object sender, EventArgs e)
        {

            string openFile = "";
            saveFileDialog.InitialDirectory = "C:/Users";
            saveFileDialog.Title = "Open a Spreadsheet File";
            saveFileDialog.FileName = "";
            saveFileDialog.Filter = "Spreadsheet File (*.ss)|*.ss|All Files (*.*)|*.*";
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                if (FileChosen != null)
                {
                    openFile = openFileDialog.FileName;
                    MessageBox.Show(FileChosen(openFile));
                    //PopulateGUI();
                }
            }
        }

        /// <summary>
        /// Populates the GUI when loaded from a file
        /// </summary>
        public void PopulateGUI()
        {
            Char c;

            for(int i = 0; i < 26;i++)
            {
                c = (Char)(97 + i);

                for(int j = 0; j < 99;j++)
                {
                    spreadsheetPanel.SetValue(i, j, GetCellValue(c + "" + (j + 1)));
                }
            }
        }

        /// <summary>
        /// Handles the saveFile dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSave_Click(object sender, EventArgs e)
        {
            saveFileDialog.InitialDirectory = "C:/Users";
            saveFileDialog.Title = "Save a Spreadsheet File";
            saveFileDialog.FileName = "";
            saveFileDialog.Filter = "Spreadsheet File (*.ss)|*.ss|All Files (*.*)|*.*";

            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                if (SaveSpreadsheet != null)
                {
                    MessageBox.Show(SaveSpreadsheet(saveFileDialog.FileName));
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoClose();
        }

        /// <summary>
        /// Sets the contents of the selected cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetCellContentsTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Enter Key is pressed
            if (e.KeyChar == (char)Keys.Enter)
            {
                EnterButton_Click(sender, e);
            }
        }

        /// <summary>
        /// Closes current window
        /// </summary>
        public void DoClose()
        {
            // Figure out Sender issues
            if (label1.Text.ToString() != "Unsaved Changes")
            {
                SpreadsheetApplicationContext.GetContext().ExitThread();
            }
            else
            {
                if (MessageBox.Show("Do you want to save changes to this spreadsheet?", "Save", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MenuItemSave_Click(null, null);
                    SpreadsheetApplicationContext.GetContext().ExitThread();
                }
            }
        }

        /// <summary>
        /// Opens new empty window
        /// </summary>
        public void OpenNew()
        {
            NewWindow();
        }

        /// <summary>
        /// Handles new in tool bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenNew();
        }

        /// <summary>
        /// Sets the contents of the selected cell with
        /// the button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterButton_Click(object sender, EventArgs e)
        {
            int row, col;
            
            String cellContents = SetCellContentsTextBox.Text;
            spreadsheetPanel.GetSelection(out col, out row);

            // Add cell contents and name to Spreadsheet
            string name;
            Char charac = (Char)(col + 97);
            name = charac + "" + (row + 1);
            try
            {
                ISet<string> values = SetContentsofCell(name, cellContents);

                int c, r;
                //set value of cell
                foreach (string value in values)
                {
                    c = getCol(value);
                    r = getRow(value);
                    if (GetCellValue(value).Equals("SS.FormulaError"))
                    {
                        spreadsheetPanel.SetValue(c, r, "Formula Error!");
                    }
                    else
                    {
                        spreadsheetPanel.SetValue(c, r, GetCellValue(value));
                    }
                }

                SetCellContentsTextBox.Text = "";
            }
            catch(Exception)
            {
                MessageBox.Show("Invalid Formula has been entered.");
            }

            if (GetCellValue(name).Equals("SS.FormulaError"))
            {
                CellValueLabel.Text = CellName(row, col, "Formula Error!");
            }
            else
            {
                CellValueLabel.Text = CellName(row, col, GetCellValue(name));
            }

            label1.Text = "Unsaved Changes";
        }

        /// <summary>
        /// Gets the column of the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int getCol (string name)
        {
            char charac;

            Char.TryParse(name.Substring(0, 1), out charac);

            return charac - 65;
        }

        /// <summary>
        /// Gets the row of the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int getRow(string name)
        {
            int row;

            int.TryParse(name.Substring(1), out row);

            return row - 1;
        }

        /// <summary>
        /// Handles when the form closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpreadsheetGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.TaskManagerClosing)
            {
                // Figure out Sender issues
                if (label1.Text.ToString() != "Unsaved Changes")
                {

                }
                else
                {
                    if (MessageBox.Show("Do you want to save changes to this spreadsheet?", "Save", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        MenuItemSave_Click(sender, null);
                    }
                }
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Spreadsheet Application for PS7 \nTo Use this spreadsheet, first you will need to click on a cell and enter a value \n" + 
                "Values can be formulas, doubles or strings \nAny string that can be interpretated as a double, will be valued as a double and every Formula is denoted with a = as its first letter \n "
                + "", "Help", MessageBoxButtons.OK);
        }
    }
}
