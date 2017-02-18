using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;
using System.Collections.Generic;
using System.Collections;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {


        [TestMethod]
        public void stressTest5()
        {
            stressTest4();
        }

        [TestMethod]
        public void stressTest4()
        {
            stressTest3();
        }

        [TestMethod]
        public void stressTest3()
        {
            stressTest2();
        }

        [TestMethod]
        public void stressTest2()
        {
            stressTest1();
        }

        [TestMethod]
        public void stressTest1()
        {
            // A bunch of strings to use
            const int SIZE = 78;
            string[] letters = new string[78];
            for (int i = 0; i < 26; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            for (int i = 0; i < 26; i++)
            {
                letters[i+26] = ("" + (char)('a' + i));
            }

            for (int i = 0; i < 26; i++)
            {
                letters[i+52] = ("" + (char)('a' + i));
            }

            string[] answer = new string[SIZE];

            for (int i = 0; i < 26;i++)
            {
                answer[i] = letters[i];
            }

            for (int i = 0; i < 26; i++)
            {
                answer[i+26] = letters[i+26];
            }

            for (int i = 0; i < 26; i++)
            {
                answer[i + 52] = letters[i + 52];
            }

            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            for(int i = 0; i < 26; i ++)
            {
                spreadsheet.SetCellContents(letters[i], letters[i]);
            }

            for (int i = 0; i < 26; i++)
            {
                spreadsheet.SetCellContents(letters[i+26], letters[i+26]);
            }

            for (int i = 0; i < 26; i++)
            {
                spreadsheet.SetCellContents(letters[i+52], letters[i+52]);
            }

            for (int i = 0; i < 26; i++)
            {
                Assert.AreEqual((String)spreadsheet.GetCellContents(letters[i]), answer[i]);
            }

            for (int i = 0; i < 26; i++)
            {
                Assert.AreEqual((String)spreadsheet.GetCellContents(letters[i+26]), answer[i+26]);
            }

            for (int i = 0; i < 26; i++)
            {
                Assert.AreEqual((String)spreadsheet.GetCellContents(letters[i+52]), answer[i+52]);
            }
        }

        [TestMethod]
        public void stressTest11()
        {
            stressTest10();
        }


        [TestMethod]
        public void stressTest10()
        {
            stressTest9();
        }


        [TestMethod]
        public void stressTest9()
        {
            stressTest8();
        }


        [TestMethod]
        public void stressTest8()
        {
            stressTest6();
        }


        [TestMethod]
        public void stressTest6()
        {
            // A bunch of strings to use
            const int SIZE = 500;
            Double[] doubles = new Double[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                doubles[i] = i;
            }

            Double[] answer = new Double[SIZE];

            for (int i = 0; i < SIZE; i++)
            {
                answer[i] = doubles[i];
            }

            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            for (int i = 0; i < SIZE; i++)
            {
                spreadsheet.SetCellContents("a" + doubles[i], doubles[i]);
            }

            for (int i = 0; i < SIZE; i++)
            {
                Assert.AreEqual(spreadsheet.GetCellContents( "a" + doubles[i]), answer[i]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestMethod1()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1" };

            Formula formula = new Formula("A1 + B1");

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);

            IEnumerable<String> result = spreadsheet.GetNamesOfAllNonemptyCells();

            foreach(string item in result)
            {
                Assert.IsTrue(set.Contains(item));
            }
        }

        [TestMethod]
        public void testMethod2()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("C1", 9090909);

            IEnumerable<String> result = spreadsheet.GetNamesOfAllNonemptyCells();

            foreach (string item in result)
            {
                Assert.IsTrue(set.Contains(item));
            }
        }

        [TestMethod]
        public void testMethod3()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "A1", "B1", "C1" };

            Formula formula = new Formula("A1 * 2");
            Formula formula1 = new Formula("B1 + A1");


            spreadsheet.SetCellContents("B1", formula);
            spreadsheet.SetCellContents("C1", formula1);

            IEnumerable<String> result = spreadsheet.SetCellContents("A1", 2);

            foreach (string item in result)
            {
                Assert.IsTrue(set.Contains(item));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void testMethod4()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            spreadsheet.SetCellContents("Ax123", null);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("C1", 9090909);

            IEnumerable<String> result = spreadsheet.GetNamesOfAllNonemptyCells();

            foreach (string item in result)
            {
                Assert.IsTrue(set.Contains(item));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void testMethod5()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents(null, formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("C1", 9090909);

            IEnumerable<String> result = spreadsheet.GetNamesOfAllNonemptyCells();

            foreach (string item in result)
            {
                Assert.IsTrue(set.Contains(item));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void testMethod6()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("C1", null);

            IEnumerable<String> result = spreadsheet.GetNamesOfAllNonemptyCells();

            foreach (string item in result)
            {
                Assert.IsTrue(set.Contains(item));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void testMethod7()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + Ax123");

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("C1", formula);

            IEnumerable<String> result = spreadsheet.GetNamesOfAllNonemptyCells();

            foreach (string item in result)
            {
                Assert.IsTrue(set.Contains(item));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void testMethod8()
        {
             AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            spreadsheet.SetCellContents("123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("C1", 9090909);

            IEnumerable<String> result = spreadsheet.GetNamesOfAllNonemptyCells();

            foreach (string item in result)
            {
                Assert.IsTrue(set.Contains(item));
            }
        }

        [TestMethod]
        public void testMethod9()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("C1", 9090909);
            spreadsheet.SetCellContents("A1", 312421);

            IEnumerable<String> result = spreadsheet.GetNamesOfAllNonemptyCells();

            foreach (string item in result)
            {
                Assert.IsTrue(set.Contains(item));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void testMethod10()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("C1", 9090909);

            spreadsheet.GetCellContents(null);

            IEnumerable<String> result = spreadsheet.GetNamesOfAllNonemptyCells();

            foreach (string item in result)
            {
                Assert.IsTrue(set.Contains(item));
            }
        }

        [TestMethod]
        public void testMethod12()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("C1", 9090909);

            Formula form = (Formula)spreadsheet.GetCellContents("A1");

            Assert.AreEqual(form, formula);
        }


        [TestMethod]
        public void testMethod13()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("C1", "!!??");

            String result = (String)spreadsheet.GetCellContents("C1");

            Assert.AreEqual(result, "!!??");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void testMethod14()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            String str = null;

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("C1", str);

            String result = (String)spreadsheet.GetCellContents("C1");

            Assert.AreEqual(result, "!!??");
        }

        [TestMethod]
        public void testMethod15()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("C1", "WTF??");
            spreadsheet.SetCellContents("C1", "Okay!!!");

            String result = (String)spreadsheet.GetCellContents("C1");

            Assert.AreEqual(result, "Okay!!!");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void testMethod16()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            String str = null;

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents(str, "C1");

            String result = (String)spreadsheet.GetCellContents("C1");

            Assert.AreEqual(result, "");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void testMethod17()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            String str = null;

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents(str, 1234);

            String result = (String)spreadsheet.GetCellContents("C1");

            Assert.AreEqual(result, "!!??");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void testMethod18()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("12332", 9898);

            String result = (String)spreadsheet.GetCellContents("C1");

            Assert.AreEqual(result, "!!??");
        }

        [TestMethod]
        public void testMethod19()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            HashSet<String> set = new HashSet<string> { "Ax123", "A1", "B1", "C1" };

            Formula formula = new Formula("C1 + B1");

            spreadsheet.SetCellContents("Ax123", 19);
            spreadsheet.SetCellContents("A1", formula);
            spreadsheet.SetCellContents("B1", 21);
            spreadsheet.SetCellContents("A1", formula);

            Formula result = (Formula)spreadsheet.GetCellContents("A1");

            Assert.AreEqual(result, formula);
        }
    }
}
