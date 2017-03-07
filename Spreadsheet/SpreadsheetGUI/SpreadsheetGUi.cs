using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;

namespace SpreadsheetGUI
{
    public partial class SpreadsheetGUI : Form, ISpreadsheetView
    {

        /// <summary>
        /// Creates a top-level view of the Spreadsheet
        /// </summary>
        public SpreadsheetGUI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fired when a file is chosen
        /// </summary>
        public event Action<string> FileChosen;

        /// <summary>
        /// Fired when the window is closed
        /// </summary>
        public event Action CloseWindow;

        /// <summary>
        /// Fired when the spreadsheet is saved
        /// </summary>
        public event Action<string> SaveSpreadsheet;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog.ShowDialog();

            if(result == DialogResult.Yes || result == DialogResult.OK)
            {
                if(SaveSpreadsheet != null)
                {
                    SaveSpreadsheet(saveFileDialog.FileName);
                }
            }
        }

        private void spreadsheetPanel1_Load(object sender, EventArgs e)
        {

        }
    }
}
