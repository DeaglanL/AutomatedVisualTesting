using System.Drawing;
using AutomatedVisualTesting.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static System.Int32;
using static System.Configuration.ConfigurationSettings;

namespace AutomatedVisualTesting
{
    [TestClass]
    public class ExampleTests
    {
        private static IWebDriver _driver;

        [TestInitialize]
        public void Startup()
        {
            // Create new Chrome WebDriver
            _driver = new ChromeDriver();

            // Get driver height/width from app.config
            var driverWidth = Parse(AppSettings.Get("DriverWidth"));
            var driverHeight = Parse(AppSettings.Get("DriverHeight"));

            // Set driver height/width of window
            _driver.Manage().Window.Size = new Size(driverWidth, driverHeight);

            // Navigate to url for testing
            _driver.Navigate().GoToUrl("http://computer-database.herokuapp.com/computers");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // ensure we close down the Chrome WebDriver
            _driver.Quit();
        }

        [TestMethod]
        public void NoDifferenceBetweenImageAndScreenshotFromUrl()
        {
            // Arrange
            var baseImage = "HomePage.png";

            // Act
            var difference = Compare.GetDifference(_driver, baseImage);

            // Assert
            Assert.IsTrue(difference == 0);
        }

        [TestMethod]
        public void NoDifferenceBetweenElementImageAndScreenshotFromUrl()
        {
            // Arrange
            var baseImage = "TableElement.png";
            var elementByCssSelector = ".computers";

            // Act
            var difference = Compare.GetDifference(_driver, baseImage, elementByCssSelector);

            // Assert
            Assert.IsTrue(difference == 0);
        }

        [TestMethod]
        public void NoDifferenceBetweenImageWithDynamicTableAndScreenshotFromUrl()
        {
            // Arrange
            var baseImage = "HomePageCoveringDynamicElement.png";
            var elementByCssSelector = ".computers";

            // Cover specified dynamic element on page with blanket
            SeleniumDriver.CoverDynamicElementBySelector(_driver, elementByCssSelector);

            // Act
            var difference = Compare.GetDifference(_driver, baseImage);

            // Assert
            Assert.IsTrue(difference == 0);
        }
    }
}