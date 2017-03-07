using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS
{
    /// <summary>
    /// Controllable interface for ISpreadsheetView
    /// </summary>
    public interface ISpreadsheetView
    {
        /// <summary>
        /// Event is called when a File is chosen from the FileDialog
        /// </summary>
        event Action<String> FileChosen;

        /// <summary>
        /// Fired when the spreadsheet is saved
        /// </summary>
        event Action<String> SaveSpreadsheet;

        /// <summary>
        /// Value for any given cell
        /// </summary>
        String cellValue { set; }

        /// <summary>
        /// Contents for any given cell
        /// </summary>
        String cellContents { set; }

        /// <summary>
        /// Event is fired when the the window is closed
        /// </summary>
        event Action CloseEvent;

        /// <summary>
        /// Event is fired when a request for a new window is made
        /// </summary>
        event Action NewEvent;

        /// <summary>
        /// Spreadsheet must be closable
        /// </summary>
        void DoClose();

        /// <summary>
        /// Spreadsheet must be openable
        /// </summary>
        void OpenNew();

        /// <summary>
        /// Spreadsheet will have a title
        /// </summary>
        string Title { set; }

        /// <summary>
        /// Spreadsheet will have a message indicating state
        /// </summary>
        string message { set; }
    }
}
