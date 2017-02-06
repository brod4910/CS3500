// Skeleton implementation written by Joe Zachary for CS 3500, January 2017.

using System;
using System.Collections.Generic;
using System.Linq;

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
    /// 
    public class DependencyGraph
    {
        private Dictionary<String, HashSet<String>> Dependents;
        private Dictionary<String, HashSet<String>> Dependees;
        private int size = 0;

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            //creates the data sets for the Dependency Graph
            Dictionary<String, HashSet<String>> Dependents = new Dictionary<String, HashSet<String>>();
            Dictionary<String, HashSet<String>> Dependees = new Dictionary<String, HashSet<String>>();

            this.Dependents = Dependents;
            this.Dependees = Dependees;
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependents(string s)
        {
             
            HashSet<String> dependentList = new HashSet<String>();

            //checks to see if string s has dependents and if so return true else false
            if (s != null)
            {
                return Dependents.TryGetValue(s, out dependentList);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.  Requires s != null.
        /// </summary>
        public bool HasDependees(string s)
        {
            HashSet<String> dependeeList = new HashSet<String>();

            //checks to see if string s has dependents and if so return true else false
            if (s != null)
            {
                return Dependees.TryGetValue(s, out dependeeList);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Enumerates dependents(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            HashSet<String> dependentList = new HashSet<String>();

            if(s != null)
            {
                if(Dependents.ContainsKey(s))
                {
                    //trys to get the dependents of the string
                   Dependents.TryGetValue(s, out dependentList);

                    //for each dependent in the list create an IEnumerable
                    foreach(string dependent in dependentList)
                    {
                        yield return dependent;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates dependees(s).  Requires s != null.
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            HashSet<String> dependeeList = new HashSet<String>();

            if (s != null)
            {
                if (Dependees.ContainsKey(s))
                {
                    //trys to get the dependents of the string
                    Dependees.TryGetValue(s, out dependeeList);

                    //for each dependent in the list create an IEnumerable
                    foreach (string dependee in dependeeList)
                    {
                        yield return dependee;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Requires s != null and t != null.
        /// (dependee, dependent)
        /// </summary>
        public void AddDependency(string s, string t)
        {
            if(s != null && t != null)
            {
                HashSet<String> dependentList = new HashSet<String>();
                HashSet<String> dependeeList = new HashSet<String>();
                bool addedDependency = false;

                //if Dependents/Dependees doesn't contain the keys then...
                if (!Dependents.ContainsKey(s) || !Dependees.ContainsKey(t))
                {
                    //If Dependents doesn't contain the key s then add the dependent to 
                    //the Dependents and set addedDependeny to true
                    if (!Dependents.ContainsKey(s))
                    {
                        dependentList.Add(t);
                        Dependents.Add(s, dependentList);
                        addedDependency = true;
                    }
                    //else get the dependents of s
                    else if(Dependents.ContainsKey(s))
                    {
                        Dependents.TryGetValue(s, out dependentList);

                        //if the list doesnt contain then add it to the list
                        //and set addedDependency to true
                        if(!dependentList.Contains(t))
                        {
                            dependentList.Add(t);
                            Dependents.Remove(s);
                            Dependents.Add(s, dependentList);
                            addedDependency = true;
                        }
                    }
                    //if Dependees doesnt contain t
                    if(!Dependees.ContainsKey(t))
                    {
                        //then add the string s to the list of dependees
                        //and add t to the Dependees structure
                        dependeeList.Add(s);
                        Dependees.Add(t, dependeeList);
                        addedDependency = true;
                    }
                    //else if Depndees does contain t get the 
                    //list of dependees of s
                    else if(Dependees.ContainsKey(t))
                    {
                        Dependees.TryGetValue(s, out dependeeList);

                        //if the list doesnt contain t 
                        //then add s to the list and put t into the 
                        //list of Depndees
                        if (!dependeeList.Contains(t))
                        {
                            dependeeList.Add(s);
                            Dependees.Remove(t);
                            Dependees.Add(t, dependeeList);
                            addedDependency = true;
                        }
                    }
                    //if a dependency was added increment by 1
                    if (addedDependency)
                    {
                        size++;
                    }
                }
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
                HashSet<String> dependentList = new HashSet<String>();
                HashSet<String> dependeeList = new HashSet<String>();
                bool removedDependency = false;

                //If Dependents and Dependees is present in the structure then...
                if(Dependents.ContainsKey(s) || Dependees.ContainsKey(t))
                {
                    //if dependents contains key s
                    if(Dependents.ContainsKey(s))
                    {
                        //Get the dependents of string s
                        Dependents.TryGetValue(s, out dependentList);

                        //then check to see if t is contained in the list
                        //if contained then remove the string t from the list
                        if (dependentList.Contains(t))
                        {
                            dependentList.Remove(t);
                            Dependents.Remove(s);
                            Dependents.Add(s, dependentList);
                            removedDependency = true;
                        }
                    }

                    //If Dependees contains key t
                    if(Dependees.ContainsKey(t))
                    {
                        //Get Dependees of string t
                        Dependees.TryGetValue(t, out dependeeList);

                        //If the list contains the string s
                        //then remove s from the list
                        if (dependeeList.Contains(s))
                        {
                            dependeeList.Remove(s);
                            Dependees.Remove(t);
                            Dependees.Add(t, dependeeList);
                            removedDependency = true;
                        }
                    }

                    //if removedDependency is true decrement size by 1
                    if (removedDependency)
                    {
                        size--;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (s != null)
            {
                //if Depeendents contains key s then...
                if (Dependents.ContainsKey(s))
                {
                    ///create two structures one for the new dependent list
                    //and one for the size of the old dependent list
                    HashSet<String> dependentList = new HashSet<String>();

                    HashSet<String> OldDependentList = new HashSet<String>();

                    //get values of the old list
                    Dependents.TryGetValue(s, out OldDependentList);
                    //get the size and decrement by that much
                    size -= OldDependentList.Count;

                    //for each dependent in the new dependents
                    foreach(string dependent in newDependents)
                    {
                        if(dependent != null)
                        {
                            //if dependent list doesnt not contain the dependent
                            //add it to the list
                            if(!dependentList.Contains(dependent))
                            {
                                dependentList.Add(dependent);
                            }
                        }
                    }

                    //remove s from the Dependents list then
                    //add it back to the list with the new
                    //dependents and increment the size
                    Dependents.Remove(s);

                    Dependents.Add(s, dependentList);

                    size += dependentList.Count;   
                }
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            if(t != null)
            { 
                if(Dependees.ContainsKey(t))
                {
                    //create two structures one for the new dependent list
                    //and one for the size of the old dependent list
                    HashSet<String> dependeeList = new HashSet<String>();

                    HashSet<String> OldDependeeList = new HashSet<String>();

                    //get values of the old list
                    Dependees.TryGetValue(t, out OldDependeeList);

                    //get the size and decrement by that much
                    size -= OldDependeeList.Count;

                    //for each dependee in the new dependees
                    foreach (string dependee in newDependees)
                    {
                        if(dependee != null)
                        {
                            //if dependee list doesnt not contain the dependee
                            //add it to the list
                            if (!dependeeList.Contains(dependee))
                            {
                                dependeeList.Add(dependee);
                            }
                        }
                    }

                    //remove s from the Dependents list then
                    //add it back to the list with the new
                    //dependents and increment the size
                    Dependees.Remove(t);

                    Dependees.Add(t, dependeeList);

                    size += dependeeList.Count;
                }
            }
        }
    }
}
