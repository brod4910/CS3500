using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;
using SSGui;
using System.Collections.Generic;
using System.Xml;
using Formulas;

namespace SpreadsheetGUI
{
    public partial class SpreadsheetGUI : Form, ISpreadsheetView
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
        public event Action<string> FileChosen;

        /// <summary>
        /// Fired when the spreadsheet is saved
        /// </summary>
        public event Action<string> SaveSpreadsheet;

        public event Func<String, String> GetCellContent;

        public event Action CloseEvent;

        public event Action NewEvent;

        /// <summary>
        /// Sets the contents of any given cell
        /// </summary>
        public event Func<string, string, ISet<string>> SetContentsofCell;

        public event Func<string, string> GetCellValue;

        /// <summary>
        /// Creates a top-level view of the Spreadsheet
        /// </summary>
        public SpreadsheetGUI()
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

        private void CellNametoRow_col(out int col , out int row, string name)
        {
            col = (int)(name[0] - 97 + 32);
            row = int.Parse(name[1].ToString()) - 1;
        }

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
                    FileChosen(openFile);
                    read_file(openFile);
                }
            }
        }

        /// <summary>
        /// Helper method to parse XML file.
        /// </summary>
        private void read_file(string file)
        {
            int col, row;
            if (file == null)
            {
                throw new ArgumentNullException();
            }
            try
            {
                string content, value;
                using (XmlReader reader = XmlReader.Create(file))
                {
                    while (reader.Read())
                    {
                        if (reader.Name == "cell")
                        {
                            content = reader["name"];
                            CellNametoRow_col(out col, out row, content);
                            value = reader["contents"];
                            spreadsheetPanel.SetValue(col, row, value);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new FormatException();
            }
        }

        private void MenuItemSave_Click(object sender, EventArgs e)
        {
            string savedFile = "";
            saveFileDialog.InitialDirectory = "C:/Users";
            saveFileDialog.Title = "Save a Spreadsheet File";
            saveFileDialog.FileName = "";
            saveFileDialog.Filter = "Spreadsheet File (*.ss)|*.ss|All Files (*.*)|*.*";

            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                if (SaveSpreadsheet != null)
                {
                    SaveSpreadsheet(saveFileDialog.FileName);
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetCellContentsTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Enter Key is pressed
            if (e.KeyChar == (char)Keys.Enter)
            {
                EnterButton_Click(sender, e);
            }
        }

        public void DoClose()
        {
            Close();
        }

        public void OpenNew()
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenNew();
        }

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
                    spreadsheetPanel.SetValue(c, r, GetCellValue(value));
                }

                SetCellContentsTextBox.Text = "";
            }
            catch(FormulaFormatException)
            {
                MessageBox.Show("Invalid Formula has been entered.");
            }


            CellValueLabel.Text = CellName(row, col, GetCellValue(name));

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
    }
}
