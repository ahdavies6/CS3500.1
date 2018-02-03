using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;

namespace DGTest
{
    [TestClass]
    public class DGTestPositives
    {
        /// <summary>
        /// Ensures AddDependency, Size, HasDependents, HasDependees, GetDependents,
        /// GetDependees, and RemoveDependency all function as intended with one dependency.
        /// </summary>
        [TestMethod]
        public void TestOne()
        {
            DependencyGraph graph = new DependencyGraph();
            string dependee = "coffee";
            string dependent = "Adam";
            graph.AddDependency(dependee, dependent);

            Assert.AreEqual(1, graph.Size);
            Assert.IsTrue(!graph.HasDependents(dependent));
            Assert.IsTrue(graph.HasDependees(dependent));
            Assert.IsTrue(!graph.HasDependees(dependee));
            Assert.IsTrue(graph.HasDependents(dependee));

            int n = 0;
            foreach (string s in graph.GetDependents(dependent))
            {
                Assert.AreEqual(dependee, s);
                n++;
            }
            Assert.AreEqual(1, n);

            n = 0;
            foreach (string s in graph.GetDependees(dependee))
            {
                Assert.AreEqual(dependent, s);
                n++;
            }
            Assert.AreEqual(1, n);

            graph.RemoveDependency(dependee, dependent);
            Assert.AreEqual(0, graph.Size);
            Assert.IsTrue(!graph.HasDependents(dependent));
            Assert.IsTrue(!graph.HasDependees(dependent));
            Assert.IsTrue(!graph.HasDependees(dependee));
            Assert.IsTrue(!graph.HasDependents(dependee));
            Assert.IsNull(graph.GetDependents(dependent));
            Assert.IsNull(graph.GetDependees(dependee));
            Assert.IsNull(graph.GetDependents(dependee));
            Assert.IsNull(graph.GetDependees(dependent));
        }

        /// <summary>
        /// Ensures AddDependency, Size, HasDependents, HasDependees, GetDependents,
        /// GetDependees, and RemoveDependency all function as intended with many dependencies.
        /// May be called a "stress test".
        /// </summary>
        [TestMethod]
        public void TestAddMany()
        {
            DependencyGraph graph = new DependencyGraph();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    graph.AddDependency(i.ToString(), (i * j).ToString());
                }
            }

            // graph.Size equals 9901 instead of 10000 because, for the first iteration of i
            // (i = 0), all iterations after 0 of j (j > 0) only insert duplicate values
            // (because all t values in dependencies (s, t) will be (0, 0) for any i = 0)
            Assert.AreEqual(9901, graph.Size);

            Assert.IsTrue(graph.HasDependents("0"));
            Assert.IsTrue(graph.HasDependees("0"));

            List<string> dependees = new List<string>();
            foreach (string s in graph.GetDependees("0"))
            {
                dependees.Add(s);
            }
            Assert.AreEqual(1, dependees.Count);

            for (int i = 1; i < 100; i++)
            {
                string iString = i.ToString();
                Assert.IsTrue(graph.HasDependents(iString));
                Assert.IsTrue(graph.HasDependees(iString));

                foreach (string s in graph.GetDependents(iString))
                {
                    int n = Int32.Parse(s);
                    Assert.IsTrue(n > 0 && n < 100);
                }

                foreach (string s in graph.GetDependees(iString))
                {
                    int n = Int32.Parse(s);
                    Assert.AreEqual(0, n % i);
                }
            }

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    graph.RemoveDependency((i * 2).ToString(), ((i * 2) * j).ToString());
                }
            }

            Assert.AreEqual(5000, graph.Size);

            for (int i = 1; i < 100; i++)
            {
                string iString = i.ToString();

                if (i % 2 == 0)
                {
                    Assert.IsFalse(graph.HasDependees(iString));
                }
                else
                {
                    Assert.IsTrue(graph.HasDependents(iString));
                    Assert.IsTrue(graph.HasDependees(iString));

                    foreach (string s in graph.GetDependents(iString))
                    {
                        int n = Int32.Parse(s);
                        Assert.IsTrue(n > 0 && n < 100);
                    }

                    foreach (string s in graph.GetDependees(iString))
                    {
                        int n = Int32.Parse(s);
                        Assert.AreEqual(0, n % i);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures ReplaceDependents and ReplaceDependees function as intended with a few
        /// dependencies.
        /// </summary>
        [TestMethod]
        public void TestReplaceSmall()
        {
            DependencyGraph graph = new DependencyGraph();

            // what I'm dependent on
            string aa = "tea";
            string ab = "philosophy";
            string ac = "free time";
            // who they're dependees of
            string dependee = "Adam";
            graph.AddDependency(aa, dependee);
            graph.AddDependency(ab, dependee);
            graph.AddDependency(ac, dependee);

            // what I'll be dependent on by the time I finish my major
            string[] replace = { "coffee", "engineering", "productivity" };
            graph.ReplaceDependents(dependee, replace);

            Assert.AreEqual(3, graph.Size);
            int i = 0;
            // soon:
            //foreach (string s in graph.GetDependents(dependee))
            // now:
            foreach (string s in graph.GetDependees(dependee))
            {
                Assert.AreEqual(replace[i], s);
                i++;
            }
            Assert.AreEqual(3, i);

            //string ba = "knowledge";
            //string bb = "experience";
            //string bc = "";
            //graph.AddDependency(dependnet,)
        }
    }

    [TestClass]
    public class DGTestNegatives
    {
        /// <summary>
        /// Ensures HasDependents raises the right exception for a null argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull1()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.HasDependents(null);
        }

        /// <summary>
        /// Ensures HasDependees raises the right exception for a null argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull2()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.HasDependees(null);
        }

        /// <summary>
        /// Ensures GetDependents raises the right exception for a null argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull3()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.GetDependents(null);
        }

        /// <summary>
        /// Ensures GetDependees raises the right exception for a null argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull4()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.GetDependees(null);
        }

        /// <summary>
        /// Ensures AddDependency raises the right exception for a null argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull5()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("hi", null);
        }

        /// <summary>
        /// Ensures RemoveDependency raises the right exception for a null argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull6()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.RemoveDependency(null, "hi");
        }

        /// <summary>
        /// Ensures ReplaceDependents raises the right exception when one element of
        /// the IEnumerable argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestNull7()
        {
            DependencyGraph graph = new DependencyGraph();
            string[] nuller = { "hi", null, "am broken" };
            graph.ReplaceDependents("hi", nuller);
        }

        /// <summary>
        /// Ensures ReplaceDependees raises the right exception for a null argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNull8()
        {
            DependencyGraph graph = new DependencyGraph();
            string[] fine = { "hi", "I'm", "fine" };
            graph.ReplaceDependees(null, fine);
        }
    }
}
