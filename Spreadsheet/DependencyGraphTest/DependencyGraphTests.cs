using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;

namespace DependencyGraphTest
{
    [TestClass]
    public class DependencyGraphTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            DependencyGraph DP = new DependencyGraph();

            DP.AddDependency("s", "a");
            DP.AddDependency("s", "a");

            DP.AddDependency("s", "b");
            DP.AddDependency("s", "c");

            DP.AddDependency("s", "d");
            DP.AddDependency("s", "d");

            DP.AddDependency("d", "s");
            DP.AddDependency("a", "s");

            DP.AddDependency("qwrwgrefa", "sac");

        }
    }
}
