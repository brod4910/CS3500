using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;
using SSGui;

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

        public event Action CloseEvent;

        public event Action NewEvent;

        public event Action<string, string> AddCell;

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
            CellValueLabel.Text = CellName(row, col, value); ;
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

        private void MenuItemOpen_Click(object sender, EventArgs e)
        {
            string savedFile = "";
            saveFileDialog.InitialDirectory = "C:/Users";
            saveFileDialog.Title = "Open a Spreadsheet File";
            saveFileDialog.FileName = "";
            saveFileDialog.Filter = "Spreadsheet File (*.ss)|*.ss|All Files (*.*)|*.*";
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                if (FileChosen != null)
                {
                    FileChosen(openFileDialog.FileName);
                }
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
            if(e.KeyChar == (char) Keys.Enter)
                EnterButton_Click(sender, e);
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
            spreadsheetPanel.SetValue(col, row, cellContents);
            // Add cell contents and name to Spreadsheet
            string result;

            Char c = (Char)(col + 97);

            result = c + "" + (row + 1);
            try
            {
                AddCell(result, cellContents);
                CellValueLabel.Text = CellName(row, col, cellContents);
            }
            catch(Exception)
            {
                // Will be either Circular Exception or a Formula Evaluation Error
                CellValueLabel.Text = "ERROR!!!";
                spreadsheetPanel.SetValue(col, row, "ERROR!!!");
            }
           // CellValueLabel.Text = CellName(row, col, cellContents);

            SetCellContentsTextBox.Text = "";
        }
    }
}
