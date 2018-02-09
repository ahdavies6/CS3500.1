using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using System.Linq;
using System.Collections.Generic;

namespace FormulaTest
{
    [TestClass]
    public class FormulaTests
    {
        // Tests of syntax errors detected by the constructor
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test1()
        {
            Formula f = new Formula("        ");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test2()
        {
            Formula f = new Formula("((2 + 5))) + 8");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test3()
        {
            Formula f = new Formula("2+5*8)");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test4()
        {
            Formula f = new Formula("((3+5*7)");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test5()
        {
            Formula f = new Formula("+3");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test6()
        {
            Formula f = new Formula("-y");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test7()
        {
            Formula f = new Formula("*7");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test8()
        {
            Formula f = new Formula("/z2x");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test9()
        {
            Formula f = new Formula(")");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test10()
        {
            Formula f = new Formula("(*5)");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test11()
        {
            Formula f = new Formula("2 5");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test12()
        {
            Formula f = new Formula("x5 y");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test13()
        {
            Formula f = new Formula("((((((((((2)))))))))");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test14()
        {
            Formula f = new Formula("$");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test15()
        {
            Formula f = new Formula("x5 + x6 + x7 + (x8) +");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test15a()
        {
            Formula f = new Formula("x1 ++ y1");
        }

        // Simple tests that throw FormulaEvaluationExceptions
        [TestMethod()]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Test16()
        {
            Formula f = new Formula("2+x");
            f.Evaluate(s => { throw new UndefinedVariableException(s); });
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Test17()
        {
            Formula f = new Formula("5/0");
            f.Evaluate(s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Test18()
        {
            Formula f = new Formula("(5 + x) / (y - 3)");
            f.Evaluate(s => 3);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Test18a()
        {
            Formula f = new Formula("(5 + x) / (3 * 2 - 12 / 2)");
            f.Evaluate(s => 3);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Test19()
        {
            Formula f = new Formula("x + y");
            f.Evaluate(s => { if (s == "x") return 0; else throw new UndefinedVariableException(s); });
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Test20()
        {
            Formula f = new Formula("x1 + x2 * x3 + x4 * x5 * x6 + x7");
            f.Evaluate(s => { if (s == "x7") throw new UndefinedVariableException(s); else return 1; });
        }

        // Simple formulas
        [TestMethod()]
        public void Test21()
        {
            Formula f = new Formula("4.5e1");
            Assert.AreEqual(45, f.Evaluate(s => 0), 1e-6);
        }

        [TestMethod()]
        public void Test21a()
        {
            Formula f = new Formula("4");
            Assert.AreEqual(4, f.Evaluate(s => 0), 1e-6);
        }

        [TestMethod()]
        public void Test22()
        {
            Formula f = new Formula("a05");
            Assert.AreEqual(10, f.Evaluate(s => 10), 1e-6);
        }

        [TestMethod()]
        public void Test22a()
        {
            Formula f = new Formula("a1b2c3d4e5f6g7h8i9j10");
            Assert.AreEqual(10, f.Evaluate(s => 10), 1e-6);
        }

        [TestMethod()]
        public void Test23()
        {
            Formula f = new Formula("5 + x");
            Assert.AreEqual(9, f.Evaluate(s => 4), 1e-6);
        }

        [TestMethod()]
        public void Test24()
        {
            Formula f = new Formula("5 - y");
            Assert.AreEqual(1, f.Evaluate(s => 4), 1e-6);
        }

        [TestMethod()]
        public void Test25()
        {
            Formula f = new Formula("5 * z");
            Assert.AreEqual(20, f.Evaluate(s => 4), 1e-6);
        }

        [TestMethod()]
        public void Test26()
        {
            Formula f = new Formula("8 / xx");
            Assert.AreEqual(2, f.Evaluate(s => 4), 1e-6);
        }

        [TestMethod()]
        public void Test27()
        {
            Formula f = new Formula("(5 + 4) * 2");
            Assert.AreEqual(18, f.Evaluate(s => 0), 1e-6);
        }

        [TestMethod()]
        public void Test28()
        {
            Formula f = new Formula("1 + 2 + 3 * 4 + 5");
            Assert.AreEqual(20, f.Evaluate(s => 0), 1e-6);
        }

        [TestMethod()]
        public void Test29()
        {
            Formula f = new Formula("(1 + 2 + 3 * 4 + 5) * 2");
            Assert.AreEqual(40, f.Evaluate(s => 0), 1e-6);
        }

        [TestMethod()]
        public void Test30()
        {
            Formula f = new Formula("((((((((((((3))))))))))))");
            Assert.AreEqual(3, f.Evaluate(s => 0), 1e-6);
        }

        [TestMethod()]
        public void Test31()
        {
            Formula f = new Formula("((((((((((((x))))))))))))");
            Assert.AreEqual(7, f.Evaluate(s => 7), 1e-6);
        }

        // Some more complicated formula evaluations
        [TestMethod()]
        public void Test32()
        {
            Formula f = new Formula("y*3-8/2+4*(8-9*2)/14*x");
            Assert.AreEqual(5.14285714285714, (double)f.Evaluate(s => (s == "x") ? 1 : 4), 1e-9);
        }

        [TestMethod()]
        public void Test32a()
        {
            Formula f = new Formula("a + b * c - d + 3 * 3.0 - 3.0e0 / 0.003e3");
            Assert.AreEqual(17, (double)f.Evaluate(s => 3), 1e-9);
        }

        [TestMethod()]
        public void Test33()
        {
            Formula f = new Formula("a+(b+(c+(d+(e+f))))");
            Assert.AreEqual(6, (double)f.Evaluate(s => 1), 1e-9);
        }

        [TestMethod()]
        public void Test34()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)f.Evaluate(s => 2), 1e-9);
        }

        [TestMethod()]
        public void Test35()
        {
            Formula f = new Formula("a-a*a/a");
            Assert.AreEqual(0, (double)f.Evaluate(s => 3), 1e-9);
        }

        // Tests to make sure there can be more than one formula at a time
        [TestMethod()]
        public void Test36()
        {
            Formula f1 = new Formula("xx+3");
            Formula f2 = new Formula("xx-3");
            Assert.AreEqual(6, f1.Evaluate(s => 3), 1e-6);
            Assert.AreEqual(0, f2.Evaluate(s => 3), 1e-6);
        }

        [TestMethod()]
        public void Test37()
        {
            Test36();
        }

        [TestMethod()]
        public void Test38()
        {
            Test36();
        }

        [TestMethod()]
        public void Test39()
        {
            Test36();
        }

        [TestMethod()]
        public void Test40()
        {
            Test36();
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test41()
        {
            Formula f = new Formula("(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)");
        }

        // Stress test for constructor, repeated five times to give it extra weight.
        [TestMethod()]
        public void Test42()
        {
            Test41();
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test43()
        {
            Test41();
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test44()
        {
            Test41();
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test45()
        {
            Test41();
        }

        #region PS4a Tests
        
        // Test for the null constructor.
        [TestMethod]
        public void NullTest()
        {
            Formula f = new Formula();
            Assert.AreEqual(0, f.Evaluate(s => 0));
            Assert.AreEqual(0, f.Evaluate(s => 6e10));
            Assert.AreNotEqual(1, f.Evaluate(s => 0));
            Assert.AreNotEqual(1, f.Evaluate(s => 4e-4));
        }

        // Tests for the overloaded constructor (with params formula, normalizer, and validator)

        [TestMethod]
        public void OverloadTest1()
        {
            Formula f = new Formula("0", (s => s), (s => true));
            Assert.AreEqual(0, f.Evaluate(s => 0));
            Assert.AreEqual(0, f.Evaluate(s => 6e10));
            Assert.AreNotEqual(1, f.Evaluate(s => 0));
            Assert.AreNotEqual(1, f.Evaluate(s => 4e-4));
        }

        [TestMethod]
        public void OverloadTest2()
        {
            Formula f = new Formula("6 * x / (4 - 1 * 2)", (s => s), (s => true));
            Assert.AreEqual(42, f.Evaluate(s => 14));
        }

        [TestMethod]
        public void OverloadTest3()
        {
            Formula f = new Formula("x * x - 1", (s => s + s), (s => s.Length < 3));
            Assert.AreEqual(3, f.Evaluate(s => s.Length));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OverloadTest4()
        {
            Formula f = new Formula("apple", (s => s), HasUpper);
        }

        [TestMethod]
        public void OverloadTest5()
        {
            Formula f = new Formula("banana", (s => s.ToUpper()), HasUpper);
            Assert.AreEqual(3, f.Evaluate(NumAs));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OverloadTest6()
        {
            Formula f = new Formula("xa * xa - 1", (s => s + s), (s => s.Length < 3));
        }

        // Tests for Formula.GetVariables()

        [TestMethod]
        public void GVTest1()
        {
            Formula f = new Formula();
            ISet<string> vars = f.GetVariables();
            Assert.AreEqual(0, vars.Count);
        }

        [TestMethod]
        public void GVTest2()
        {
            Formula f = new Formula("random + (x2 - whocares) * a8098 / random", (s => s + "eh"), (s => true));
            string[] expected = { "randomeh", "x2eh", "whocareseh", "a8098eh" };
            ISet<string> vars = f.GetVariables();
            Assert.AreEqual(4, vars.Count);
            
            for (int i = 0; i < vars.Count; i++)
            {
                Assert.IsTrue(vars.Contains(expected[i]));
            }
        }

        // Tests for Formula.ToString()

        [TestMethod]
        public void TSTest1()
        {
            Formula f = new Formula();
            Assert.AreEqual("0", f.ToString());
        }

        [TestMethod]
        public void TSTest2()
        {
            Formula f1 = new Formula("((adam - banana * onea) + none) / (14 * (none + 1))");
            Formula f2 = new Formula(f1.ToString(), s => s, s => true);

            AssertEquality(f1, f2);
        }

        // Tests to ensure null arguments throw ArgumentNullException

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTest1()
        {
            Formula f = new Formula(null, (s => s), (s => true));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTest2()
        {
            Formula f = new Formula("otherwise - fine", null, (s => true));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTest3()
        {
            Formula f = new Formula("should + be * okay", (s => s), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTest4()
        {
            Formula f = new Formula("4");
            f.Evaluate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTest5()
        {
            Formula f = new Formula(null);
        }

        // Tests ensure exceptions at division by zero

        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void DivByZeroTest1()
        {
            Formula f = new Formula("14/x");
            f.Evaluate(s => 0);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void DivByZeroTest2()
        {
            Formula f = new Formula("(x + (5 * (x + 1))) / (x - 2)", (s => s + s), (s => true));
            f.Evaluate(s => s.Length);
        }

        // Test ensures all methods treat Formula() the same as Formula("0")
        [TestMethod]
        public void ZeroArgumentTest()
        {
            Formula f1 = new Formula();
            Formula f2 = new Formula();

            AssertEquality(f1, f2);
        }

        // Stress tests

        [TestMethod]
        public void StressTest()
        {
            Formula f1 = new Formula("(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)",
                (s => s + "UPPER"), HasUpper);
            Formula f2 = new Formula(f1.ToString(), (s => s), (s => s.Any(c => char.IsUpper(c))));

            AssertEquality(f1, f2);
        }

        [TestMethod]
        public void RepeatStress1()
        {
            StressTest();
        }

        [TestMethod]
        public void RepeatStress2()
        {
            StressTest();
        }

        [TestMethod]
        public void RepeatStress3()
        {
            StressTest();
        }

        #endregion

        // Returns whether s has at least one capitalized char.
        public bool HasUpper(string s)
        {
            return s.Any(c => char.IsUpper(c));
        }

        // Return number of 'a's in s
        public double NumAs(string s)
        {
            int n = 0;
            foreach (char c in s)
            {
                if (Char.ToLower(c) == 'a')
                {
                    n++;
                }
            }
            return n;
        }

        // Make sure both formulas return identical results for all public methods
        public bool AssertEquality(Formula f1, Formula f2)
        {
            Assert.AreEqual(f1.Evaluate(s => 4), f2.Evaluate(s => 4));
            Assert.AreEqual(f1.Evaluate(NumAs), f2.Evaluate(NumAs));

            var f1v = f1.GetVariables().ToList();
            var f2v = f2.GetVariables().ToList();
            CollectionAssert.AreEqual(f1v, f2v);

            Assert.AreEqual(f1.ToString(), f2.ToString());

            return true;
        }
    }
}