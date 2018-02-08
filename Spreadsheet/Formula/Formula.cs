// Skeleton written by Joe Zachary for CS 3500, January 2017

// Formula constructor and Formula.Evaluate completed by:
// Adam Davies
// CS 3500-001
// Spring Semester, 2018

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        /// <summary>
        /// Instance variable infix, initialized in Formula's constructor, holds a standard infix
        /// expression derived from valid input tokens.
        /// </summary>
        private List<string> equation;

        // Several Regex patterns, to be used throughout the class
        private const string pFull = @"^\($|^\)$|^[\+\-*/]$|^[a-zA-Z][0-9a-zA-Z]*$|^(?:\d+\.\d*|\d*\.\d+|\d+)(?:e[\+-]?\d+)?$";
        private const string pOpen = @"\(";
        private const string pClose = @"\)";
        private const string pOperator = @"[\+\-*/]";
        private const string pVariable = @"[a-zA-Z][0-9a-zA-Z]*";
        private const string pNumber = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";

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
            int openParentheses = 0;
            int closeParentheses = 0;

            equation = new List<string>();

            foreach (string token in GetTokens(formula))
            {
                equation.Add(token);
            }

            for (int i = 0; i < equation.Count; i++)
            {
                // All tokens must be syntactically valid
                if (Regex.IsMatch(equation[i], pFull, RegexOptions.IgnorePatternWhitespace))
                {
                    if (MatchThese(equation[i], pOpen))
                    {
                        openParentheses++;
                    }

                    // There must be no more closing parenthesis than opening parenthesis
                    // (while reading left to right)
                    if (MatchThese(equation[i], pClose))
                    {
                        closeParentheses++;

                        if (closeParentheses > openParentheses)
                        {
                            throw new FormulaFormatException("There must be no more closing parenthesis " +
                                "than opening parenthesis, reading left to right.");
                        }
                    }
                    if (i < equation.Count - 1)
                        {
                        // Any token immediately following an opening parenthesis or operator must be
                        // a number, variable, or opening parenthesis
                        if (MatchThese(equation[i], pOpen, pOperator))
                        {
                            if (!MatchThese(equation[i + 1], pNumber, pVariable, pOpen))
                            {
                                throw new FormulaFormatException(
                                    "Any token immediately following an opening parenthesis or operator " +
                                    "a number, variable, or opening parenthesis.");
                            }
                        }
                        // Any token immediately following a number, variable, or closing parenthesis
                        // must be an operator or closing parenthesis
                        if (MatchThese(equation[i], pNumber, pVariable, pClose))
                        {
                            if (!MatchThese(equation[i + 1], pOperator, pClose))
                            {
                                throw new FormulaFormatException(
                                    "Any token immediately following a number, variable, or closing parenthesis " +
                                    "must be an operator or closing parenthesis.");
                            }
                        }
                    }
                }
                else
                {
                    throw new FormulaFormatException("Invalid token: " + equation[i]);
                }
            }
            // There must be an equal number of opening and closing parenthesis
            // Note: the inequality (closeParenthesis > openParenthesis) has already been checked for
            // each instance of a closing parenthesis.
            if (openParentheses > closeParentheses)
            {
                throw new FormulaFormatException("Not all parenthesis have been closed.");
            }
            // Cannot test beginning/end token with empty equation
            if (equation.Count > 0)
            {
                // The first token of a formula must be a number, variable, or opening parenthesis
                if (!MatchThese(equation[0], pNumber, pVariable, pOpen))
                {
                    throw new FormulaFormatException("Formula must begin with a number, variable, or opening parenthesis.");
                }
                // The last token of a formula must be a number, variable, or closing parenthesis
                if (!MatchThese(equation[equation.Count - 1], pNumber, pVariable, pClose))
                {
                    throw new FormulaFormatException("Formula must end with a number, variable, or closing parenthesis.");
                }
            }
            // There must be at least one token
            else
            {
                throw new FormulaFormatException("Formula must have at least one valid token.");
            }
        }

        /// <summary>
        /// A Lookup method is one that maps some strings to double values.  Given a string,
        /// such a function can either return a double (meaning that the string maps to the
        /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
        /// to a value). Exactly how a Lookup method decides which strings map to doubles and which
        /// don't is up to the implementation of the method.
        /// </summary>
        public delegate double Lookup(string var);

        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            Stack<double> vStack = new Stack<double>(); // value stack
            Stack<string> oStack = new Stack<string>(); // operator stack

            foreach (string token in equation)
            {
                if (MatchThese(token, pVariable))
                {
                    {
                        double val;

                        // Checks whether lookup returned a value
                        try
                        {
                            val = lookup(token);
                        }
                        catch (UndefinedVariableException e)
                        {
                            throw new FormulaEvaluationException(e.Source + " threw an " +
                                "UndefinedVariableException.");
                        }

                        if (oStack.Count > 0)
                        {
                            if (oStack.Peek() == "*")
                            {
                                oStack.Pop();
                                double multBy = vStack.Pop();
                                vStack.Push((val * multBy));
                            }
                            else if (oStack.Peek() == "/")
                            {
                                if (val != 0)
                                {
                                    oStack.Pop();
                                    double numerator = vStack.Pop();
                                    vStack.Push((numerator / val));
                                }
                                else
                                {
                                    throw new FormulaEvaluationException("Cannot divide by zero.");
                                }
                            }
                            else
                            {
                                vStack.Push(val);
                            }
                        }
                        else
                        {
                            vStack.Push(val);
                        }
                    }
                }
                else if (MatchThese(token, pNumber))
                {
                    if (!Double.TryParse(token, out double val))
                    {
                        throw new FormulaEvaluationException("Invalid token: " + token);
                    }

                    if (oStack.Count > 0)
                    {
                        if (oStack.Peek() == "*")
                        {
                            oStack.Pop();
                            double multBy = vStack.Pop();
                            vStack.Push((val * multBy));
                        }
                        else if (oStack.Peek() == "/")
                        {
                            if (val != 0)
                            {
                                oStack.Pop();
                                double numerator = vStack.Pop();
                                vStack.Push((numerator / val));
                            }
                            else
                            {
                                throw new FormulaEvaluationException("Cannot divide by zero.");
                            }
                        }
                        else
                        {
                            vStack.Push(val);
                        }
                    }
                    else
                    {
                        vStack.Push(val);
                    }
                }
                else if (MatchThese(token, pOperator))
                {
                    if (oStack.Count > 0)
                    {
                        if (token == "+" || token == "-")
                        {
                            if (oStack.Peek() == "+")
                            {
                                double v1 = vStack.Pop();
                                double v2 = vStack.Pop();
                                oStack.Pop();
                                vStack.Push(v1 + v2);
                            }
                            else if (oStack.Peek() == "-")
                            {
                                double subThis = vStack.Pop();
                                double subFrom = vStack.Pop();
                                oStack.Pop();
                                vStack.Push(subFrom - subThis);
                            }
                        }
                    }
                    // This is all that's done for division and multiplication, and is
                    // also done after the previous operations for addition and subtraction.
                    oStack.Push(token);
                }
                else if (MatchThese(token, pOpen))
                {
                    oStack.Push(token);
                }
                else if (MatchThese(token, pClose))
                {
                    if (oStack.Count > 0)
                    {
                        if (oStack.Peek() == "+")
                        {
                            double v1 = vStack.Pop();
                            double v2 = vStack.Pop();
                            oStack.Pop();
                            vStack.Push(v1 + v2);
                        }
                        else if (oStack.Peek() == "-")
                        {
                            double subThis = vStack.Pop();
                            double subFrom = vStack.Pop();
                            oStack.Pop();
                            vStack.Push(subFrom - subThis);
                        }
                    }
                    // This should be done before division or multiplication, and is
                    // also all that's left to do after the previous operations for addition
                    // and subtraction.
                    oStack.Pop();

                    if (oStack.Count > 0)
                    {
                        if (oStack.Peek() == "*")
                        {
                            double v1 = vStack.Pop();
                            double v2 = vStack.Pop();
                            oStack.Pop();
                            vStack.Push(v1 * v2);
                        }
                        else if (oStack.Peek() == "/")
                        {
                            double denominator = vStack.Pop();
                            if (denominator != 0)
                            {
                                double numerator = vStack.Pop();
                                oStack.Pop();
                                vStack.Push(numerator / denominator);
                            }
                            else
                            {
                                throw new FormulaEvaluationException("Cannot divide by zero.");
                            }
                        } 
                    }
                }
            }
            if (oStack.Count == 0)
            {
                return vStack.Pop();
            }
            else 
            {
                if (oStack.Peek() == "+")
                {
                    double v1 = vStack.Pop();
                    double v2 = vStack.Pop();
                    oStack.Pop();
                    return v1 + v2;
                }
                else // oStack contains only one "-"
                {
                    double subThis = vStack.Pop();
                    double subFrom = vStack.Pop();
                    oStack.Pop();
                    return subFrom - subThis;
                }
            }
        }

        /// <summary>
        /// Checks whether the string input matches at least one of multiple Regular Expression patterns.
        /// If input matches at least one pattern, returns true; otherwise, returns false.
        /// </summary>
        private bool MatchThese(string input, params string[] patterns)
        {
            foreach (string pattern in patterns)
            {
                if (Regex.IsMatch(input, "^" + pattern + "$", RegexOptions.IgnorePatternWhitespace))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens.
            // NOTE:  These patterns are designed to be used to create a pattern to split a string into tokens.
            // For example, the opPattern will match any string that contains an operator symbol, such as
            // "abc+def".  If you want to use one of these patterns to match an entire string (e.g., make it so
            // the opPattern will match "+" but not "abc+def", you need to add ^ to the beginning of the pattern
            // and $ to the end (e.g., opPattern would need to be @"^[\+\-*/]$".)
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
            // it is used.  See below for an example of this.  This pattern is useful for 
            // splitting a string into tokens.
            String splittingPattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            // PLEASE NOTE:  Notice the second parameter to Split, which says to ignore embedded white space
            /// in the pattern.
            foreach (String s in Regex.Split(formula, splittingPattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

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
