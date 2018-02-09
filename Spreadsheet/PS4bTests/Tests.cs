// Implementation completed by:
// Adam Davies
// CS 3500-001
// Spring Semester 2018

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;
using System.Linq;

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
            DependencyGraph dg1 = new DependencyGraph();
            DependencyGraph dg2 = new DependencyGraph(dg1);
            Assert.AreEqual(0, dg1.Size);
            Assert.AreEqual(dg1.Size, dg2.Size);

            // dg1 and dg2 should be independent of each other (modifying one should not modify the other)
            string dee = "a";
            string dent1 = "b";
            string dent2 = "c";

            dg1.AddDependency(dee, dent1);
            Assert.AreEqual(1, dg1.Size);
            Assert.AreEqual(0, dg2.Size);

            dg2.AddDependency(dee, dent2);
            string dg1dee = IE0ToString(dg1.GetDependees(dent1));
            string dg1dent = IE0ToString(dg1.GetDependents(dee));
            string dg2dee = IE0ToString(dg2.GetDependees(dent2));
            string dg2dent = IE0ToString(dg2.GetDependents(dee));
            Assert.AreEqual(1, dg1.Size);
            Assert.AreEqual(1, dg2.Size);
            Assert.AreEqual("a", dg1dee);
            Assert.AreEqual("b", dg1dent);
            Assert.AreEqual("a", dg2dee);
            Assert.AreEqual("c", dg2dent);
            Assert.AreEqual(dg1dee, dg2dee);
            Assert.AreNotEqual(dg1dent, dg2dent);
        }

        private string IE0ToString(IEnumerable<string> ie)
        {
            return ie.First();
        }

        // A few stress tests:

        [TestMethod]
        public void COStressTest1()
        {
            DependencyGraph dg1 = new DependencyGraph();
            List<string> allDents = new List<string>();
            for (int i = 0; i < 10000; i++)
            {
                allDents.Add(((i + 1) * 3).ToString());
                allDents.Add(((i + 1) * 4).ToString());
                dg1.AddDependency((i).ToString(), ((i + 1) * 3).ToString());
                dg1.AddDependency((i).ToString(), ((i + 1) * 4).ToString());
            }
            Assert.AreEqual(20000, dg1.Size);
            DependencyGraph dg2 = new DependencyGraph(dg1);

            // dg1 should be unchanged
            Assert.AreEqual(20000, dg1.Size);
            List<string> dg1DentCheck = new List<string>();
            for (int i = 0; i < 10000; i++)
            {
                foreach (string s in dg1.GetDependents((i).ToString()))
                {
                    dg1DentCheck.Add(s);
                }
            }
            CollectionAssert.AreEquivalent(allDents, dg1DentCheck);

            // dg1 and dg2 should contain exactly the same set of dependencies
            Assert.AreEqual(dg1.Size, dg2.Size);
            List<string> dg1Dents = new List<string>();
            List<string> dg2Dents = new List<string>();
            for (int i = 0; i < 10000; i++)
            {
                int j = 0;
                foreach (string s in dg1.GetDependents((i).ToString()))
                {
                    dg1Dents.Add(s);
                    j++;
                }

                j = 0;
                foreach (string s in dg2.GetDependents((i).ToString()))
                {
                    dg2Dents.Add(s);
                    j++;
                }
            }
            CollectionAssert.AreEquivalent(dg1Dents, dg2Dents);

            // dg1 and dg2 should be independent of each other (modifying one should not modify the other)
            for (int i = 0; i < 10000; i++)
            {
                dg1.RemoveDependency(i.ToString(), ((i + 1) * 3).ToString());
                dg1Dents.Remove(((i + 1) * 3).ToString());
            }
            Assert.AreEqual(10000, dg1.Size);
            Assert.AreEqual(20000, dg2.Size);
            Assert.AreEqual(10000, dg1Dents.Count);
            Assert.AreEqual(20000, dg2Dents.Count);

            for (int i = 0; i < 10000; i++)
            {
                dg2.RemoveDependency(i.ToString(), ((i + 1) * 4).ToString());
                dg2Dents.Remove(((i + 1) * 4).ToString());
            }
            Assert.AreEqual(10000, dg1.Size);
            Assert.AreEqual(10000, dg2.Size);
            Assert.AreEqual(10000, dg1Dents.Count);
            Assert.AreEqual(10000, dg2Dents.Count);
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
