﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;

namespace AutomatedVisualTesting.Utilities
{
    public static class SeleniumDriver
    {
        public enum Browser
        {
            Chrome,
            IE,
            FireFox
        }

        private static IWebDriver _driver;

        /// <summary>
        ///     Set WebDriver browser
        /// </summary>
        /// <param name="browser">Browser to use</param>
        private static void SetDriver(Browser browser)
        {
            switch (browser)
            {
                case Browser.IE:
                    _driver = new InternetExplorerDriver();
                    break;
                case Browser.FireFox:
                    _driver = new FirefoxDriver();
                    break;
                default:
                    _driver = new ChromeDriver();
                    break;
            }
        }

        /// <summary>
        ///     Load given url and wait for page to load
        /// </summary>
        /// <param name="url">Url to navigate to</param>
        private static void LoadUrl(string url)
        {
            _driver.Navigate().GoToUrl(url);
            WaitForLoad(_driver);
        }

        /// <summary>
        ///     Wait for page to load
        /// </summary>
        /// <param name="driver">web driver</param>
        /// <param name="timeoutSec">Time to wait</param>
        public static void WaitForLoad(this IWebDriver driver, int timeoutSec = 60)
        {
            var js = (IJavaScriptExecutor) driver;
            var wait = new WebDriverWait(driver, new TimeSpan(0, 0, timeoutSec));
            wait.Until(wd => js.ExecuteScript("return document.readyState").ToString() == "complete");
        }

        /// <summary>
        ///     Save screenshot of page loaded from url to Screenshots folder in
        ///     project using specified web driver and using page Title as filename
        /// </summary>
        /// <param name="url">Webpage to navigate to</param>
        /// <param name="browser">web browser to use</param>
        public static void SaveScreenShotByUrl(string url, Browser browser = Browser.Chrome)
        {
            SetDriver(browser);
            LoadUrl(url);

            var ss = ((ITakesScreenshot) _driver).GetScreenshot();
            // TODO: Stick directory in a setting
            var fileDirectory = "../../TestData/";
            if (!Directory.Exists(fileDirectory))
            {
                // screenshot directory doesn't exist
                _driver.Quit();
                throw new IOException("Please check screenshots folder exists within test solution to save screenshots");
            }

            string fileName = $"{fileDirectory}{browser}.png";
            ss.SaveAsFile(fileName, ImageFormat.Png);

            _driver.Quit();
        }

        /// <summary>
        ///     Save screenshot of element on page loaded from url to Screenshots folder in
        ///     project using specified web driver and using page Title as filename
        /// </summary>
        /// <param name="url">Webpage to navigate to</param>
        /// <param name="elementSelector">Element to take snapshot of</param>
        /// <param name="browser">Web Browser to use</param>
        public static void SaveElementScreenShotByUrl(string url, string elementSelector,
            Browser browser = Browser.Chrome)
        {
            SetDriver(browser);
            LoadUrl(url);

            //find the element! - 
            //TODO: Add error catching and magic selector so can specify id, tag or css and it will find first instance? If more than one instance take photos of each!?
            var element = _driver.FindElement(By.CssSelector(elementSelector));
            var byteArray = ((ITakesScreenshot) _driver).GetScreenshot().AsByteArray;
            var screenshot = new Bitmap(new MemoryStream(byteArray));
            var croppedImage = new Rectangle(element.Location.X, element.Location.Y, element.Size.Width,
                element.Size.Height);
            screenshot = screenshot.Clone(croppedImage, screenshot.PixelFormat);

            // TODO: Stick directory in a setting
            var fileDirectory = "../../TestData/";
            if (!Directory.Exists(fileDirectory))
            {
                // screenshot directory doesn't exist
                _driver.Quit();
                throw new IOException("Please check screenshots folder exists within test solution to save screenshots");
            }
            string fileName = $"{fileDirectory}{browser}.png";
            screenshot.Save(fileName, ImageFormat.Png);
            _driver.Quit();
        }

        /// <summary>
        ///     Create image of website for the given url
        /// </summary>
        /// <param name="url">Url to take an image of</param>
        /// <param name="elementSelector">Selector to find element</param>
        /// <param name="browser">Web Browser</param>
        /// <returns></returns>
        public static byte[] GetScreenshotByUrl(string url, string elementSelector, Browser browser = Browser.Chrome)
        {
            SetDriver(browser);
            LoadUrl(url);

            //find the element! - TODO: Add error catching and magic selector so can specify id, tag or css and it will find first instance? If more than one instance take photos of each!?
            var element = _driver.FindElement(By.CssSelector(elementSelector));
            var byteArray = ((ITakesScreenshot) _driver).GetScreenshot().AsByteArray;
            var screenshot = new Bitmap(new MemoryStream(byteArray));
            var croppedImage = new Rectangle(element.Location.X, element.Location.Y, element.Size.Width,
                element.Size.Height);
            screenshot = screenshot.Clone(croppedImage, screenshot.PixelFormat);
            var bytes = ImageToByte(screenshot);
            _driver.Quit();

            return bytes;
        }

        /// <summary>
        ///     Returns image as byte array
        /// </summary>
        /// <param name="img">Image to return as Byte array</param>
        /// <returns></returns>
        public static byte[] ImageToByte(Image img)
        {
            var converter = new ImageConverter();
            return (byte[]) converter.ConvertTo(img, typeof(byte[]));
        }

        /// <summary>
        ///     Create image of website for the given url
        /// </summary>
        /// <param name="url">Url to take an image of</param>
        /// <param name="browser">Web Browser</param>
        /// <returns></returns>
        public static byte[] GetScreenshotByUrl(string url, Browser browser = Browser.Chrome)
        {
            SetDriver(browser);
            LoadUrl(url);

            var ss = ((ITakesScreenshot) _driver).GetScreenshot();
            var screenshot = ss.AsBase64EncodedString;
            var bytes = Convert.FromBase64String(screenshot);
            _driver.Quit();

            return bytes;
        }
    }
}