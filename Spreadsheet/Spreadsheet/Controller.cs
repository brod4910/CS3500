using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
            window.SetContentsofCell += HandleSetContentsofCell;
            window.SaveSpreadsheet += HandleFileSave;
            window.GetCellValue += HandleGetCellValue;
            window.GetCellContent += HandleGetCellContent;
            this.model = new Spreadsheet(new Regex(@"^[a-zA-Z][1-9]{1}[0-9]{0,1}$"));
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
                MessageBox.Show("Successfully loaded " + filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open file\n" + ex.Message);
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
                MessageBox.Show("Successfully Saved " + filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save file\n" + ex.Message);
            }
        }

        /// <summary>
        /// Sets the Contents of any give cell
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        private ISet<string> HandleSetContentsofCell(string name, string content)
        {
            //set contents of cell
            return this.model.SetContentsOfCell(name, content);
        }

        /// <summary>
        /// Gets the value of any given cell
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private String HandleGetCellValue(string name)
        {
            return this.model.GetCellValue(name).ToString();
        }

        private string HandleGetCellContent(string name)
        {
            return this.model.GetCellContents(name).ToString();
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
