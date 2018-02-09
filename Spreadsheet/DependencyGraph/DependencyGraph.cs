// Skeleton implementation written by Joe Zachary for CS 3500, January 2018.
//
// Full implementation completed by:
// Adam Davies
// CS 3500-001
// Spring Semester 2018

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
            get
            {
                int n = 0;
                foreach (DependencyNode node in nodes.Values)
                {
                    n += node.Size;
                }
                // division by two because, for each node N representing all dependents s of t,
                // there is a mirror node N* representing all dependees t of s.
                // (In other words, each dependency involves a dependent and a dependee. DependencyGraph
                // represents dependencies as relationships between nodes, where each dependency
                // involves a dependent and a dependency. In one dependency, there is a dependent
                // and a dependee; these are each represented in DependencyNodes, with each of two nodes
                // representing the same dependency. This means each dependency is represented
                // twice.)
                return n / 2;
            }
        }

        /// <summary>
        /// Enumerates the keys of all DependencyNodes in the DependencyGraph.
        /// </summary>
        internal IEnumerable<string> GetNodes
        {
            get
            {
                foreach (string s in nodes.Keys)
                {
                    yield return s;
                }
            }
        }

        /// <summary>
        /// All the nodes (vertices) in the DependencyGraph.
        /// </summary>
        private Dictionary<string, DependencyNode> nodes;

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            nodes = new Dictionary<string, DependencyNode>();
        }

        /// <summary>
        /// Creates a DependencyGraph that is an exact copy of graph.
        /// Requires graph != null.
        /// </summary>
        public DependencyGraph(DependencyGraph graph) : this()
        {
            if (graph != null)
            {
                foreach (string s in graph.GetNodes)
                {
                    foreach (string t in graph.GetDependents(s))
                    {
                        AddDependency(s, t);
                    }
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (s != null)
            {
                if (nodes.ContainsKey(s))
                {
                    return nodes[s].Dependents.Count > 0;
                }
                else
                {
                    return false;
                }
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
                if (nodes.ContainsKey(s))
                {
                    return nodes[s].Dependees.Count > 0;
                }
                else
                {
                    return false;
                }
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
                if (nodes.ContainsKey(s))
                {
                    return GetDependentsInternal(s);
                }
                else
                {
                    return System.Linq.Enumerable.Empty<string>();
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Enumerates dependents(s). Handles yield return so public GetDependents
        /// can (non-yield) return null.
        /// 
        /// Requires s != null.
        /// </summary>
        private IEnumerable<string> GetDependentsInternal(string s)
        {
            if (s != null)
            {
                foreach (string key in nodes[s].Dependents.Keys)
                {
                    yield return key;
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Enumerates dependees(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (s != null)
            {
                if (nodes.ContainsKey(s))
                {
                    return GetDependeesInternal(s);
                }
                else
                {
                    return System.Linq.Enumerable.Empty<string>();
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Enumerates dependents(s). Handles yield return so public GetDependees
        /// can (non-yield) return null.
        /// 
        /// Requires s != null.
        /// </summary>
        private IEnumerable<string> GetDependeesInternal(string s)
        {
            if (s != null)
            {
                foreach (string key in nodes[s].Dependees.Keys)
                {
                    yield return key;
                }
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
                DependencyNode dependee;
                DependencyNode dependent;

                if (nodes.ContainsKey(s))
                {
                    dependee = nodes[s];
                }
                else
                {
                    dependee = new DependencyNode(s);
                    nodes.Add(s, dependee);
                }

                if (nodes.ContainsKey(t))
                {
                    dependent = nodes[t];
                }
                else
                {
                    dependent = new DependencyNode(t);
                    nodes.Add(t, dependent);
                }

                if (!dependee.Dependents.ContainsKey(t))
                {
                    dependee.Dependents.Add(t, dependent);
                }
                if (!dependent.Dependees.ContainsKey(s))
                {
                    dependent.Dependees.Add(s, dependee);
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
                if (nodes.ContainsKey(s) && nodes.ContainsKey(t))
                {
                    nodes[s].Dependents.Remove(t);
                    nodes[t].Dependees.Remove(s);

                    // if that was the last dependency the node was involved in, get
                    // rid of it to free up memory
                    if (nodes[s].Size == 0)
                    {
                        nodes.Remove(s);
                    }
                    if (nodes[t].Size == 0)
                    {
                        nodes.Remove(t);
                    }
                }
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
            if (s != null && newDependents != null)
            {
                if (HasDependents(s))
                {
                    List<string> keys = new List<string>();
                    foreach (string key in GetDependents(s))
                    {
                        keys.Add(key);
                    }
                    foreach (string key in keys)
                    {
                        RemoveDependency(s, key);
                    }
                }
                foreach (string t in newDependents)
                {
                    if (t != null)
                    {
                        AddDependency(s, t);
                    }
                    else
                    {
                        throw new ArgumentNullException();
                    }
                }
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
            if (t != null && newDependees != null)
            {
                if (HasDependees(t))
                {
                    List<string> keys = new List<string>();
                    foreach (string key in GetDependees(t))
                    {
                        keys.Add(key);
                    }
                    foreach (string key in keys)
                    {
                        RemoveDependency(key, t);
                    }
                }
                foreach (string s in newDependees)
                {
                    if (s != null)
                    {
                        AddDependency(s, t);
                    }
                    else
                    {
                        throw new ArgumentNullException();
                    }
                }
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
        /// <summary>
        /// The node's key in DependencyNode's Nodes Dictionary.
        /// </summary>
        private string myKey;

        public int Size
        {
            get
            {
                return myDependeees.Count + myDependents.Count;
            }
        }

        /// <summary>
        /// All dependents of this node.
        /// </summary>
        public Dictionary<string, DependencyNode> Dependees
        {
            get { return myDependeees; }
        }
        private Dictionary<string, DependencyNode> myDependeees;

        /// <summary>
        /// All dependees of this node.
        /// </summary>
        public Dictionary<string, DependencyNode> Dependents
        {
            get { return myDependents; }
        }
        private Dictionary<string, DependencyNode> myDependents;

        /// <summary>
        /// Creates a DependencyNode of name (key) with an empty set of dependents and an empty set of dependees.
        /// Requires key != null.
        /// </summary>
        public DependencyNode(string key)
        {
            if (key != null)
            {
                myKey = key;
                myDependeees = new Dictionary<string, DependencyNode>();
                myDependents = new Dictionary<string, DependencyNode>();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
    }
}
