using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;
using System.Collections.Generic;
using System.Collections;
using System.IO;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        static Spreadsheet spreadsheet = new Spreadsheet();

        [TestMethod]
        public void TestMethodSetCellContents1()
        {
            spreadsheet.SetContentsOfCell("A1", "8");
            Assert.AreEqual(spreadsheet.GetCellValue("A1"), 8.0);
        }

        [TestMethod]
        public void TestMethodSetCellContents2()
        {
            spreadsheet.SetContentsOfCell("A2", "=A1 + 8");
            Assert.AreEqual(spreadsheet.GetCellValue("A2"), 16.0);
        }

        [TestMethod]
        public void TestMethodSetCellContents3()
        {
            spreadsheet.SetContentsOfCell("A3", "=A2 + A1");
            Assert.AreEqual(spreadsheet.GetCellValue("A3"), 24.0);
        }

        [TestMethod]
        public void TestMethodSetCellContents4()
        {
            spreadsheet.SetContentsOfCell("A3", "=A2 - A1");
            Assert.AreEqual(spreadsheet.GetCellValue("A3"), 8.0);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestMethodSetCellContents5()
        {
            spreadsheet.SetContentsOfCell("A2", "=A1 + A3");
            Assert.AreEqual("", "");
        }

        [TestMethod]
        public void TestMethodSetCellContents6()
        {
            spreadsheet.SetContentsOfCell("A2", "=A1 - A1");
            Assert.AreEqual(spreadsheet.GetCellValue("A2"), 0.0);
        }

        [TestMethod]
        public void TestMethodSetCellContents7()
        {
            spreadsheet.SetContentsOfCell("A4", "=A5 + 1");
            Assert.IsTrue(spreadsheet.GetCellValue("A4") is FormulaError);
        }

        [TestMethod]
        public void TestSaveMethod()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

            string fileName = Path.Combine(path, "Spreadsheet.xml");

            FileStream f = File.Create(@".\Spreadsheet.xml");

            StreamWriter writer = new StreamWriter(f); 
            
            spreadsheet.Save(writer);
        }
    }
}
