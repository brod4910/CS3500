using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;


namespace DependencyGraphTest
{
    [TestClass]
    public class DependencyGraphTests
    {
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
    }
}
