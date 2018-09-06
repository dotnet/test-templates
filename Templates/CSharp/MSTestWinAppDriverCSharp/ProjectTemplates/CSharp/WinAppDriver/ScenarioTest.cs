using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Threading;
using System;

namespace $safeprojectname$
{
    [TestClass]
    public class ScenarioTest : TestSession
    {
        [TestMethod]
        public void Test()
        {
            // Add the test logic
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TearDown();
        }
    }
}
