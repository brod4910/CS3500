using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Controllable interface for ISpreadsheetView
    /// </summary>
    public interface ISpreadsheetView
    {
        /// <summary>
        /// Event is called when a File is chosen from the FileDialog
        /// </summary>
       event Func<String,String> FileChosen;

        /// <summary>
        /// Fired when the spreadsheet is saved
        /// </summary>
        event Func<String, String> SaveSpreadsheet;

        /// <summary>
        /// Value for any given cell
        /// </summary>
        String cellValue { set; }

        /// <summary>
        /// Contents for any given cell
        /// </summary>
        String cellContents { set; }

        /// <summary>
        /// Handles opening of a new window
        /// </summary>
        void OpenNew();

        /// <summary>
        /// Closes the window
        /// </summary>
        void DoClose();

        /// <summary>
        /// Get the value of any given cell
        /// </summary>
        event Func<String, String> GetCellValue;

        /// <summary>
        /// Get the content of any given cell
        /// </summary>
        event Func<String, String> GetCellContent;

        /// <summary>
        /// Sets the contents of any given cell.
        /// </summary>
        event Func<String, String, ISet<string>> SetContentsofCell;

        /// <summary>
        /// Handles opening of a new window
        /// </summary>
        event Action NewWindow;

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
