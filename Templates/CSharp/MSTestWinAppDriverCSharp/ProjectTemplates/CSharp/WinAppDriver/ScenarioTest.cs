using Microsoft.VisualStudio.TestTools.UnitTesting;

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
