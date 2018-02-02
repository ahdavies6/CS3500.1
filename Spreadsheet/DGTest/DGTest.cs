using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;

namespace DGTest
{
    [TestClass]
    public class DGTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull1()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.HasDependents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull2()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.HasDependees(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull3()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.GetDependents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull4()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.GetDependees(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull5()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("hi", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull6()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.RemoveDependency(null, "hi");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull7()
        {
            DependencyGraph graph = new DependencyGraph();
            string[] nuller = { "hi", null, "am broken" };
            graph.ReplaceDependents("hi", nuller);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull8()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.ReplaceDependees(null, null);
        }

        [TestMethod]
        public void TestAdd1()
        {
            DependencyGraph graph = new DependencyGraph();
            string dependent = "Adam";
            string dependee = "coffee";
            graph.AddDependency(dependent, dependee);

            Assert.AreEqual(1, graph.Size);
            Assert.IsTrue(graph.HasDependents(dependee));
            Assert.IsTrue(!graph.HasDependees(dependee));
            Assert.IsTrue(graph.HasDependees(dependent));
            Assert.IsTrue(!graph.HasDependents(dependent));

            int n = 0;
            foreach (string s in graph.GetDependents(dependee))
            {
                Assert.AreEqual(dependent, s);
                n++;
            }
            Assert.AreEqual(1, n);

            n = 0;
            foreach (string s in graph.GetDependees(dependent))
            {
                Assert.AreEqual(dependee, s);
                n++;
            }
            Assert.AreEqual(1, n);
        }
    }
}
