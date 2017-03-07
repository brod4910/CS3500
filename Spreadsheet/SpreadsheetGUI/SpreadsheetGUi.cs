using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;

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

        /// <summary>
        /// Creates a top-level view of the Spreadsheet
        /// </summary>
        public SpreadsheetGUI()
        {
            InitializeComponent();
        }

        public event Action<string> FileChosen;
        public event Action CloseEvent;
        public event Action NewEvent;

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

        public void DoClose()
        {
            Close();
        }

        public void OpenNew()
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }
    }
}
