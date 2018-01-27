using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;

namespace FormulaTest
{
    [TestClass]
    public class FormulaTests
    {
        //string pOpen = @"\(";
        //string pClose = @"\)";
        //string pOperator = @"[\+\-*/]";
        //string pVariable = @"[a-zA-Z][0-9a-zA-Z]*";
        //string pNumber = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";

        //[TestMethod]
        //public void MatchTest() //num, var, open
        //{
        //    Formula f = new Formula("0");
        //    PrivateObject fTest = new PrivateObject(f);
        //    object[] parameters = { "(", pNumber, pVariable, pOpen };

        //    Assert.IsTrue((bool)fTest.Invoke("MatchThese", parameters));
        //}

        // Constructor tests

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

        // Evaluate and Lookup tests

        [TestMethod]
        public void EvalTest1()
        {
            Formula f = new Formula("6");
            Assert.AreEqual(f.Evaluate(Return5), 6);
        }

        [TestMethod]
        public void EvalTest2()
        {
            Formula f = new Formula("x");
            Assert.AreEqual(f.Evaluate(Return5), 5);
        }

        [TestMethod]
        public void EvalTest3()
        {
            Formula f = new Formula("14 + x");
            Assert.AreEqual(f.Evaluate(Return5), 19);
        }

        [TestMethod]
        public void EvalTest4()
        {
            Formula f = new Formula("(14 * s) / 7");
            Assert.AreEqual(f.Evaluate(Return5), 10);
        }

        [TestMethod]
        public void EvalTest5()
        {
            Formula f = new Formula("(200 - 10 / s) + 4");
            Assert.AreEqual(f.Evaluate(Return5), 42);
        }

        [TestMethod]
        public void EvalFinal()
        {
            Formula f = new Formula("(s * 4) + (10/(2+3)) - ((99 - 100) + 3) * (7      -  4) / 4");
            Assert.AreEqual(f.Evaluate(Return5), 15);
        }

        [TestMethod]
        public void LookupTestWild()
        {
            Formula f = new Formula("(five + woohoo) * (6 / (seven - four))");
            Assert.AreEqual(f.Evaluate(Wild), 22);
        }

        private double Return5(string s)
        {
            return 5;
        }

        private double Wild(string s)
        {
            if (s == "five")
            {
                return 5;
            }
            else if (s == "seven")
            {
                return 7;
            }
            else
            {
                return s.Length;
            }
        }
    }
}
