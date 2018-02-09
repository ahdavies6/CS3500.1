// Implementation completed by:
// Adam Davies
// CS 3500-001
// Spring Semester 2018

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;

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

        #endregion
    }
}
