using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Edge;

namespace $safeprojectname$
{
    [TestClass]
    public class EdgeDriverTest
    {
        // In order to run the below test(s), 
        // download and install Microsoft WebDriver from https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/,
        // then add the folder containing the installed MircrosoftWebDriver.exe to your PATH enviornment variable 
        // and launch MircrosoftWebDriver.exe from a command prompt

        private EdgeDriver _driver;

        [TestInitialize]
        public void EdgeDriverInitialize()
        {
           // Initialize edge driver 
            _driver = new EdgeDriver();
        }

        [TestMethod]
        public void VerifyPageTitle()
        {
           // Replace with your own test logic
            _driver.Url = "https://www.bing.com";
            Assert.AreEqual("Bing", _driver.Title);
        }

        [TestCleanup]
        public void EdgeDriverCleanup()
        {
            _driver.Quit();
        }
    }
}
