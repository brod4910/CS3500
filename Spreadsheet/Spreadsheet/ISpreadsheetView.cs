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
        event Action<String> FileChosen;
        event Action CloseEvent;
        event Action NewEvent;
        void DoClose();
        void OpenNew();
        string Title { set; }
        string message { set; }
    }
}
