using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;


namespace DependencyGraphTest
{
    /// <summary>
    /// Uses a plethora of tests to test against the implemination of 
    /// the Dependency Graph class
    /// </summary>
    [TestClass]
    public class DependencyGraphTests
    {
        /// <summary>
        /// Tests to check if the strings has Dependees
        /// </summary>
        [TestMethod]
        
        public void TestHasDependees()
        {
            DependencyGraph DG = new DependencyGraph();
            
            DG.AddDependency("a", "b");
            DG.AddDependency("a", "c");
            DG.AddDependency("b", "d");
            DG.AddDependency("d", "d");

            Assert.IsFalse(DG.HasDependees("s"));
            Assert.IsFalse(DG.HasDependees("a"));
            Assert.IsTrue(DG.HasDependees("b"));
            Assert.IsTrue(DG.HasDependees("c"));
            Assert.IsTrue(DG.HasDependees("d"));

        }

        /// <summary>
        /// Tests to check if a strings has Dependents
        /// </summary>
        [TestMethod]
        public void TestHasDependents()
        {
            DependencyGraph DG = new DependencyGraph();

            DG.AddDependency("a", "b");
            DG.AddDependency("a", "c");
            DG.AddDependency("b", "d");
            DG.AddDependency("d", "d");

            Assert.IsFalse(DG.HasDependents("c"));
            Assert.IsTrue(DG.HasDependents("a"));
            Assert.IsTrue(DG.HasDependents("b"));
            Assert.IsTrue(DG.HasDependents("d"));
        }

        /// <summary>
        /// Tests to check if the strings has no Dependees
        /// </summary>
        [TestMethod]

        public void TestHasNoDependees()
        {
            DependencyGraph DG = new DependencyGraph();

            DG.AddDependency("a", "b");
            DG.AddDependency("a", "c");
            DG.AddDependency("b", "d");
            DG.AddDependency("d", "d");

            Assert.IsFalse(DG.HasDependees("q"));
            Assert.IsFalse(DG.HasDependees("12"));
            Assert.IsFalse(DG.HasDependees("b345"));
            Assert.IsFalse(DG.HasDependees("c4124"));
            Assert.IsFalse(DG.HasDependees("123123"));

        }

        /// <summary>
        /// Tests to check if a strings has no Dependents
        /// </summary>
        [TestMethod]
        public void TestHasNoDependents()
        {
            DependencyGraph DG = new DependencyGraph();

            DG.AddDependency("a", "b");
            DG.AddDependency("a", "c");
            DG.AddDependency("b", "d");
            DG.AddDependency("d", "d");

            Assert.IsFalse(DG.HasDependents("c565"));
            Assert.IsFalse(DG.HasDependents("a123123"));
            Assert.IsFalse(DG.HasDependents("b123123"));
            Assert.IsFalse(DG.HasDependents("d5345345"));
        }

        /// <summary>
        /// Tests to check if a null input has Dependees/Dependents
        /// </summary>
        [TestMethod]
        public void TestHasDependentsandDependeeswithNull()
        {
            DependencyGraph DG = new DependencyGraph();

            DG.AddDependency("a", "b");
            DG.AddDependency("a", "c");
            DG.AddDependency("b", "d");
            DG.AddDependency("d", "d");

            Assert.IsFalse(DG.HasDependents(null));
            Assert.IsFalse(DG.HasDependents(null));
            Assert.IsFalse(DG.HasDependees(null));
            Assert.IsFalse(DG.HasDependees(null));
        }

        /// <summary>
        /// Tests against a small set of Dependencies to see if the
        /// dependees exist or not
        /// </summary>
        [TestMethod]
        public void TestGetDependees()
        {
            DependencyGraph DG = new DependencyGraph();

            DG.AddDependency("a", "b");
            DG.AddDependency("a", "c");
            DG.AddDependency("b", "d");
            DG.AddDependency("d", "d");

            IEnumerable<String> DGE = DG.GetDependees("d");

            List<String> DP = new List<string>();

            DP.Add("b");
            DP.Add("d");

            foreach(string dependee in DGE)
            {
                Assert.IsTrue(DP.Contains(dependee));
            }

            DG.RemoveDependency("d", "d");

            DGE = DG.GetDependees("d");

            DP.Remove("d");

            foreach(string dependee in DGE)
            {
                Assert.IsTrue(DP.Contains(dependee));
            }
        }

        /// <summary>
        /// Removes a small set of dependencies
        /// </summary>
        [TestMethod]
        public void testRemoveDependency()
        {
            DependencyGraph DG = new DependencyGraph();

            DG.AddDependency("a", "b");
            DG.AddDependency("a", "c");
            DG.AddDependency("b", "d");
            DG.AddDependency("d", "d");

            IEnumerable<String> DGE = DG.GetDependents("a");

            List<String> DP = new List<string>();

            DP.Add("b");
            DP.Add("c");

            DG.RemoveDependency("a", "d");

            foreach(string dependent in DGE)
            {
                Assert.IsTrue(DP.Contains(dependent));
            }
            
        }

        /// <summary>
        /// Attempt to remove a dependency that does not exist
        /// </summary>
        [TestMethod]
        public void testRemoveDependencywithNoDependents()
        {
            DependencyGraph DG = new DependencyGraph();

            DG.AddDependency("a", "b");
            DG.AddDependency("a", "c");
            DG.AddDependency("b", "d");
            DG.AddDependency("d", "d");

            DG.RemoveDependency("p", "d");

            Assert.IsTrue(DG.Size == 4);
        }

        /// <summary>
        /// Attempt to add a dependency that is alreadt contained in the graph
        /// and checks size after
        /// </summary>
        [TestMethod]
        public void testAddDependencythatExistsinGraph()
        {
            DependencyGraph DG = new DependencyGraph();

            DG.AddDependency("a", "b");
            DG.AddDependency("a", "c");
            DG.AddDependency("b", "d");
            DG.AddDependency("d", "d");

            DG.AddDependency("d", "d");

            IEnumerable<String> DGE = DG.GetDependents("d");

            foreach(string dependent in DGE)
            {
                Assert.AreEqual(dependent, "d");
            }

            Assert.IsTrue(DG.Size == 4);
        }

        /// <summary>
        /// Replaces a singular items in the list of dependees that was
        /// given to the specified string
        /// </summary>
        [TestMethod]
        public void testReplaceDependees()
        {
            DependencyGraph DG = new DependencyGraph();

            for (int i = 1; i <= 100000; i++)
            {
                DG.AddDependency("a", i + "");
            }

            List<String> DP = new List<string>();

            DP.Add("P");

            DG.ReplaceDependees("20000", DP);

            IEnumerable<String> DGE = DG.GetDependees("20000");

            foreach (string dependee in DGE)
            {
                Assert.IsTrue(dependee.Equals("P"));
            }

            Assert.AreEqual(100000, DG.Size);
        }

        /// <summary>
        /// Uses a singular string "a" giving it multiple dependencies
        /// and replacing those dependencies with a smaller set of ints
        /// </summary>
        [TestMethod]
        public void testReplaceDependents()
        {
            DependencyGraph DG = new DependencyGraph();

            for (int i = 1; i <= 100000; i++)
            {
                DG.AddDependency("a", i + "");
            }

            List<String> DP = new List<string>();

            for (int i = 1; i <= 10000; i++)
            {
                DP.Add(i + "");
            }

            DG.ReplaceDependents("a", DP);

            IEnumerable<String> DGE = DG.GetDependents("a");

            foreach(string dependent in DGE)
            {
                Assert.IsTrue(DP.Contains(dependent));
            }

            Assert.AreEqual(10000, DG.Size);
        }

        /// <summary>
        /// Attempts to add null items into the Dependency Graph
        /// </summary>
        [TestMethod]
        public void testNullInputsAddDependecy()
        {
            DependencyGraph DG = new DependencyGraph();

            DG.AddDependency(null, null);

            Assert.AreEqual(0, DG.Size);

            DG.AddDependency(null, "ert");

            Assert.AreEqual(0, DG.Size);

            DG.AddDependency("ert", null);

            Assert.AreEqual(0, DG.Size);
        }

        /// <summary>
        /// Attempts to remove items in the Dependency Graph but null inputs
        /// should not affect the DP.
        /// </summary>
        [TestMethod]
        public void testNullInputRemoveDependency()
        {
            DependencyGraph DG = new DependencyGraph();

            DG.RemoveDependency(null, null);

            Assert.AreEqual(0, DG.Size);

            DG.RemoveDependency(null, "ert");

            Assert.AreEqual(0, DG.Size);

            DG.RemoveDependency("ert", null);

            Assert.AreEqual(0, DG.Size);
        }

        /// <summary>
        /// Uses a List of nulls to Test if replacing calling ReplaceDependents
        /// has no affect on the DependencyGraph.
        /// </summary>
        [TestMethod]
        public void testNullInputReplaceDependents()
        {
            DependencyGraph DG = new DependencyGraph();

            for(int i = 1; i <= 10000;i++)
            {
                DG.AddDependency(i + "", i * 2 + "");
            }

            List<String> ReplaceDependents = new List<String>();

            List<String> OldDependents = new List<String>();


            IEnumerable<String> DBE = DG.GetDependents("9500");

            foreach(string dependents in DBE)
            {
                OldDependents.Add(dependents);
            }

            for(int i = 1; i <= 10000; i *= 2)
            {
                ReplaceDependents.Add(null);
            }

            DG.ReplaceDependents("9500", ReplaceDependents);

            IEnumerable<String> DGE = DG.GetDependents("9500");

            foreach(string dependent in DGE)
            {
                Assert.IsTrue(OldDependents.Contains(dependent));
            }
        }

        /// <summary>
        /// Uses a List of nulls to Test if replacing calling ReplaceDependees
        /// has no affect on the DependencyGraph.
        /// </summary>
        [TestMethod]
        public void testNullInputReplaceDependees()
        {
            DependencyGraph DG = new DependencyGraph();

            for (int i = 1; i <= 10000; i++)
            {
                DG.AddDependency(i + "", i * 2 + "");
            }

            List<String> ReplaceDependees = new List<String>();

            List<String> OldDependeess = new List<String>();


            IEnumerable<String> DBE = DG.GetDependees("19000");

            foreach (string dependee in DBE)
            {
                OldDependeess.Add(dependee);
            }

            for (int i = 1; i <= 10000; i *= 2)
            {
                ReplaceDependees.Add(null);
            }

            DG.ReplaceDependents("9500", ReplaceDependees);

            IEnumerable<String> DGE = DG.GetDependents("9500");

            foreach (string dependee in DGE)
            {
                Assert.IsTrue(OldDependeess.Contains(dependee));
            }
        }
    }
}
