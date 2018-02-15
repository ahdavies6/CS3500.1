// Source code written by:
// Adam Davies
// CS 3500-001
// Spring Semester, 2018

using System;
using Formulas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormulaTests
{
    [TestClass]
    class ConstructorTests
    {
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
