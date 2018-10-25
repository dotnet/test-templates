using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Edge;

namespace $safeprojectname$
{
    [TestClass]
    public class EdgeDriverTest
    {
        // In order to use web driver of Edge, it is required to download and install the Edge Web Driver from
        // https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/
        // Then add it to your PATH enviroment variable and run it at the background.

        EdgeDriver _driver;

        [TestInitialize]
        public void EdgeDriverInitialize()
        {
           // Initialize edge driver 
            _driver = new EdgeDriver();
        }

        [TestMethod]
        public void VerifyPageTitle()
        {
            // Add your own test logic
            _driver.Url = "https://www.bing.com";
            Assert.AreEqual("Bing", Driver.Title);
        }

        [TestCleanup]
        public void EdgeDriverCleanup()
        {
            _driver.Quit();
        }
    }
}
