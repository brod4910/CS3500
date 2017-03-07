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
        /// Fired when a file is chosen
        /// </summary>
        event Action<String> FileChosen;

        /// <summary>
        /// Fired when a window is closed
        /// </summary>
        event Action CloseWindow;

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
    }
}
