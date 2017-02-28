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
            Assert.AreEqual(spreadsheet.GetCellContents("A1").ToString(), "8");
        }

        [TestMethod]
        public void TestMethodSetCellContents2()
        {
            spreadsheet.SetContentsOfCell("A2", "=A1 + 8");
            Assert.AreEqual(spreadsheet.GetCellValue("A2"), 16.0);
            Assert.AreEqual(spreadsheet.GetCellContents("A2").ToString(), "A1+8");
        }

        [TestMethod]
        public void TestMethodSetCellContents3()
        {
            spreadsheet.SetContentsOfCell("A3", "=A2 + A1");
            Assert.AreEqual(spreadsheet.GetCellValue("A3"), 24.0);
            Assert.AreEqual(spreadsheet.GetCellContents("A3").ToString(), "A2+A1");
        }

        [TestMethod]
        public void TestMethodSetCellContents4()
        {
            spreadsheet.SetContentsOfCell("A3", "=A2 - A1");
            Assert.AreEqual(spreadsheet.GetCellValue("A3"), 8.0);
            Assert.AreEqual(spreadsheet.GetCellContents("A3").ToString(), "A2-A1");
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
            Assert.AreEqual(spreadsheet.GetCellContents("A21"), "");
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
        public void TestSetCellContents10()
        {
            spreadsheet.SetContentsOfCell("B3", "=1/0");

            FormulaError formerr = (FormulaError)spreadsheet.GetCellValue("B3");

            Assert.AreEqual(formerr.Reason, "Division by zero has occured.");
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
            Assert.AreEqual(spreadsheet.GetCellContents("A7").ToString(), "String");
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

            while (true)
            {
                String line1 = readf.ReadLine();

                String line2 = readf1.ReadLine();

                Assert.AreEqual(line1, line2);

                if(line1 == null)
                {
                    Assert.IsTrue(true);
                }
            }
        }

        [TestMethod]
        public void testIsValidRegex1()
        {
            Spreadsheet spread = new Spreadsheet();
            PrivateObject obj = new PrivateObject(spread);

            String[] param = new[] { "8" };

            Object isvalid = obj.Invoke("isValidRegex", param);

            Assert.IsFalse(isvalid.Equals(false));
        }


        [TestMethod]
        public void testIsValidRegex2()
        {
            Spreadsheet spread = new Spreadsheet();
            PrivateObject obj = new PrivateObject(spread);

            String[] param = new[] { "\\d+\\d*" };

            Object isvalid = obj.Invoke("isValidRegex", param);

            Assert.IsTrue(isvalid.Equals(true));
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

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetVersionException))]
        public void testReadMethodSpreadSheetReadException11()
        {
            StreamReader stream = new StreamReader("../../SpreadsheetException11.xml");

            Spreadsheet sheet = new Spreadsheet(stream, new Regex(@"^\d+?"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void testReadMethodSpreadSheetReadException12()
        {
            StreamReader stream = new StreamReader("../../SpreadsheetException12.xml");

            Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void testReadMethodSpreadSheetReadException13()
        {
            StreamReader stream = new StreamReader("../../SpreadsheetException13.xml");

            Spreadsheet sheet = new Spreadsheet(stream, new Regex(".*"));
        }

        //____________________________________________________________________

        /// <summary>
        /// Used to make assertions about set equality.  Everything is converted first to
        /// upper case.
        /// </summary>
        public static void AssertSetEqualsIgnoreCase(IEnumerable<string> s1, IEnumerable<string> s2)
        {
            var set1 = new HashSet<String>();
            foreach (string s in s1)
            {
                set1.Add(s.ToUpper());
            }

            var set2 = new HashSet<String>();
            foreach (string s in s2)
            {
                set2.Add(s.ToUpper());
            }

            Assert.IsTrue(new HashSet<string>(set1).SetEquals(set2));
        }

        // EMPTY SPREADSHEETS
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("AA");
        }

        [TestMethod()]
        public void Test3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test4()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "1.5");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test5()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1A", "1.5");
        }

        [TestMethod()]
        public void Test6()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.5");
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test7()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test8()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hello");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test9()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("AZ", "hello");
        }

        [TestMethod()]
        public void Test10()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test11()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, new Formula("2").ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test12()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("AZ", new Formula("2").ToString());
        }

        [TestMethod()]
        public void Test13()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "=" + new Formula("3").ToString());
            Formula f = (Formula)s.GetCellContents("Z7");
            Assert.AreEqual(3, f.Evaluate(x => 0), 1e-6);
        }

        // CIRCULAR FORMULA DETECTION
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test14()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=" + new Formula("A2").ToString());
            s.SetContentsOfCell("A2", "=" + new Formula("A1").ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test15()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=" + new Formula("A2+A3").ToString());
            s.SetContentsOfCell("A3", "=" + new Formula("A4+A5").ToString());
            s.SetContentsOfCell("A5", "=" + new Formula("A6+A7").ToString());
            s.SetContentsOfCell("A7", "=" + new Formula("A1+A1").ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test16()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=" + new Formula("A2+A3").ToString());
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", "=" + new Formula("A3*A1").ToString());
            }
            catch (CircularException e)
            {
                Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod()]
        public void Test17()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test18()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test19()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("C2", "hello");
            s.SetContentsOfCell("C2", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test20()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            AssertSetEqualsIgnoreCase(s.GetNamesOfAllNonemptyCells(), new string[] { "B1" });
        }

        [TestMethod()]
        public void Test21()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "52.25");
            AssertSetEqualsIgnoreCase(s.GetNamesOfAllNonemptyCells(), new string[] { "B1" });
        }

        [TestMethod()]
        public void Test22()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "=" + new Formula("3.5").ToString());
            AssertSetEqualsIgnoreCase(s.GetNamesOfAllNonemptyCells(), new string[] { "B1" });
        }

        [TestMethod()]
        public void Test23()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "hello");
            s.SetContentsOfCell("B1", "=" + new Formula("3.5").ToString());
            AssertSetEqualsIgnoreCase(s.GetNamesOfAllNonemptyCells(), new string[] { "A1", "B1", "C1" });
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod()]
        public void Test24()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            s.SetContentsOfCell("C1", "=" + new Formula("5").ToString());
            AssertSetEqualsIgnoreCase(s.SetContentsOfCell("A1", "17.2"), new string[] { "A1" });
        }

        [TestMethod()]
        public void Test25()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "=" + new Formula("5").ToString());
            AssertSetEqualsIgnoreCase(s.SetContentsOfCell("B1", "hello"), new string[] { "B1" });
        }

        [TestMethod()]
        public void Test26()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("B1", "hello");
            AssertSetEqualsIgnoreCase(s.SetContentsOfCell("C1", "=" + new Formula("5").ToString()), new string[] { "C1" });
        }

        [TestMethod()]
        public void Test27()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=" + new Formula("A2+A3").ToString());
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", "=" + new Formula("A2+A4").ToString());
            s.SetContentsOfCell("A4", "=" + new Formula("A2+A5").ToString());
            HashSet<string> result = new HashSet<string>(s.SetContentsOfCell("A5", "82.5"));
            AssertSetEqualsIgnoreCase(result, new string[] { "A5", "A4", "A3", "A1" });
        }

        // CHANGING CELLS
        [TestMethod()]
        public void Test28()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=" + new Formula("A2+A3").ToString());
            s.SetContentsOfCell("A1", "2.5");
            Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
        }

        [TestMethod()]
        public void Test29()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=" + new Formula("A2+A3").ToString());
            s.SetContentsOfCell("A1", "Hello");
            Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
        }

        [TestMethod()]
        public void Test30()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "Hello");
            s.SetContentsOfCell("A1", "=" + new Formula("23").ToString());
            Assert.AreEqual(23, ((Formula)s.GetCellContents("A1")).Evaluate(x => 0));
        }

        // STRESS TESTS
        [TestMethod()]
        public void Test31()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=" + new Formula("B1+B2").ToString());
            s.SetContentsOfCell("B1", "=" + new Formula("C1-C2").ToString());
            s.SetContentsOfCell("B2", "=" + new Formula("C3*C4").ToString());
            s.SetContentsOfCell("C1", "=" + new Formula("D1*D2").ToString());
            s.SetContentsOfCell("C2", "=" + new Formula("D3*D4").ToString());
            s.SetContentsOfCell("C3", "=" + new Formula("D5*D6").ToString());
            s.SetContentsOfCell("C4", "=" + new Formula("D7*D8").ToString());
            s.SetContentsOfCell("D1", "=" + new Formula("E1").ToString());
            s.SetContentsOfCell("D2", "=" + new Formula("E1").ToString());
            s.SetContentsOfCell("D3", "=" + new Formula("E1").ToString());
            s.SetContentsOfCell("D4", "=" + new Formula("E1").ToString());
            s.SetContentsOfCell("D5", "=" + new Formula("E1").ToString());
            s.SetContentsOfCell("D6", "=" + new Formula("E1").ToString());
            s.SetContentsOfCell("D7", "=" + new Formula("E1").ToString());
            s.SetContentsOfCell("D8", "=" + new Formula("E1").ToString());
            ISet<String> cells = s.SetContentsOfCell("E1", "0");
            AssertSetEqualsIgnoreCase(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }, cells);
        }
        [TestMethod()]
        public void Test32()
        {
            Test31();
        }
        [TestMethod()]
        public void Test33()
        {
            Test31();
        }
        [TestMethod()]
        public void Test34()
        {
            Test31();
        }

        [TestMethod()]
        public void Test35()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++)
            {
                cells.Add("A" + i);
                AssertSetEqualsIgnoreCase(cells, s.SetContentsOfCell("A" + i, "=" + new Formula("A" + (i + 1)).ToString()));
            }
        }
        [TestMethod()]
        public void Test36()
        {
            Test35();
        }
        [TestMethod()]
        public void Test37()
        {
            Test35();
        }
        [TestMethod()]
        public void Test38()
        {
            Test35();
        }
        [TestMethod()]
        public void Test39()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            for (int i = 1; i < 200; i++)
            {
                s.SetContentsOfCell("A" + i, "=" + new Formula("A" + (i + 1)).ToString());
            }
            try
            {
                s.SetContentsOfCell("A150", "=" + new Formula("A50").ToString());
                Assert.Fail();
            }
            catch (CircularException)
            {
            }
        }
        [TestMethod()]
        public void Test40()
        {
            Test39();
        }
        [TestMethod()]
        public void Test41()
        {
            Test39();
        }
        [TestMethod()]
        public void Test42()
        {
            Test39();
        }

        [TestMethod()]
        public void Test43()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++)
            {
                s.SetContentsOfCell("A1" + i, "=" + new Formula("A1" + (i + 1)).ToString());
            }

            ISet<string> sss = s.SetContentsOfCell("A1499", "25.0");
            Assert.AreEqual(500, sss.Count);
            for (int i = 0; i < 500; i++)
            {
                Assert.IsTrue(sss.Contains("A1" + i) || sss.Contains("a1" + i));
            }

            sss = s.SetContentsOfCell("A1249", "25.0");
            Assert.AreEqual(250, sss.Count);
            for (int i = 0; i < 250; i++)
            {
                Assert.IsTrue(sss.Contains("A1" + i) || sss.Contains("a1" + i));
            }


        }

        [TestMethod()]
        public void Test44()
        {
            Test43();
        }
        [TestMethod()]
        public void Test45()
        {
            Test43();
        }
        [TestMethod()]
        public void Test46()
        {
            Test43();
        }

        //[TestMethod()]
        //public void Test47()
        //{
        //    RunRandomizedTest(47, 2519);
        //}
        //[TestMethod()]
        //public void Test48()
        //{
        //    RunRandomizedTest(48, 2521);
        //}
        //[TestMethod()]
        //public void Test49()
        //{
        //    RunRandomizedTest(49, 2526);
        //}
        //[TestMethod()]
        //public void Test50()
        //{
        //    RunRandomizedTest(50, 2521);
        //}

        //    public void RunRandomizedTest(int seed, int size)
        //    {
        //        AbstractSpreadsheet s = new Spreadsheet();
        //        Random rand = new Random(seed);
        //        for (int i = 0; i < 10000; i++)
        //        {
        //            try
        //            {
        //                switch (rand.Next(3))
        //                {
        //                    case 0:
        //                        s.SetContentsOfCell(randomName(rand), "3.14");
        //                        break;
        //                    case 1:
        //                        s.SetContentsOfCell(randomName(rand), "hello");
        //                        break;
        //                    case 2:
        //                        s.SetContentsOfCell(randomName(rand), randomFormula(rand));
        //                        break;
        //                }
        //            }
        //            catch (CircularException)
        //            {
        //            }
        //        }
        //        ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
        //        Assert.AreEqual(size, set.Count);
        //    }

        //    private String randomName(Random rand)
        //    {
        //        return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
        //    }

        //    private String randomFormula(Random rand)
        //    {
        //        String f = randomName(rand);
        //        for (int i = 0; i < 10; i++)
        //        {
        //            switch (rand.Next(4))
        //            {
        //                case 0:
        //                    f += "+";
        //                    break;
        //                case 1:
        //                    f += "-";
        //                    break;
        //                case 2:
        //                    f += "*";
        //                    break;
        //                case 3:
        //                    f += "/";
        //                    break;
        //            }
        //            switch (rand.Next(2))
        //            {
        //                case 0:
        //                    f += 7.2;
        //                    break;
        //                case 1:
        //                    f += randomName(rand);
        //                    break;
        //            }
        //        }
        //        return f;
        //    }
        //}

    }
}
