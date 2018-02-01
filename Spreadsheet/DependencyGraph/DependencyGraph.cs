// Skeleton implementation written by Joe Zachary for CS 3500, January 2018.

using System;
using System.Collections.Generic;

namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///     
    /// All of the methods below require their string parameters to be non-null.  This means that 
    /// the behavior of the method is undefined when a string parameter is null.  
    ///
    /// IMPORTANT IMPLEMENTATION NOTE
    /// 
    /// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
    /// as discussed above.
    /// 
    /// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
    /// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
    /// 
    /// You'll need to be more clever than that.  Design a representation that is both easy to work
    /// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
    /// the test cases with which you will be graded will create massive DependencyGraphs.  If you
    /// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
    /// </summary>
    public class DependencyGraph
    {
        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            //OG
            //get { return 0; }
            get;
        }

        private Dictionary<string, DependencyNode> nodes;

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            nodes = new Dictionary<string, DependencyNode>();
            Size = 0;
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (s != null)
            {
                return nodes[s].Dependents.Count > 0;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (s != null)
            {
                return nodes[s].Dependees.Count > 0;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Enumerates dependents(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (s != null)
            {
                return nodes[s].Dependents.Keys;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Enumerates dependees(t).  Requires t != null.
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (s != null)
            {
                return nodes[s].Dependees.Keys;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void AddDependency(string s, string t)
        {
            if (s != null && t != null)
            {
                if (nodes.ContainsKey(s))
                {
                    if (!nodes[s].Dependents.Contains(s) && !nodes[s].Dependees.Contains(t))
                    {
                        nodes[s].Dependents.Add(s);
                        nodes[s].Dependees.Add(t);
                    }
                }
                else
                {
                    nodes.Add(s, new DependencyNode());
                    nodes[s].Dependents.Add(s);
                    nodes[s].Dependees.Add(t);
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Requires s != null and t != null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            if (s != null && t != null)
            {

            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (s != null && t != null)
            {

            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            if (s != null && t != null)
            {

            }
            else
            {
                throw new ArgumentNullException();
            }
        }
    }

    /// <summary>
    /// Represents a node in DependencyGraph. Contains a set of dependents and a set of and dependees.
    /// </summary>
    internal class DependencyNode
    {
        public Dictionary<string, DependencyNode> Dependents { get; set; }
        public Dictionary<string, DependencyNode> Dependees { get; set; }

        /// <summary>
        /// Creates a DependencyNode with an empty set of dependents and an empty set of dependees.
        /// </summary>
        public DependencyNode()
        {
            Dependents = new Dictionary<string, DependencyNode>();
            Dependees = new Dictionary<string, DependencyNode>();
        }

        public void AddDependent(string s, DependencyGraph graph)
        {
            if (s != null)
            {
                if (Dependents.ContainsKey(s))
                {
                    if (!Dependents[s].Dependents.Contains(s) && !Dependents[s].Dependees.Contains(t))
                    {
                        Dependents[s].Dependents.Add(s);
                        Dependents[s].Dependees.Add(t);
                    }
                }
                else
                {
                    Dependents.Add(s, new DependencyNode());

                    if (graph.HasDependents(s))
                    {
                        Dependents.Add(s, graph.GetDependents)
                    }
                    //Dependents[s].Dependents.Add(s);
                    //Dependents[s].Dependees.Add(t);
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
    }
}
