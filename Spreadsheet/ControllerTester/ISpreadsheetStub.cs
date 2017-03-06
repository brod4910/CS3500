using System;
using SS;

namespace ControllerTester
{
    class ISpreadsheetStub : ISpreadsheetView
    {
        public event Action<string> FileChosen;
    }
}
