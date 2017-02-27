using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

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
        public void TestMethodSetCellContents8()
        {
            spreadsheet.SetContentsOfCell("B1", "=A2 + 9");
            Assert.AreEqual(spreadsheet.GetCellValue("B1"), 9.0);
        }

        [TestMethod]
        public void testBeforeSave()
        {
            Assert.IsTrue(spreadsheet.Changed);
        }

        [TestMethod]
        public void TestSaveMethod()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

            string fileName = Path.Combine(path, "Spreadsheet.xml");

            FileStream f = File.Create(@".\Spreadsheet.xml");

            StreamWriter writer = new StreamWriter(f);

            spreadsheet.Save(writer);

            Assert.IsFalse(spreadsheet.Changed);
        }

        [TestMethod]
        public void TestAfterSave()
        {
            spreadsheet.SetContentsOfCell("A1001", "9.0");
            Assert.IsTrue(spreadsheet.Changed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void testNullContent()
        {
            spreadsheet.SetContentsOfCell("Something", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void testNullName()
        {
            spreadsheet.SetContentsOfCell(null, "Something");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void testNullInvalidContent1()
        {
            spreadsheet.SetContentsOfCell("Somthing", "asdf");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void testNullInvalidContent2()
        {
            Spreadsheet sheet = new Spreadsheet(new Regex(@"\d+"));

            sheet.SetContentsOfCell("Somthing", "a");
        }

        [TestMethod]
        public void DivisionByZero()
        {
            spreadsheet.SetContentsOfCell("B1", "=1/0");

            Assert.IsTrue(spreadsheet.GetCellValue("B1") is FormulaError);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValueNullName()
        {
            spreadsheet.GetCellValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValueInvalidName()
        {
            Spreadsheet sheet = new Spreadsheet(new Regex(@"\d+"));

            sheet.GetCellValue("a");
        }

        //[TestMethod]
        //public void TestReadMethod()
        //{
        //    StreamReader stream = new StreamReader(@".\SpreadsheetResult.xml");

        //    Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));

        //    sheet.SetContentsOfCell("A1", "1");
        //}
    }
}
