using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;

namespace FormulaTest
{
    [TestClass]
    public class ConstructorTests
    {
        string pOpen = @"\(";
        string pClose = @"\)";
        string pOperator = @"[\+\-*/]";
        string pVariable = @"[a-zA-Z][0-9a-zA-Z]*";
        string pNumber = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";

        [TestMethod]
        public void MatchThis() //num, var, open
        {
            Assert.IsTrue(Formula.MatchThese("42", pNumber));
            Assert.IsTrue(Formula.MatchThese("4324 789342 7894323", pNumber));

            Assert.IsTrue(Formula.MatchThese("var", pVariable));
            Assert.IsTrue(Formula.MatchThese("adam is a person", pVariable));
            Assert.IsTrue(Formula.MatchThese("var", pVariable));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Syntax1()
        {
            Formula f = new Formula("!");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Syntax2()
        {
            Formula f = new Formula("4 + 5_ * (6) * 7");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Syntax3()
        {
            Formula f = new Formula("4x + ($y * 2)");
        }

        [TestMethod]
        public void Syntax4()
        {
            Formula f = new Formula("4(x) + (y * 2)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ExtraClose()
        {
            Formula f = new Formula("4(x + y) + 7)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void AfterOpen()
        {
            Formula f = new Formula("3 + (-4x)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void AfterVar()
        {
            Formula f = new Formula("4 + (a b)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void UnequalPs()
        {
            Formula f = new Formula("(3 + 4) + (a + (b + c)");
        }

        [TestMethod]
        public void EqualPs()
        {
            Formula f = new Formula("(3 + 4) + (a + (b + c))");
        }
    }

    [TestClass]
    class EvaluateTests
    {

    }

    [TestClass]
    class LookupTests
    {

    }
}
