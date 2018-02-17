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
            HashSet<string> test = (HashSet<string>)ss.SetCellContents("Lib89", new Formula());
            Formula f = new Formula();
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

            // Here comes the indirect part!!
            f = new Formula("AD19 * 2");
            ss.SetCellContents("b16", f);
            f = new Formula("a20 + 1");
            ss.SetCellContents("AD19", f);
            HashSet<string> test = (HashSet<string>)ss.SetCellContents("a20", "doesn't even matter");
            Assert.AreEqual(4, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "Lib89", "b16", "AD19", "a20" }));

            // passing this means that either it works properly, or it isn't deleting dependencies
            // it isn't deleting dependencies
            f = new Formula("a20 * a20");
            ss.SetCellContents("Lib89", f);
            f = new Formula("Lib89 + 42");
            ss.SetCellContents("b16", f);
            test = (HashSet<string>)ss.SetCellContents("a20", "here we go again");
            Assert.AreEqual(4, test.Count);
            Assert.IsTrue(test.SetEquals(new HashSet<string> { "Lib89", "b16", "AD19", "a20" }));


            // todo: delete this
            // Just to make sure we're golden:
            //f = new Formula("b16 + 1");
            //ss.SetCellContents("Pd5", f);
            //f = new Formula("Pd5 - AD19");
            //ss.SetCellContents("jk101", f);
            //test = (HashSet<string>)ss.SetCellContents("a20", "doesn't even matter");
            //Assert.AreEqual(6, test.Count);
            //Assert.IsTrue(test.SetEquals(new HashSet<string>
            //{
            //    "Lib89", "a20", "b16", "AD19", "Pd5", "jk101"
            //}));

            // todo: test return ISet (MAKE SURE 3+ LAYERS DEEP WORKS)
            // also make sure disconnected one's arent included
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
