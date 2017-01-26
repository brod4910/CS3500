// Skeleton written by Joe Zachary for CS 3500, January 2017
// Brian Rodriguez u0853593

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public class Formula
    {
        private IEnumerable<String> formula;

        private const String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
        private const String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
        private const String opPattern = @"[\+\-*/]";
        private const String rpPattern = @"\)";
        private const String lpPattern = @"\(";


        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        public Formula(String formula)
        {
            int rparen = 0;
            int lparen = 0;
            bool areTokens = false;
            bool isfirstToken = true;
            IEnumerable<String> tokens = GetTokens(formula);
            int index = 1;
            int size = tokens.Count();
            string prevToken = null;

            foreach (string token in tokens)
            {
                areTokens = true;

                if (Regex.IsMatch(token, lpPattern) || Regex.IsMatch(token, varPattern) || Regex.IsMatch(token, doublePattern, RegexOptions.IgnorePatternWhitespace) || Regex.IsMatch(token, rpPattern) || Regex.IsMatch(token, opPattern))
                {
                    if (isfirstToken)
                    {
                        isfirstToken = false;
                        if (Regex.IsMatch(token, lpPattern) || Regex.IsMatch(token, varPattern) || Regex.IsMatch(token, doublePattern, RegexOptions.IgnorePatternWhitespace))
                        {
                            if (Regex.IsMatch(token, lpPattern))
                            {
                                lparen++;
                            }
                            else if(!Regex.IsMatch(token, varPattern))
                            {
                                if(Convert.ToDouble(token) < 0)
                                {
                                    throw new FormulaFormatException("Only non-negative numbers are permitted");
                                }
                            }
                            prevToken = token;
                            index++;
                        }
                        else
                        {
                            throw new FormulaFormatException("The first token of a formula must be a number, a variable, or an opening parenthesis.");
                        }
                    }
                    else
                    {
                        if (Regex.IsMatch(prevToken, lpPattern) || Regex.IsMatch(prevToken, opPattern))
                        {
                            if (Regex.IsMatch(token, rpPattern) || Regex.IsMatch(token, opPattern))
                            {
                                throw new FormulaFormatException("Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.");
                            }
                        }
                        else
                        {
                            if (Regex.IsMatch(token, lpPattern) || Regex.IsMatch(token, varPattern) || Regex.IsMatch(token, doublePattern, RegexOptions.IgnorePatternWhitespace))
                            {
                                throw new FormulaFormatException("Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.");
                            }
                        }

                        if (index == size)
                        {
                            if (Regex.IsMatch(token, opPattern) || Regex.IsMatch(token, lpPattern))
                            {
                                throw new FormulaFormatException("The last token of a formula must be a number, a variable, or a closing parenthesis.");
                            }
                        }

                        if(Regex.IsMatch(token, doublePattern, RegexOptions.IgnorePatternWhitespace))
                        {
                            if (Convert.ToDouble(token) < 0)
                            {
                                throw new FormulaFormatException("Only non-negative numbers are permitted");
                            }
                        }

                        if (Regex.IsMatch(token, lpPattern))
                        {
                            lparen++;
                        }
                        else if (Regex.IsMatch(token, rpPattern))
                        {
                            rparen++;
                        }

                        if (rparen > lparen)
                        {
                            throw new FormulaFormatException("Number of closing parentheses is greater than opening parentheses.");
                        }

                        prevToken = token;
                        index++;
                    }
                }
                else
                {
                    throw new FormulaFormatException("Invalid token in Formula.");
                }
            }

            if (lparen != rparen)
            {
                throw new FormulaFormatException("Number of parentheses are not equal.");
            }


            if (areTokens == false)
            {
                throw new FormulaFormatException("No tokens have been entered.");
            }

            this.formula = tokens;
        }

        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            string oper;
            double value1;
            double value2;

            Stack<String> opStack = new Stack<String>();

            Stack<Double> valueStack = new Stack<Double>();

            foreach(string token in formula)
            {
                if(Regex.IsMatch(token, opPattern))
                {
                    if(opStack.Count != 0)
                    {

                        if (opStack.Peek().Equals("+") || opStack.Peek().Equals("-"))
                        {
                            oper = opStack.Pop();
                            value2 = valueStack.Pop();
                            value1 = valueStack.Pop();

                            if (oper.Equals("+"))
                            {
                                valueStack.Push(value1 + value2);
                            }
                            else
                            {
                                valueStack.Push(value1 - value2);
                            }
                            opStack.Push(token);
                        }
                        else
                        {
                            opStack.Push(token);
                        }
                    }
                    else
                    {
                        opStack.Push(token);
                    }
                }
                else if(Regex.IsMatch(token, varPattern))
                {
                    if (opStack.Count != 0)
                    {
                        if (opStack.Peek().Equals("*") || opStack.Peek().Equals("/"))
                        {
                            oper = opStack.Pop();
                            value1 = valueStack.Pop();

                            try
                            {
                                value2 = lookup(token);
                            }
                            catch (UndefinedVariableException)
                            {
                                throw new FormulaEvaluationException("Variable is undefined.");
                            }

                            if (value2 < 0)
                            {
                                throw new FormulaEvaluationException("A variable must be a non-negative number.");
                            }

                            if (oper.Equals("*"))
                            {
                                valueStack.Push(value1 * value2);
                            }
                            else
                            {
                                if (value2 == 0)
                                {
                                    throw new FormulaEvaluationException("Division by zero has occured.");
                                }
                                else
                                {
                                    valueStack.Push(value1 / value2);
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                lookup(token);
                            }
                            catch (UndefinedVariableException)
                            {
                                throw new FormulaEvaluationException("Variable is undefined.");
                            }

                            if (lookup(token) < 0)
                            {
                                throw new FormulaEvaluationException("A variable must be a non-negative number.");
                            }
                            else
                            {
                                valueStack.Push(lookup(token));
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            lookup(token);
                        }
                        catch (UndefinedVariableException)
                        {
                            throw new FormulaEvaluationException("Variable is undefined.");
                        }

                        if (lookup(token) < 0)
                        {
                            throw new FormulaEvaluationException("A variable must be a non-negative number.");
                        }
                        else
                        {
                            valueStack.Push(lookup(token));
                        }
                    }
                }
                else if(Regex.IsMatch(token, lpPattern))
                {
                    opStack.Push(token);
                }
                else if(Regex.IsMatch(token, rpPattern))
                {
                    if (opStack.Count != 0)
                    {
                        if (opStack.Peek().Equals("+") || opStack.Peek().Equals("-"))
                        {
                            oper = opStack.Pop();
                            value2 = valueStack.Pop();
                            value1 = valueStack.Pop();

                            if (oper.Equals("+"))
                            {
                                valueStack.Push(value1 + value2);
                                opStack.Pop();
                            }
                            else
                            {
                                valueStack.Push(value1 - value2);
                                opStack.Pop();
                            }
                        }
                        else
                        {
                            opStack.Pop();
                        }
                    }
                    else
                    {
                        opStack.Pop();
                    }

                    if (opStack.Count != 0)
                    {
                        if (opStack.Peek().Equals("*") || opStack.Peek().Equals("/"))
                        {
                            oper = opStack.Pop();
                            value2 = valueStack.Pop();
                            value1 = valueStack.Pop();

                            if (oper.Equals("*"))
                            {
                                valueStack.Push(value1 * value2);
                            }
                            else
                            {
                                if (value2 == 0)
                                {
                                    throw new FormulaEvaluationException("Division by zero has occured.");
                                }
                                else
                                {
                                    valueStack.Push(value1 / value2);
                                }
                            }
                        }
                    }
                }
                else if(Regex.IsMatch(token, doublePattern, RegexOptions.IgnorePatternWhitespace))
                {
                    if (opStack.Count != 0)
                    {
                        if (opStack.Peek().Equals("*") || opStack.Peek().Equals("/"))
                        {
                            oper = opStack.Pop();
                            value2 = Convert.ToDouble(token);
                            value1 = valueStack.Pop();

                            if (oper.Equals("*"))
                            {
                                valueStack.Push(value1 * value2);
                            }
                            else
                            {
                                if (value2 == 0)
                                {
                                    throw new FormulaEvaluationException("Division by zero has occured.");
                                }
                                else
                                {
                                    valueStack.Push(value1 / value2);
                                }
                            }
                        }
                        else
                        {
                            valueStack.Push(Convert.ToDouble(token));
                        }
                    }
                    else
                    {
                        valueStack.Push(Convert.ToDouble(token));
                    }
                }
            }

            if(opStack.Count == 0)
            {
                return valueStack.Pop();
            }
            else
            {
                oper = opStack.Pop();
                value2 = valueStack.Pop();
                value1 = valueStack.Pop();
                if(oper.Equals("+"))
                {
                    return value1 + value2;
                }
                else
                {
                    return value1 - value2;
                }
            }
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            // PLEASE NOTE:  I have added white space to this regex to make it more readable.
            // When the regex is used, it is necessary to include a parameter that says
            // embedded white space should be ignored.  See below for an example of this.
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern.  It contains embedded white space that must be ignored when
            // it is used.  See below for an example of this.
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            // PLEASE NOTE:  Notice the second parameter to Split, which says to ignore embedded white space
            /// in the pattern.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string var);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    [Serializable]
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    [Serializable]
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}
