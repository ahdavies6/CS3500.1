// Implementation completed by:
// Adam Davies
// CS 3500-001
// Spring Semester 2018

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;

namespace PS4bTests
{
    [TestClass]
    public class Tests
    {
        #region Arg Null Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest1()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency(null, "a");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest2()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest3()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.GetDependees(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest4()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.GetDependents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest5()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.HasDependees(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest6()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.HasDependents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest7()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.RemoveDependency(null, "a");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest8()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.RemoveDependency("a", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest9()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.ReplaceDependees("a", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest10()
        {
            DependencyGraph graph = new DependencyGraph();
            string[] a = { "a" };
            graph.ReplaceDependees(null, a);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest11()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.ReplaceDependents("a", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest12()
        {
            DependencyGraph graph = new DependencyGraph();
            string[] a = { "a" };
            graph.ReplaceDependents(null, a);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest13()
        {
            DependencyGraph graph = new DependencyGraph();
            string[] a = { "a", null };
            graph.ReplaceDependees("b", a);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest14()
        {
            DependencyGraph graph = new DependencyGraph();
            string[] a = { "a", null };
            graph.ReplaceDependents("b", a);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgNullTest15()
        {
            DependencyGraph graph = new DependencyGraph(null);
        }

        #endregion

        #region Constructor Overload Tests

        // Ensures that the overloaded constructor functions as intended.

        [TestMethod]
        public void COEmptyTest()
        {
            // todo remove extra code
            DependencyGraph dg1 = new DependencyGraph();
            DependencyGraph dg2 = new DependencyGraph(dg1);
            Assert.AreEqual(dg1.Size, dg2.Size);

            // dg1 should be unchanged

            // dg1 and dg2 should contain exactly the same set of dependencies

            // dg1 and dg2 should be independent of each other (modifying one should not modify the other)
            string dee = "a";
            string dent1 = "b";
            string dent2 = "c";

            dg1.AddDependency(dee, dent1);
            Assert.AreEqual(1, dg1.Size);
            Assert.AreEqual(0, dg2.Size);

            dg2.AddDependency(dee, dent2);
            List<string> dg1dee = (List<string>)dg1.GetDependees(dent1);
            List<string> dg1dent = (List<string>)dg1.GetDependents(dee);
            List<string> dg2dee = (List<string>)dg1.GetDependees(dent2);
            List<string> dg2dent = (List<string>)dg2.GetDependents(dee);
            Assert.AreEqual(1, dg1.Size);
            Assert.AreEqual(1, dg2.Size);

            Assert.AreEqual(dg1dee, dg2dee);
            Assert.AreNotEqual(dg1dent, dg2dent);
        }

        // A few stress tests:

        [TestMethod]
        public void COStressTest1()
        {
            DependencyGraph dg1 = new DependencyGraph();
            List<string> dg1all = new List<string>();
            // add stuff in here
            List<string> dg1dee1 = new List<string>();
            foreach (string s in dg1all)
            {
                dg1dee1.Add(s);
            }
            List<string> dg1dent1 = new List<string>();
            foreach (string s in dg1all)
            {
                dg1dent1.Add(s);
            }

            DependencyGraph dg2 = new DependencyGraph(dg1);
            Assert.AreEqual(dg1.Size, dg2.Size);

            // dg1 should be unchanged
            List<string> dg1dee2 = new List<string>();
            foreach (string s in dg1all)
            {
                dg1dee2.Add(s);
            }
            List<string> dg1dent2 = new List<string>();
            foreach (string s in dg1all)
            {
                dg1dent2.Add(s);
            }
            CollectionAssert.AreEquivalent(dg1dee1, dg1dee2);
            CollectionAssert.AreEquivalent(dg1dent1, dg1dent2);

            // dg1 and dg2 should contain exactly the same set of dependencies
            List<string> dg2dee = new List<string>();
            foreach (string s in dg1all)
            {
                dg2dee.Add(s);
            }
            List<string> dg2dent = new List<string>();
            foreach (string s in dg1all)
            {
                dg2dent.Add(s);
            }
            CollectionAssert.AreEquivalent(dg1dee2, dg2dee);
            CollectionAssert.AreEquivalent(dg1dent2, dg2dent);

            // dg1 and dg2 should be independent of each other (modifying one should not modify the other)
            // modify stuff in here
        }

        [TestMethod]
        public void COStressTest2()
        {
            COStressTest1();
        }

        [TestMethod]
        public void COStressTest3()
        {
            COStressTest1();
        }

        [TestMethod]
        public void COStressTest4()
        {
            COStressTest1();
        }

        #endregion
    }
}
