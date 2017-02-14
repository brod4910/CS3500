// Written by Joe Zachary for CS 3500, January 2017.
// Brian Rodriguez u0853593

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using System.Collections.Generic;

namespace FormulaTestCases
{
    /// <summary>
    /// These test cases are in no sense comprehensive!  They are intended to show you how
    /// client code can make use of the Formula class, and to show you how to create your
    /// own (which we strongly recommend).  To run them, pull down the Test menu and do
    /// Run > All Tests.
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        /// This tests that a syntactically incorrect parameter to Formula results
        /// in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct1()
        {
            Formula f = new Formula("_");
        }

        /// <summary>
        /// This is another syntax error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct2()
        {
            Formula f = new Formula("2++3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct3()
        {
            Formula f = new Formula("2 3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct4()
        {
            Formula f = new Formula(") * 3");
        }


        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct5()
        {
            Formula f = new Formula("() * 3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct6()
        {
            Formula f = new Formula("( * 3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct7()
        {
            Formula f = new Formula("((((((() * 3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct8()
        {
            Formula f = new Formula("( 3 * 2) x * 3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct9()
        {
            Formula f = new Formula("( 3 * 2 x) x * 3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct10()
        {
            Formula f = new Formula("( 3 * 2) * 3 *");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct11()
        {
            Formula f = new Formula("(( 3 * 2) * 3) (");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct12()
        {
            Formula f = new Formula("(( 3 * 2)) * 3)");
        }

        /// <summary>
        /// No Syntax error.
        /// </summary>
        [TestMethod]
        public void Construct13()
        {
            Formula f = new Formula("2 / 0");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct14()
        {
            Formula f = new Formula("");
        }

        /// <summary>
        /// No syntax error
        /// </summary>
        [TestMethod]
        public void Construct15()
        {
            Formula f = new Formula("0 / 2");
        }

        /// <summary>
        /// Makes sure that "2+3" evaluates to 5.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate1()
        {
            Formula f = new Formula("2+3");
            Assert.AreEqual(f.Evaluate(v => 0), 5.0, 1e-6);
        }

        /// <summary>
        /// The Formula consists of a single variable (x5).  The value of
        /// the Formula depends on the value of x5, which is determined by
        /// the delegate passed to Evaluate.  Since this delegate maps all
        /// variables to 22.5, the return value should be 22.5.
        /// </summary>
        [TestMethod]
        public void Evaluate2()
        {
            Formula f = new Formula("x5");
            Assert.AreEqual(f.Evaluate(v => 22.5), 22.5, 1e-6);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate6()
        {
            Formula f = new Formula("5 / 0");
            Assert.AreEqual(f.Evaluate(v => 22.5), 22.5, 1e-6);
        }

        /// <summary>
        /// Here, the delegate passed to Evaluate always throws a
        /// UndefinedVariableException (meaning that no variables have
        /// values).  The test case checks that the result of
        /// evaluating the Formula is a FormulaEvaluationException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate3()
        {
            Formula f = new Formula("x + y");
            f.Evaluate(v => { throw new UndefinedVariableException(v); });
        }

        /// <summary>
        /// The delegate passed to Evaluate is defined below.  We check
        /// that evaluating the formula returns in 10.
        /// </summary>
        [TestMethod]
        public void Evaluate4()
        {
            Formula f = new Formula("x + y");
            Assert.AreEqual(f.Evaluate(Lookup4), 10.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate5 ()
        {
            Formula f = new Formula("(x + y) * (z / x) * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 20.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluat6 ()
        {
            Formula f = new Formula("(w / 2) + (x / w)");
            Assert.AreEqual(f.Evaluate(Lookup4), 20.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate7()
        {
            Formula f = new Formula("(w / 2) + (x / a)");
            Assert.AreEqual(f.Evaluate(Lookup4), 20.0, 1e-6);
        }

        /// <summary>
        /// Evaluate
        /// </summary>
        [TestMethod]
        public void Evaluate8()
        {
            Formula f = new Formula("((20 - 41) * 15) + (4 / 2) * 20");
            Assert.AreEqual(f.Evaluate(Lookup4), -275, 1e-6);
        }

        /// <summary>
        /// Negative variable input
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate9()
        {
            Formula f = new Formula("((20 - a) * 15) + (4 / 2) * 20");
            Assert.AreEqual(f.Evaluate(Lookup4), -6260, 1e-6);
        }

        /// <summary>
        /// Division by zero
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate10()
        {
            Formula f = new Formula("((20 - 41) * 15) + (4 / w) * 20");
            Assert.AreEqual(f.Evaluate(Lookup4), -6260, 1e-6);
        }

        /// <summary>
        /// Evaluate
        /// </summary>
        [TestMethod]
        public void Evaluate11()
        {
            Formula f = new Formula("((13245 * 400 * .5) / (123 / 123) * (90 / 1))");
            Assert.AreEqual(f.Evaluate(Lookup4), 238410000, 1e-6);
        }


        /// <summary>
        /// Uses an undefined variable!
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate12()
        {
            Formula f = new Formula("((13245 * d * .5) / (123 / 123) * (90 / 1))");
            Assert.AreEqual(f.Evaluate(Lookup4), 238410000, 1e-6);
        }

        /// <summary>
        /// Complicated Evaluation
        /// </summary>
        [TestMethod]
        public void Evaluate13()
        {
            Formula f = new Formula("(((y / x) * z) * 300) + 365 * 10 / 2 + 1000");
            Assert.AreEqual(f.Evaluate(Lookup4), 6425, 1e-6);
        }

        /// <summary>
        /// Complicated Evaluation but divides by zero
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate14()
        {
            Formula f = new Formula("(((y / x) * z) * 300) + 365 * 10 / w + 1000");
            Assert.AreEqual(f.Evaluate(Lookup4), 6425, 1e-6);
        }

        /// <summary>
        /// A string to test if the Strings are equal after it has been
        /// normalized
        /// </summary>
        /// 
        [TestMethod]
        public void toStringTest1()
        {

            Normalizer norm = s => "x3";

            Formula f = new Formula("(((y / x) * z) * 300) + 365 * 10 / w + 1000", norm, s => true);

            string newForm = "(((x3/x3)*x3)*300)+365*10/x3+1000";

            string result = f.ToString();

            Assert.AreEqual(result, newForm);
        }

        /// <summary>
        /// Test that goes outside of the format of the formula
        /// specifications
        /// </summary>
        /// 
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void toStringTest2()
        {

            Normalizer norm = s => "_x3";

            Formula f = new Formula("(((y / x) * z) * 300) + 365 * 10 / w + 1000", norm, s => true);

            string newForm = "(((x3/x3)*x3)*300)+365*10/x3+1000";

            string result = f.ToString();

            Assert.AreEqual(result, newForm);
        }

        /// <summary>
        /// A string to test if the Strings are equal after it has been
        /// normalized
        /// </summary>
        /// 
        [TestMethod]
        public void toStringTest3()
        {

            Normalizer norm = s => "x3134324124";

            Formula f = new Formula("(((y / x) * z) * 300) + 365 * 10 / w + 1000", norm, s => true);

            string newForm = "(((x3134324124/x3134324124)*x3134324124)*300)+365*10/x3134324124+1000";

            string result = f.ToString();

            Assert.AreEqual(result, newForm);
        }

        /// <summary>
        /// A string to test if the Strings are equal after it has been
        /// normalized
        /// </summary>
        /// 
        [TestMethod]
        public void toStringTest4()
        {

            Normalizer norm = s => s.ToUpper();

            Formula f = new Formula("(((y / x) * z) * 300) + 365 * 10 / w + 1000", norm, s => true);

            string newForm = "(((Y/X)*Z)*300)+365*10/W+1000";

            string result = f.ToString();

            Assert.AreEqual(result, newForm);
        }

        /// <summary>
        /// Tests normalizer null input
        /// </summary>
        /// 
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void toStringTest5()
        {
            Normalizer norm = s => "x3";

            Formula f = new Formula("(((y / x) * z) * 300) + 365 * 10 / w + 1000", null, s => true);

            string newForm = "(((x3/x3)*x3)*300)+365*10/x3+1000";

            string result = f.ToString();

            Assert.AreEqual(result, newForm);
        }

        /// <summary>
        /// Tests validator null input
        /// </summary>
        /// 
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void toStringTest6()
        {
            Normalizer norm = s => "x3";

            Formula f = new Formula("(((y / x) * z) * 300) + 365 * 10 / w + 1000", norm, null);

            string newForm = "(((x3/x3)*x3)*300)+365*10/x3+1000";

            string result = f.ToString();

            Assert.AreEqual(result, newForm);
        }

        /// <summary>
        /// Tests normalizer null input
        /// </summary>
        /// 
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void toStringTest7()
        {
            Normalizer norm = s => null;

            Formula f = new Formula("(((y / x) * z) * 300) + 365 * 10 / w + 1000", norm, s => true);

            string newForm = "(((x3/x3)*x3)*300)+365*10/x3+1000";

            string result = f.ToString();

            Assert.AreEqual(result, newForm);
        }

        /// <summary>
        /// Tests the zero argument constructor
        /// </summary>
        /// 
        [TestMethod]
        public void zeroArgumentConstructorTest1()
        {
            Formula f = new Formula();

            Assert.AreEqual(0, f.Evaluate(s => 10000));
        }

        /// <summary>
        /// Tests the zero argument constructor
        /// </summary>
        /// 
        [TestMethod]
        public void zeroArgumentConstructorTest2()
        {
            Formula f = new Formula();

            Assert.AreEqual(0, f.Evaluate(s => 0));
        }

        /// <summary>
        /// Two formulas that should behave the same
        /// </summary>
        [TestMethod]
        public void nestedFormulaConstructing()
        {
            Formula f1 = new Formula("(((y / x) * z) * 300) + 365 * 10 / w + 1000", s => s, s => true);

            Formula f2 = new Formula(f1.ToString(), s => s, s => true);

            Assert.AreEqual(f1.Evaluate(s => 1), f2.Evaluate(s => 1));
        }

        [TestMethod]
        public void getVariablesTest1()
        {
            Formula f1 = new Formula("(((y / x) * z) * 300) + 365 * 10 / w + 1000", s => s, s => true);

            List<String> vars = new List<string>();

            vars.Add("x");
            vars.Add("w");
            vars.Add("z");
            vars.Add("y");

            foreach(string var in f1.GetVariables())
            {
                Assert.IsTrue(vars.Contains(var));
            }

            Formula f2 = new Formula("(((y / x) * z) * 300) + 365 * 10 / w + 1000", s => "P12", s => true);

            vars.Clear();

            vars.Add("P12");

            foreach(string var in f2.GetVariables())
            {
                Assert.IsTrue(vars.Contains(var));
            }
        }

        /// <summary>
        /// A Lookup method that maps x to 4.0, y to 6.0, and z to 8.0.
        /// All other variables result in an UndefinedVariableException.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Lookup4(String v)
        {
            switch (v)
            {
                case "a": return -1000;
                case "w": return 0;
                case "x": return 4.0;
                case "y": return 6.0;
                case "z": return 8.0;
                default: throw new UndefinedVariableException(v);
            }
        }
    }
}
