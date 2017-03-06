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

        public event Action<string> FileChosen;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
