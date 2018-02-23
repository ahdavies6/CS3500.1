using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Dependencies;
using Formulas;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SpreadsheetTests
{
    [TestClass]
    public class Tests
    {
        #region PS5

        #region GetNamesOfAllNonemptyCells Tests

        [TestMethod]
        public void NonemptyCells()
        {
            Spreadsheet ss = new Spreadsheet();
            HashSet<string> test = IEToHash(ss.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(0, test.Count);

            ss.SetContentsOfCell("A1", "0");
            test = IEToHash(ss.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(1, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "A1" }));

            ss.SetContentsOfCell("A1", "something different");
            test = IEToHash(ss.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(1, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "A1" }));

            ss.SetContentsOfCell("bJFIOEdfsa123489701".ToUpper(), "=A1 + 1");
            test = IEToHash(ss.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(2, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "A1", "bJFIOEdfsa123489701".ToUpper() }));
        }

        #endregion

        #region GetCellContents Tests

        [TestMethod]
        public void GCC()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("pNzlefEEKN942", "19");
            ss.SetContentsOfCell("zYLd419", "who knows what this will be?");
            string f = "=a1 + b20 / 7";
            ss.SetContentsOfCell("inFEWNnnk107", f);

            Assert.AreEqual((double)19, ss.GetCellContents("pNzlefEEKN942".ToUpper()));
            Assert.AreEqual("who knows what this will be?", ss.GetCellContents("zYLd419".ToUpper()));
            // todo: figure out why this one doesn't work without the .ToString()s
            Assert.AreEqual(NormalizeFormulaString(f), ss.GetCellContents("inFEWNnnk107".ToUpper()).ToString());
        }

        [TestMethod]
        public void GCCEmpty()
        {
            Spreadsheet ss = new Spreadsheet();

            for (int i = 1; i < 100; i++)
            {
                Assert.AreEqual("", ss.GetCellContents("A" + i.ToString()));
            }
        }

        #endregion

        #region IsValidCellName Tests

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidCellName1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("!A1", "2");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidCellName2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A", "text");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidCellName3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A0", "=");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidCellName4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1!", "2");
        }

        [TestMethod]
        public void ValidCellName1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "2");
        }

        [TestMethod]
        public void ValidCellName2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a10", "2");
        }

        [TestMethod]
        public void ValidCellName3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("iWanTTObeDonEWIThThiS666", "2");
        }

        [TestMethod]
        public void ValidCellName4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("ThisSSHOUldWoRk104329047", "2");
        }

        #endregion

        #region SetContentsOfCell (double, string) Tests

        [TestMethod]
        public void SCCD()
        {
            Spreadsheet ss = new Spreadsheet();
            HashSet<string> test = (HashSet<string>)ss.SetContentsOfCell("b60", "2");
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "b60".ToUpper() }));
            Assert.AreEqual((double)2, ss.GetCellContents("b60"));

            test = (HashSet<string>)ss.SetContentsOfCell("b60", "199");
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "b60".ToUpper() }));
            Assert.AreEqual((double)199, ss.GetCellContents("b60"));

            test = (HashSet<string>)ss.SetContentsOfCell("b60", "now I'm a string!");
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "b60".ToUpper() }));
            Assert.AreEqual("now I'm a string!", ss.GetCellContents("b60"));

            test = (HashSet<string>)ss.SetContentsOfCell("b60", "2");
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "b60".ToUpper() }));
            Assert.AreEqual((double)2, ss.GetCellContents("b60"));
        }

        [TestMethod]
        public void SCCS()
        {
            Spreadsheet ss = new Spreadsheet();
            HashSet<string> test = (HashSet<string>)ss.SetContentsOfCell("AdD1423", "IsString");
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "AdD1423".ToUpper() }));
            Assert.AreEqual("IsString", ss.GetCellContents("AdD1423"));

            test = (HashSet<string>)ss.SetContentsOfCell("AdD1423", "different one!");
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "AdD1423".ToUpper() }));
            Assert.AreEqual("different one!", ss.GetCellContents("AdD1423"));

            test = (HashSet<string>)ss.SetContentsOfCell("AdD1423", "42");
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "AdD1423".ToUpper() }));
            Assert.AreEqual((double)42, ss.GetCellContents("AdD1423"));

            test = (HashSet<string>)ss.SetContentsOfCell("AdD1423", "finally:");
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "AdD1423".ToUpper() }));
            Assert.AreEqual("finally:", ss.GetCellContents("AdD1423"));
        }

        #endregion

        #region SetContentsOfCell (formula) and GetDirectDependents Tests

        /// <summary>
        /// This helper method will set up a Spreadsheet with formula dependencies for other
        /// SCCF tests.
        /// 
        /// Note: this method only provides direct dependencies; indirect dependencies will be
        /// added on top of existing ones.
        /// </summary>
        public Spreadsheet SCCFStart()
        {
            Spreadsheet ss = new Spreadsheet();
            HashSet<string> test = (HashSet<string>)ss.SetContentsOfCell("Lib89", "a");
            Assert.AreEqual(1, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "Lib89".ToUpper() }));
            Assert.AreEqual("a", ss.GetCellContents("Lib89"));

            string f = "=a20 - b16 + (876 * b16) / 7";
            test = (HashSet<string>)ss.SetContentsOfCell("Lib89", f);
            Assert.AreEqual(NormalizeFormulaString(f), ss.GetCellContents("Lib89").ToString());
            Assert.AreEqual(1, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "Lib89".ToUpper() }));

            test = (HashSet<string>)ss.SetContentsOfCell("a20", "5");
            Assert.AreEqual(2, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "a20".ToUpper(), "Lib89".ToUpper() }));

            test = (HashSet<string>)ss.SetContentsOfCell("b16", "string boi");
            Assert.AreEqual(2, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "Lib89".ToUpper(), "b16".ToUpper() }));

            f = "=a20 + 1";
            test = (HashSet<string>)ss.SetContentsOfCell("b16", f);
            Assert.AreEqual(2, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "Lib89".ToUpper(), "b16".ToUpper() }));

            test = (HashSet<string>)ss.SetContentsOfCell("a20", "7");
            Assert.AreEqual(3, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "a20".ToUpper(), "Lib89".ToUpper(), "b16".ToUpper() }));

            return ss;
        }

        /// <summary>
        /// Tests only the material in SCCFStart. Makes sure at least direct formual dependencies
        /// are functioning as intended.
        /// </summary>
        [TestMethod]
        public void SCCFDirect()
        {
            SCCFStart();
        }

        [TestMethod]
        public void SCCFIndirect()
        {
            Spreadsheet ss = SCCFStart();
            string f = ss.GetCellContents("Lib89").ToString();
            Assert.AreEqual(NormalizeFormulaString("=a20 - b16 + (876 * b16) / 7"), f.ToString());
            HashSet<string> test = (HashSet<string>)ss.SetContentsOfCell("a20", "let's begin");
            Assert.AreEqual(3, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "a20".ToUpper(), "Lib89".ToUpper(), "b16".ToUpper() }));

            f = "=AD19 * 2";
            ss.SetContentsOfCell("b16", f);
            f = "=a20 + 1";
            ss.SetContentsOfCell("AD19", f);
            test = (HashSet<string>)ss.SetContentsOfCell("a20", "doesn't even matter");
            Assert.AreEqual(4, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "Lib89".ToUpper(), "b16".ToUpper(), "AD19".ToUpper(), "a20".ToUpper() }));

            f = "=b16 + 1";
            ss.SetContentsOfCell("Pd5", f);
            f = "=Pd5 - AD19";
            ss.SetContentsOfCell("jk101", f);
            test = (HashSet<string>)ss.SetContentsOfCell("a20", "doesn't even matter");
            Assert.AreEqual(6, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string>
            {
                "Lib89".ToUpper(), "a20".ToUpper(), "b16".ToUpper(), "AD19".ToUpper(), "Pd5".ToUpper(), "jk101".ToUpper()
            }));

            f = "=jk101 - 1";
            test = (HashSet<string>)ss.SetContentsOfCell("Next1", f);
            Assert.AreEqual(1, test.Count);
            f = "=Next1 * jk101";
            test = (HashSet<string>)ss.SetContentsOfCell("Next2", f);
            Assert.AreEqual(1, test.Count);
            f = "=Next2 - a20";
            test = (HashSet<string>)ss.SetContentsOfCell("Next3", f);
            Assert.AreEqual(1, test.Count);
            test = (HashSet<string>)ss.SetContentsOfCell("a20", "irrelevant");
            Assert.AreEqual(9, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string>
            {
                "Lib89".ToUpper(), "a20".ToUpper(), "b16".ToUpper(), "AD19".ToUpper(), "Pd5".ToUpper(), "jk101".ToUpper(), "Next1".ToUpper(), "Next2".ToUpper(), "Next3".ToUpper()
            }));

            test = (HashSet<string>)ss.SetContentsOfCell("Next3", "whocares");
            Assert.AreEqual(1, test.Count);
        }

        [TestMethod]
        public void SCCFStress()
        {
            Spreadsheet ss = new Spreadsheet();
            HashSet<string> test = (HashSet<string>)ss.SetContentsOfCell("ROOT1", "0");
            Assert.AreEqual(1, test.Count);
            for (int i = 1; i <= 100; i++)
            {
                string f = "=ROOT1";
                ss.SetContentsOfCell("A" + i.ToString(), f);

                f = "=A" + i.ToString();
                ss.SetContentsOfCell("B" + i.ToString(), f);

                f = "=B" + i.ToString();
                ss.SetContentsOfCell("C" + i.ToString(), f);

                f = "=C" + i.ToString();
                ss.SetContentsOfCell("D" + i.ToString(), f);

                f = "=D" + i.ToString();
                ss.SetContentsOfCell("E" + i.ToString(), f);

                f = "=E" + i.ToString();
                ss.SetContentsOfCell("STUB1", f);

                test = (HashSet<string>)ss.SetContentsOfCell("ROOT1", "0");
                Assert.AreEqual(i * 5 + 2, test.Count);
            }

            test = (HashSet<string>)ss.SetContentsOfCell("ROOT1", "0");
            Assert.AreEqual(502, test.Count);

            test = (HashSet<string>)ss.SetContentsOfCell("B1", "0");
            Assert.AreEqual(4, test.Count);

            test = (HashSet<string>)ss.SetContentsOfCell("STUB1", "why bother");
            Assert.AreEqual(1, test.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SCCFInvalidName()
        {
            Spreadsheet ss = new Spreadsheet();
            string f = "=a12 / b + 72";
            ss.SetContentsOfCell("Lib89", f);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SCCFCircular()
        {
            Spreadsheet ss = new Spreadsheet();
            string f = "=B1 * 2";
            ss.SetContentsOfCell("A1", f);
            f = "=A1 + 1";
            ss.SetContentsOfCell("A1", f);
        }

        #endregion

        /// <summary>
        /// Helper method that converts an IEnumerable string object to a HashSet string object.
        /// </summary>
        public HashSet<string> IEToHash(IEnumerable<string> ie)
        {
            HashSet<string> result = new HashSet<string>();
            foreach (string s in ie)
            {
                result.Add(s);
            }
            return result;
        }

        /// <summary>
        /// Helper method converts a formula string to the normalized form achieved through
        /// Spreadsheet.SetContentsOfCell and Formula.ToString().
        /// </summary>
        public string NormalizeFormulaString(string formulaString)
        {
            return new Formula(formulaString.Remove(0, 1), s => s.ToUpper(), s => true).ToString();
        }

        /// <summary>
        /// Built to test the following specification:
        /// 
        /// When you are done, another developer using your library should be able to create an
        /// AbstractSpreadsheet object with the following code:
        /// 
        /// <code>AbstractSpreadsheet sheet = new Spreadsheet();</code>
        /// </summary>
        [TestMethod]
        public void MiscellaneousRequirement()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
        }

        #endregion

        #region PS6

        [TestMethod]
        public void LookupDouble()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "4");
            Assert.AreEqual(4, ss.GetCellContents("a1"));
            Assert.AreEqual(4, ss.GetCellContents("a1"));
            ss.SetContentsOfCell("a1", "4");
            ss.SetContentsOfCell("a1", "4");
        }

        [TestMethod]
        public void LookupString()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "adam");
            Assert.AreEqual("adam", ss.GetCellContents("a1"));
            Assert.AreEqual("adam", ss.GetCellContents("a1"));
        }

        [TestMethod]
        public void LookupFormulaNoVars()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "=4");
            Assert.AreEqual(4, ss.GetCellContents("a1"));
            Assert.AreEqual(4, ss.GetCellContents("a1"));
        }

        // lookup formula with one var
        // lookup formula with multiple vars on one level
        // lookup formula with nested vars
        // lookup formula with var stress test

        #endregion
    }
}
