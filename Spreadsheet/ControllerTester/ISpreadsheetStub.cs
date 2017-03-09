using System;
using System.Collections.Generic;
using SpreadsheetGUI;
using SS;

namespace ControllerTester
{
    class ISpreadsheetStub : ISpreadsheetView
    {
        public string cellContents
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public string cellValue
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

        public string Title
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public event Action CloseEvent;
        public event Action<string> FileChosen;
        public event Func<string, string> GetCellContent;
        public event Func<string, string> GetCellValue;
        public event Action NewEvent;
        public event Action NewWindow;
        public event Action<string> SaveSpreadsheet;
        public event Func<string, string, ISet<string>> SetContentsofCell;

        event Func<string, string> ISpreadsheetView.FileChosen
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event Func<string, string> ISpreadsheetView.SaveSpreadsheet
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public void DoClose()
        {
            throw new NotImplementedException();
        }

        public void OpenNew()
        {
            throw new NotImplementedException();
        }
    }
}
