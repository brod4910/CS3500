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

        public void FireCloseEvent()
        {
            if(CloseEvent != null)
                CloseEvent();
        }

        public void FireOpenEvent()
        {
            if (NewEvent != null)
                NewEvent();
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
                hasTitle = value;
            }
            get
            {
                return hasTitle;
            }
        }

        public void FireNewWindow()
        {
            if (NewWindow != null)
            {
                NewWindow();
                hasNewWindow = true;
            }
        }

        public void FireSetContentsOfCell(string name, string value)
        {
            SetContentsofCell(name, value);
        }

        public object FireGetContentsOfCell(string name)
        {
            return GetCellContent(name);
        }

        public object FireGetValueOfCell(string name)
        {
            return GetCellValue(name);
        }

        public void FireSaveSpreadsheet(string filename)
        {
            SaveSpreadsheet(filename);
            hasSavedSpreadsheet = true;  
        }

        public void FireOpenSpreadsheet(string filename)
        {
                FileChosen(filename);
                hasFileOpen = true;
            
        }

        public event Action CloseEvent;
        public event Func<String, String> FileChosen;
        public event Func<string, string> GetCellContent;
        public event Func<string, string> GetCellValue;
        public event Action NewEvent;
        public event Action NewWindow;
        public event Func<String, String> SaveSpreadsheet;
        public event Func<string, string, ISet<string>> SetContentsofCell;
        public bool HasClosed;
        public string hasTitle;
        public bool HasNew;
        public bool hasNewWindow;
        public bool hasSavedSpreadsheet;
        public bool hasFileOpen;
        private Func<string, string> _savespreadsheet;
        private Func<string, string> _openspreadsheet;

       
        public void DoClose()
        {
            HasClosed = true;
        }

        public void OpenNew()
        {
            HasNew = true;
        }
    }
}
