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
                throw new NotImplementedException();
            }
        }

        public string message
        {
            set
            {
                throw new NotImplementedException();
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

        /// <summary>
        /// Creates a top-level view of the Spreadsheet
        /// </summary>
        public SpreadsheetGUI()
        {
            InitializeComponent();
            spreadsheetPanel.SelectionChanged += displaySelection;
            spreadsheetPanel.SetSelection(1, 1);
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
            if (value == "")
            {
                ss.SetValue(col, row, DateTime.Now.ToLocalTime().ToString("T"));
                ss.GetValue(col, row, out value);
                CellValueLabel.Text = "Cell Name: " + "" + " Cell Value: " + value;
            }
        }

        private void MenuItemOpen_Click(object sender, EventArgs e)
        {
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
        }

        public void DoClose()
        {
            throw new NotImplementedException();
        }

        public void OpenNew()
        {
            throw new NotImplementedException();
        }
    }
}
