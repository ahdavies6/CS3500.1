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
        public void MatchTest() //num, var, open
        {
            Formula f = new Formula("0");
            PrivateObject fTest = new PrivateObject(f);
            object[] parameters = { "(", pNumber, pVariable, pOpen };

            Assert.IsTrue((bool)fTest.Invoke("MatchThese", parameters));
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
            Formula f = new Formula("4 * x + (y * 2)");
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

        [TestMethod]
        public void Begin()
        {
            Formula f = new Formula("a + 5");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void End()
        {
            Formula f = new Formula("5 + a +");
        }

        [TestMethod]
        public void ProvidedValid()
        {
            Formula f = new Formula("2.5e9 + x5 / 17");
            Formula g = new Formula("(5 * 2) + 8");
            Formula h = new Formula("x*y-2+35/9");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ProvidedInvalid1()
        {
            Formula f = new Formula("_");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ProvidedInvalid2()
        {
            Formula f = new Formula("-5.3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ProvidedInvalid3()
        {
            Formula f = new Formula("2 5 + 3");
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
