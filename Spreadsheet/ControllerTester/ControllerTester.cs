using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;
using SS;
using System.IO;

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
            stub.FireSaveSpreadsheet("test1.ss");
            Assert.AreEqual(stub.Title.ToString(),"test1.ss");
        }
        // TestNewWindow Causes random error
        [TestMethod]
        public void TestNewWindow()
        {
            ISpreadsheetStub stub = new ISpreadsheetStub();
            Controller control = new Controller(stub);
            stub.FireSetContentsOfCell("A1", "1");
            stub.FireNewWindow();
            Assert.IsTrue(stub.hasNewWindow);
        }
               
        [TestMethod]
        public void TestOpen()
        {
            ISpreadsheetStub stub = new ISpreadsheetStub();
            Controller control = new Controller(stub);
            string addr = Path.GetFullPath("test.ss");
            stub.FireOpenSpreadsheet(addr);
            Assert.AreEqual(stub.Title.ToString(), addr);
        }

        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void TestOpenFail()
        {
            ISpreadsheetStub stub = new ISpreadsheetStub();
            Controller control = new Controller(stub);
            stub.FireOpenSpreadsheet("testfail.ss");
            // stub.Title is not obstantiated if OpenFile does not succeed
            stub.Title.ToString();
        }

        [TestMethod]
        public void TestSecondConstructor()
        {
            ISpreadsheetStub stub = new ISpreadsheetStub();
            Controller control = new Controller(stub, "test.ss");
        }

        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void TestSaveError()
        {
            ISpreadsheetStub stub = new ISpreadsheetStub();
            Controller control = new Controller(stub);
            stub.FireSetContentsOfCell("A1", "1");
            stub.FireSaveSpreadsheet("");
            // If it never sets the title, then the save threw an error
            stub.Title.ToString();
        }


    }
}
