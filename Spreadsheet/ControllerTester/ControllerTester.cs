using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;
using SS;

namespace ControllerTester
{
    [TestClass]
    public class ControllerTester
    {
        [TestMethod]
        public void TestGetSetCell()
        {
            ISpreadsheetStub stub = new ISpreadsheetStub();
            Controller control = new Controller(stub);
            stub.FireSetContentsOfCell("A1", "1");
            Assert.AreEqual("1", stub.FireGetContentsOfCell("A1"));
        }

        [TestMethod]
        public void TestGetCellContent()
        {
            ISpreadsheetStub stub = new ISpreadsheetStub();
            Controller control = new Controller(stub);
            stub.FireSetContentsOfCell("A1", "1");
            Assert.AreEqual("1", stub.FireGetValueOfCell("A1"));
        }

       [TestMethod]
       public void TestSave()
        {
            ISpreadsheetStub stub = new ISpreadsheetStub();
            Controller control = new Controller(stub);
            stub.FireSetContentsOfCell("A1", "1");
            stub.FireSaveSpreadsheet("test.ss");
            Assert.IsTrue(stub.hasSavedSpreadsheet);
        }
        // TestNewWindow Causes random error
        /*
                [TestMethod]
                public void TestNewWindow()
                {
                    ISpreadsheetStub stub = new ISpreadsheetStub();
                    Controller control = new Controller(stub);
                    stub.FireSetContentsOfCell("A1", "1");
                    stub.FireNewWindow();
                    Assert.IsTrue(stub.hasNewWindow);
                }
                */
        [TestMethod]
        public void TestOpen()
        {
            ISpreadsheetStub stub = new ISpreadsheetStub();
            Controller control = new Controller(stub);
            stub.FireOpenSpreadsheet("test.ss");
            Assert.IsTrue(stub.hasFileOpen);
        }


    }
}
