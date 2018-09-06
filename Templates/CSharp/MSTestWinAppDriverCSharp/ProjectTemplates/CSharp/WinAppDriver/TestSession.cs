using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using System;

namespace $safeprojectname$
{
    // In order to run test, please download and install Win App Driver from https://github.com/Microsoft/WinAppDriver/releases.
    // Then run it in background, default it listens to 127.0.0.1:4723
    // For more information, see https://github.com/Microsoft/WinAppDriver
    public class TestSession
    {
        // Win App Driver runs on localhost:4723 by default
        protected const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        // Provide the app id of your application
        private const string ApplicationID = @"Your Application ID";

        protected static WindowsDriver<WindowsElement> Session;
        protected static WindowsElement TestElement;

        public static void Setup(TestContext context)
        {
            if (Session == null)
            {
                // Create a new Session to launch application
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", ApplicationID);
                Session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
                Assert.IsNotNull(Session);
                Assert.IsNotNull(Session.SessionId);

                // Provide the name of test element to track it through test
                TestElement = Session.FindElementByClassName("");
                Assert.IsNotNull(TestElement);
            }
        }

        public static void TearDown()
        {
            // Close the application and delete the Session
            if (Session != null)
            {
                Session.Close();
                Session.Quit();
                Session = null;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize the test element
        }
    }
}