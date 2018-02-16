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

        #region SetCellContents (all overloads) Tests

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

        [TestMethod]
        public void SCCF1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("Lib89", new Formula());
            Formula f = new Formula();
            Assert.AreEqual(f, ss.GetCellContents("Lib89"));

            f = new Formula("a20 - b16 + (876 * b16) / 7");
            ss.SetCellContents("Lib89", f);
            Assert.AreEqual(f, ss.GetCellContents("Lib89"));

            // todo: test return ISet (MAKE SURE 3+ LAYERS DEEP WORKS)
            // also make sure disconnected one's arent included
            // also make sure dependents != dependees...
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SCCF2()
        {
            Spreadsheet ss = new Spreadsheet();
            Formula f = new Formula("a12 / b + 72");
            ss.SetCellContents("Lib89", f);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SCCF3()
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
