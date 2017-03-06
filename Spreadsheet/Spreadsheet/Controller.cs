using System;

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
            this.model = new Spreadsheet();
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
