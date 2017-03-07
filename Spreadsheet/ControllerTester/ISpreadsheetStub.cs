using System;
using SS;

namespace ControllerTester
{
    class ISpreadsheetStub : ISpreadsheetView
    {
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
        public event Action NewEvent;

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
