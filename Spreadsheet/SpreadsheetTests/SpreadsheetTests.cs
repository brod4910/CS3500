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
        [ExpectedException(typeof(InvalidNameException))]

        public void TestMethodSetCellContents9()
        {
            spreadsheet.SetContentsOfCell("", "");
            Assert.AreEqual(spreadsheet.GetCellValue(""), "");
        }

        [TestMethod]
        public void TestMethod()
        {

        }

        [TestMethod]
        public void TestMethodGetCellContents1()
        {
            Assert.AreEqual(spreadsheet.GetCellValue("A213123"), "");
        }

        [TestMethod]
        public void TestMethodGetCellContents2()
        {
            spreadsheet.SetContentsOfCell("A7", "String");
            spreadsheet.SetContentsOfCell("A8", "=A7 + 8");
            Assert.IsTrue(spreadsheet.GetCellValue("A8") is FormulaError);
        }


        [TestMethod]
        public void testBeforeSave()
        {
            Assert.IsTrue(spreadsheet.Changed);
        }

        [TestMethod]
        public void TestSaveMethod1()
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
        public void TestSaveMethod2()
        {
            FileStream f = File.Create("../../Spreadsheet1.xml");

            FileStream f1 = File.OpenRead("../../Spreadsheet2.xml");

            StreamWriter writer = new StreamWriter(f);

            spreadsheet.Save(writer);

            StreamReader readf = new StreamReader(f);

            StreamReader readf1 = new StreamReader(f1);

            while(true)
            {
                String line1 = readf.ReadLine();

                String line2 = readf1.ReadLine();

                Assert.AreEqual(line1, line2);
            }

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

        [TestMethod]
        public void TestReadMethod()
        {
            StreamReader stream = new StreamReader("../../Spreadsheet.xml");

            Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));

            Assert.AreEqual(sheet.GetCellValue("A1"), 8.0);

            sheet.SetContentsOfCell("A1", "1");

            Assert.AreEqual(sheet.GetCellValue("A1"), 1.0);
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void TestReadMethodIOException()
        {
            StreamReader stream = new StreamReader("../../SpreadsheetError.xml");

            Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));
        }

        [TestMethod]
        public void testReadMethodSpreadSheetReadException1()
        {
            try
            {
                StreamReader stream = new StreamReader("../../SpreadsheetException1.xml");

                Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));
            }
            catch (SpreadsheetReadException ex)
            {
                Assert.AreEqual(ex.Message, "The 'cells' attribute is not declared.");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void testReadMethodSpreadSheetIOException2()
        {
            StreamReader stream = new StreamReader("../../SpreadsheetException2.xml");

            Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));
        }

        [TestMethod]
        public void testReadMethodSpreadSheetReadException3()
        {
            try
            {
                StreamReader stream = new StreamReader("../../SpreadsheetException3.xml");

                Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));
            }
            catch (SpreadsheetReadException ex)
            {
                Assert.AreEqual(ex.Message, "Source does not follow schema");

            }
        }


        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void testReadMethodSpreadSheetReadException4()
        {
            StreamReader stream = new StreamReader("../../SpreadsheetException4.xml");

            Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));
        }

        [TestMethod]
        public void testReadMethodSpreadSheetReadException5()
        {
            try
            {
                StreamReader stream = new StreamReader("../../SpreadsheetException5.xml");

                Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));
            }
            catch (SpreadsheetReadException ex)
            {
                Assert.AreEqual(ex.Message, "The 'content' attribute is not declared.");

            }
        }

        [TestMethod]
        public void testReadMethodSpreadSheetReadException6()
        {
            try
            {
                StreamReader stream = new StreamReader("../../SpreadsheetException6.xml");

                Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));
            }
            catch (SpreadsheetReadException ex)
            {
                Assert.AreEqual(ex.Message, "No Duplicate cell names are allowed.");

            }
        }

        [TestMethod]
        public void testReadMethodSpreadSheetReadException7()
        {
            try
            {
                StreamReader stream = new StreamReader("../../SpreadsheetException7.xml");

                Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));
            }
            catch (SpreadsheetReadException ex)
            {
                Assert.AreEqual(ex.Message, "Invalid cell name with old isValid.");
            }
        }

        [TestMethod]
        public void testReadMethodSpreadSheetReadException8()
        {
            try
            {
                StreamReader stream = new StreamReader("../../SpreadsheetException8.xml");

                Spreadsheet sheet = new Spreadsheet(stream, new Regex(@"\d+"));
            }
            catch (SpreadsheetReadException ex)
            {
                Assert.AreEqual(ex.Message, "Invalid cell name with new isValid.");
            }
        }

        [TestMethod]
        public void testReadMethodSpreadSheetReadException9()
        {
            try
            {
                StreamReader stream = new StreamReader("../../SpreadsheetException9.xml");

                Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));
            }
            catch (SpreadsheetReadException ex)
            {
                Assert.AreEqual(ex.Message, "Source has circular formulas.");
            }
        }

        /// <summary>
        /// Testing Exception where formula is ="==A1-A1"
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void testReadMethodSpreadSheetReadException10()
        {
            StreamReader stream = new StreamReader("../../SpreadsheetException10.xml");

            Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));
        }
    }
}
