using System;
using System.IO;

namespace SS
{
    /// <summary>
    /// Controls operations between the model and view
    /// </summary>
    public class Controller
    {
        private ISpreadsheetView window;

        private Spreadsheet model;

        /// <summary>
        /// Creates a controller
        /// </summary>
        /// <param name="window"></param>
        public Controller(ISpreadsheetView window)
        {
            this.window = window;
            window.FileChosen += HandleFileChosen;
            window.CloseEvent += HandleClose;
            window.NewEvent += HandleNew;
            this.model = new Spreadsheet();
        }

        /// <summary>
        /// Handles request to open a file
        /// </summary>
        private void HandleFileChosen(string filename)
        {
            try
            {
                TextReader sr = new StringReader(filename);
                this.model = new Spreadsheet(sr, new System.Text.RegularExpressions.Regex(""));
                window.Title = filename;
            }
            catch (Exception ex)
            {
                window.message = "Unable to open file\n" + ex.Message;
            }
        }

        /// <summary>
        /// Handles request to close a file
        /// </summary>
        private void HandleClose()
        {
            window.DoClose();
        }

        /// <summary>
        /// Handles request to open a new Window
        /// </summary>
        private void HandleNew()
        {
            window.OpenNew();
        }

        /// <summary>
        /// Creates a controller from the given XML file.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="file"></param>
        public Controller(ISpreadsheetView view, String file) : this(view)
        {
        }
    }
}
