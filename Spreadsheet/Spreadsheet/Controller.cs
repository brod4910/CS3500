using System;
using System.IO;
using System.Text.RegularExpressions;

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
            window.AddCell += HandleCell;
            window.SaveSpreadsheet += HandleFileSave;
            this.model = new Spreadsheet();
        }

        /// <summary>
        /// Handles request to open a file
        /// </summary>
        private void HandleFileChosen(string filename)
        {
            // I think we need to brute force put items back into Spreadsheet
            Regex varPattern = new Regex(@"^[a-zA-Z]+[1-9]+[0-9]*$");
            try
            {
                TextReader sr = new StringReader(filename);
                this.model = new Spreadsheet(sr, varPattern);
                window.Title = filename;
                window.message = "Successfully loaded " + filename;
            }
            catch (Exception ex)
            {
                window.message = "Unable to open file\n" + ex.Message;
            }
        }

        /// <summary>
        /// Handles request to save a file
        /// </summary>
        private void HandleFileSave(string filename)
        {
            try
            {
                TextWriter sw = new StreamWriter(filename);
                this.model.Save(sw);
                window.Title = filename;
                window.message = "Successfully Saved " + filename;
            }
            catch (Exception ex)
            {
                window.message = "Unable to save file\n" + ex.Message;
            }
        }

        private void HandleCell(string name, string content)
        {
            this.model.SetContentsOfCell(name, content);
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
