using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Dependencies;
using Formulas;
using System.Collections.Generic;

namespace SpreadsheetTests
{
    [TestClass]
    public class Tests
    {
        #region GetNamesOfAllNonemptyCells Tests
        #endregion

        #region GetCellContents Tests

        [TestMethod]
        public void GCC()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("pNzlefEEKN942", 19);
            ss.SetCellContents("zYLd419", "who knows what this will be?");
            Formula f = new Formula("a1 + b20 / 7");
            ss.SetCellContents("inFEWNnnk107", f);

            Assert.AreEqual((double)19, ss.GetCellContents("pNzlefEEKN942"));
            Assert.AreEqual("who knows what this will be?", ss.GetCellContents("zYLd419"));
            Assert.AreEqual(f, ss.GetCellContents("inFEWNnnk107"));

            // Tests for Formula cells included in SCCFIndirect()
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
            ss.SetCellContents("!A1", 2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidCellName2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("A", "text");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidCellName3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("A0", new Formula());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidCellName4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("A1!", 2);
        }

        [TestMethod]
        public void ValidCellName1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("A1", 2);
        }

        [TestMethod]
        public void ValidCellName2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a10", 2);
        }

        [TestMethod]
        public void ValidCellName3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("iWanTTObeDonEWIThThiS666", 2);
        }

        [TestMethod]
        public void ValidCellName4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("ThisSSHOUldWoRk104329047", 2);
        }

        #endregion

        #region SetCellContents (double, string) Tests

        [TestMethod]
        public void SCCD()
        {
            Spreadsheet ss = new Spreadsheet();
            HashSet<string> test = (HashSet<string>)ss.SetCellContents("b60", 2);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "b60" }));
            Assert.AreEqual((double)2, ss.GetCellContents("b60"));

            test = (HashSet<string>)ss.SetCellContents("b60", 199);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "b60" }));
            Assert.AreEqual((double)199, ss.GetCellContents("b60"));

            test = (HashSet<string>)ss.SetCellContents("b60", "now I'm a string!");
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "b60" }));
            Assert.AreEqual("now I'm a string!", ss.GetCellContents("b60"));

            test = (HashSet<string>)ss.SetCellContents("b60", 2);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "b60" }));
            Assert.AreEqual((double)2, ss.GetCellContents("b60"));
        }

        [TestMethod]
        public void SCCS()
        {
            Spreadsheet ss = new Spreadsheet();
            HashSet<string> test = (HashSet<string>)ss.SetCellContents("AdD1423", "IsString");
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "AdD1423" }));
            Assert.AreEqual("IsString", ss.GetCellContents("AdD1423"));

            test = (HashSet<string>)ss.SetCellContents("AdD1423", "different one!");
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "AdD1423" }));
            Assert.AreEqual("different one!", ss.GetCellContents("AdD1423"));

            test = (HashSet<string>)ss.SetCellContents("AdD1423", 42);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "AdD1423" }));
            Assert.AreEqual((double)42, ss.GetCellContents("AdD1423"));

            test = (HashSet<string>)ss.SetCellContents("AdD1423", "finally:");
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "AdD1423" }));
            Assert.AreEqual("finally:", ss.GetCellContents("AdD1423"));
        }

        #endregion

        #region SetCellContents (formula) Tests

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
            Formula f = new Formula();
            HashSet<string> test = (HashSet<string>)ss.SetCellContents("Lib89", f);
            Assert.AreEqual(1, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "Lib89" }));
            Assert.AreEqual(f, ss.GetCellContents("Lib89"));

            f = new Formula("a20 - b16 + (876 * b16) / 7");
            test = (HashSet<string>)ss.SetCellContents("Lib89", f);
            Assert.AreEqual(f, ss.GetCellContents("Lib89"));
            Assert.AreEqual(1, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "Lib89" }));

            test = (HashSet<string>)ss.SetCellContents("a20", 5);
            Assert.AreEqual(2, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "a20", "Lib89" }));

            test = (HashSet<string>)ss.SetCellContents("b16", "string boi");
            Assert.AreEqual(2, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "Lib89", "b16" }));

            f = new Formula("a20 + 1");
            test = (HashSet<string>)ss.SetCellContents("b16", f);
            Assert.AreEqual(2, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "Lib89", "b16" }));

            test = (HashSet<string>)ss.SetCellContents("a20", 7);
            Assert.AreEqual(3, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "a20", "Lib89", "b16" }));

            return ss;
        }

        /// <summary>
        /// Tests only the material in SCCFStart. Makes sure at least direct formual dependencies
        /// are functioning as intended.
        /// </summary>
        [TestMethod]
        public void SCCFDirect()
        {
            // Make sure all the assertions in SCCFStart are passing.
            SCCFStart();
        }

        [TestMethod]
        public void SCCFIndirect()
        {
            // Start with the spreadsheet from SCCFStart. We'll add indirect dependencies soon.
            Spreadsheet ss = SCCFStart();

            // Making sure GetCellContents retrieves the right thing, after all the modifications
            // we've made.
            Formula f = (Formula)ss.GetCellContents("Lib89");
            Assert.AreEqual(new Formula("a20 - b16 + (876 * b16) / 7").ToString(), f.ToString());

            // 2 layers
            f = new Formula("AD19 * 2");
            ss.SetCellContents("b16", f);
            f = new Formula("a20 + 1");
            ss.SetCellContents("AD19", f);
            HashSet<string> test = (HashSet<string>)ss.SetCellContents("a20", "doesn't even matter");
            Assert.AreEqual(4, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "Lib89", "b16", "AD19", "a20" }));

            // 4 layers
            f = new Formula("b16 + 1");
            ss.SetCellContents("Pd5", f);
            f = new Formula("Pd5 - AD19");
            ss.SetCellContents("jk101", f);
            test = (HashSet<string>)ss.SetCellContents("a20", "doesn't even matter");
            Assert.AreEqual(6, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string>
            {
                "Lib89", "a20", "b16", "AD19", "Pd5", "jk101"
            }));

            // 1 more
            f = new Formula("jk101 - 1");
            ss.SetCellContents("Next1", f);
            f = new Formula("Next1 * jk101");
            ss.SetCellContents("Next2", f);
            f = new Formula("Next2 - a20");
            ss.SetCellContents("Next3", f);
            test = (HashSet<string>)ss.SetCellContents("a20", "irrelevant");
            Assert.AreEqual(9, test.Count);

            // todo: test return ISet (MAKE SURE 3+ LAYERS DEEP WORKS)
            // also make sure disconnected ones arent included
            // also make sure we don't get back dependees when we want dependents
        }

        // 52 nodes; 211,111 connections
        [TestMethod]
        public void SCCFFStress()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("ROOT1", 0);
            for (int a = 1; a <= 10; a++)
            {
                Formula f = new Formula("ROOT1");
                ss.SetCellContents("A" + a.ToString(), f);

                for (int b = 1; b <= 10; b++)
                {
                    f = new Formula("A" + a.ToString());
                    ss.SetCellContents("B" + b.ToString(), f);

                    for (int c = 1; c <= 10; c++)
                    {
                        f = new Formula("B" + b.ToString());
                        ss.SetCellContents("C" + c.ToString(), f);

                        for (int d = 1; d <= 10; d++)
                        {
                            f = new Formula("C" + c.ToString());
                            ss.SetCellContents("D" + d.ToString(), f);

                            for (int e = 1; e <= 10; e++)
                            {
                                f = new Formula("D" + d.ToString());
                                ss.SetCellContents("E" + e.ToString(), f);

                                f = new Formula("E" + e.ToString());
                                ss.SetCellContents("STUB1", f);
                            }
                        }
                    }
                }
            }

            HashSet<string> test = (HashSet<string>)ss.SetCellContents("ROOT1", 1);
            Assert.AreEqual(211111, test.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SCCFInvalidName()
        {
            Spreadsheet ss = new Spreadsheet();
            Formula f = new Formula("a12 / b + 72");
            ss.SetCellContents("Lib89", f);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SCCFCircular()
        {
            // todo: test circular dependencies
        }

        #endregion

        #region GetDirectDependents Tests

        // todo: remove following comment
        // What can I even test here? Nothing black-box, presumably...

        #endregion

        #region GetCellsToRecalculate Tests
        #endregion

        // todo: make sure the next line works in another namespace
        //AbstractSpreadsheet sheet = new Spreadsheet();
    }
}
