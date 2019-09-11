using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using Automation.Core;
using Automation.Core.Logging;
using System.Net;
using WindowsInput;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using Automation.Core.Infrastructure;
using Automation.Core.Exceptions;



namespace Automation.Core.Infrastructure
{
    public static class Extensions
    {

        public static string driverBrowser = "";
        
        /// <summary>
        /// Hover on element
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        public static void HowerOnElement(this IWebDriver driver, By selector)
        {
            Actions builder = new Actions(driver);
            IWebElement element = driver.FindElement(selector);
            builder.MoveToElement(element)
            .Build().Perform();

        }

        public static void HowerOnElement(this IWebDriver driver, IWebElement element)
        {
            Actions builder = new Actions(driver);            
            builder.MoveToElement(element)
            .Build().Perform();

        }

        public static void OpenNewWindowByUrlAndFocus(this IWebDriver driver, string url)
        {
           // bool switchOutofHandles = true;
            System.Threading.Thread.Sleep(3000);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.open('" + url + "','_blank');");
            IList<string> windowHandlesEnd = driver.WindowHandles;
            System.Threading.Thread.Sleep(3000);

            foreach (string windowHandle in windowHandlesEnd)
            {
                if (driver.SwitchTo().Window(windowHandle).Url.Contains(url))
                {
                  //  switchOutofHandles = false;
                    break;
                }
            }

           
        }

        public static void UpdateBrowserName(this IWebDriver driver, string browser)
        {
            driverBrowser = browser;
        }

        public static string GetBrowserName(this IWebDriver driver)
        {
            return driverBrowser;
        }
        /// <summary>
        /// Wait for element in 80 sec and return the element itslef
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="element"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
                      
        public static IWebElement WaitAndReturnElement(this IWebDriver driver, IWebElement element, int timeout = 80000)
        {
            int timer = 0;
            while (timer < timeout)
            {
                timer += 500;
                System.Threading.Thread.Sleep(500);
                try
                {

                    if (element.Displayed)
                        return element;
                }
                catch (NoSuchElementException)
                {
                    continue;
                }
                catch (WebDriverTimeoutException e)
                {
                    Assert.Fail("no such element " + element.ToString() + e.Message);
                }
                catch (StaleElementReferenceException)
                {

                    continue;
                }

                catch (Exception e)
                {
                    continue;                    
                }
            }
            return null;
        }
              
        /// <summary>
        /// Wait for Frame to be loaded in page
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="frame"></param>
        /// <param name="iterator"></param>
        /// <returns></returns>
        public static void SwtichToFrame(this IWebDriver driver, string frame, int iterator = 70)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Random number = new Random();
            int choosenNum = number.Next(100, 200);
            int countdown = iterator;

            while (countdown > 0)
            {
                try
                {
                    driver.SwitchTo().Frame(frame);
                    driver.WaitForPageToLoad();
                    return;
                }
                catch (NoSuchFrameException)
                {
                    System.Threading.Thread.Sleep(1000);
                    countdown--;
                }
                catch (Exception)
                {
                    break;
                }
            }
            stopwatch.Stop();
            Log.WriteMessage("The frame :" + frame + " was not found or not interactive in page, waiting " + stopwatch.Elapsed + " seconds - test stopped ");
            Assert.Fail("The frame :" + frame + " was not found or not interactive in page, waiting " + stopwatch.Elapsed + " seconds - test stopped ");
            throw new FrameNotFoundException("The following Frame was not found " + frame);
            

        }
       
        /// <summary>
        /// Clear all Flash Cookies
        /// </summary>
        /// <param name="coockiejar"></param>
        public static void ClearFlashCoockiesChrome(this ICookieJar coockiejar, string browser)
        {
            string flashCoockiesLocation;
            if (browser.Equals("Chrome"))
            {
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                appdata = appdata.Replace("Roaming", "");
                flashCoockiesLocation = Path.Combine(appdata, @"Local\Google\Chrome\User Data\Default\Pepper Data\Shockwave Flash\WritableRoot\#SharedObjects");
            }
            else
            {
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                flashCoockiesLocation = Path.Combine(appdata, @"Macromedia\Flash Player\#SharedObjects");
            }

            clearFolder(flashCoockiesLocation);

        }

        private static void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }
        /// <summary>
        /// Clear and delete flash cookies
        /// </summary>
        /// <param name="coockiejar"></param>
        public static void ClearFlashCoockies(this ICookieJar coockiejar)
        {
            try
            {

                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string flashCoockiesLocation = Path.Combine(appdata, @"Macromedia\Flash Player\#SharedObjects");
                if (!Directory.Exists(flashCoockiesLocation))
                    return;
                List<string> files = Directory.GetFiles(flashCoockiesLocation, "*.*", SearchOption.AllDirectories).ToList();

                foreach (string item in files)
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception)
                    {
                        //throw;
                        continue;
                    }
                }
            }
            catch (System.IO.DirectoryNotFoundException)
            {                                
            }
        }
        
        public static void ClearChromeCookies()
        {

            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appdata = appdata.Replace("Roaming", "");
            string CoockiesLocation = Path.Combine(appdata, @"Local\Google\Chrome\User Data\Default\Local Storage");

            List<string> files = Directory.GetFiles(CoockiesLocation, "*.*", SearchOption.AllDirectories).ToList();
            foreach (string item in files)
            {
                try
                {
                    File.Delete(item);
                }
                catch (Exception)
                {
                    //throw;
                    continue;
                }
            }


        }
       
        
        /// <summary>
        /// Get Web Driver capabilities
        /// </summary>
        /// <param name="driver"></param>
        /// <returns>the capabilities of the web driver type</returns>
        public static ICapabilities Capabilities(this IWebDriver driver)
        {
            if (driver is RemoteWebDriver)
            {
                return ((RemoteWebDriver)driver).Capabilities;
            }

            if (driver is FirefoxDriver)
            {
                return ((FirefoxDriver)driver).Capabilities;
            }

            if (driver is InternetExplorerDriver)
            {
                return ((InternetExplorerDriver)driver).Capabilities;
            }

            if (driver is ChromeDriver)
            {
                return ((ChromeDriver)driver).Capabilities;
            }

            return null;
        }
        /// <summary>
        /// get the element id if exists
        /// </summary>
        public static string ID(this IWebElement element)
        {
            return element.GetAttribute("id");
        }

        public static bool IsAlertPresent(this IWebDriver driver)
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }
  
       
        /// <summary>
        /// Force click by Java Scripts with element location, for isseus of IE11 that sometimes the click is not performed
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        public static void ForceClick(this IWebDriver driver, By by)
        {
            string browser = driver.GetBrowserName();
            // get current frame element
            driver.WaitForPageToLoad();
            if (!browser.Equals("Safari"))
            {
                IWebElement elmnt = driver.FindElement(by);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", elmnt);
                //driver.SwitchTo().Frame(el);
                driver.WaitForPageToLoad();
            }
            else
            {
                driver.FindElement(by).Click();
                driver.WaitForPageToLoad();
            }
        }
              
        /// <summary>
        /// Pass frame ID and Element by.Id
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="iFrameId"></param>
        /// <param name="elementId"></param>
        public static void ClickOnIframeElement(this IWebDriver driver, string iFrameId, string elementId)
        {
            Log.WriteMessage("ClickOnIframeElement() with javascript execution");
            driver.SwitchTo().DefaultContent();
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            try
            {
                js.ExecuteScript("window.frames['" + iFrameId + "'].document.getElementById('" + elementId + "').click();");
            }
            catch (Exception e)
            {
                Thread.Sleep(3000);
                Log.WriteMessage("ClickOnIframeElement() with javascript execution second ettempt " + e.Message);
                js.ExecuteScript("jQuery('#" + iFrameId + "').contents().find('#" + elementId + "').click();");
            }


        }

        public static void ForceClick(this IWebDriver driver, IWebElement element)
        {
            if (!(driverBrowser.Equals("Safari")))
            {
                // get current frame element
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", driver.WaitAndReturnElement(element));
                //driver.SwitchTo().Frame(el);
            }
            else
            element.Click();
        }

        public static void ForceJqueryClick(this IWebDriver driver, string cssSelector = "input#btn_submit")
        {

            ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector('"+cssSelector+"').click();");

          
        }

        public static void ClickJq(this IWebDriver driver,string jqueryLocator)
        {

            Log.WriteMessage("Click Cancel ");
            driver.WaitForPageToLoad();
            ((IJavaScriptExecutor)driver).ExecuteScript("$('"+jqueryLocator+"').click()");
            //  return new OpenningPageMbl(driver);        

            // get current frame element
            //   ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", driver.WaitAndReturnElement(element));
            //driver.SwitchTo().Frame(el);

        }
                   
        public static void WaitForPageToLoad(this IWebDriver driver, int timeOut=70000)
        {           
          
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string readyState = string.Empty;

            IJavaScriptExecutor javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
                throw new ArgumentException("driver", " Driver must support javascript execution");
            
            do
            {
                
                try
                {                 
                    readyState = javascript.ExecuteScript(
                      "return document.readyState").ToString();
                   
                }
                catch (InvalidOperationException e)
                {                    
                    break;
                }
                catch (WebDriverException)
                {             
                }
                catch (Exception)
                {
                }

            }
            while ( (stopwatch.ElapsedMilliseconds <= timeOut) && (readyState.ToLower() != "complete") );

            //try
            //{
            //    javascript.ExecuteScript("$('#fsrOverlay').html('')");
            //}
            //catch (Exception)
            //{

            //}                                                

        }

        public static void WaitForAjaxLoadingEnds(this IWebDriver driver)
        {
            try
            {
                while (true) // Handle timeout somewhere
                {
                    var ajaxIsComplete = (bool)(driver as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0");
                    if (ajaxIsComplete)
                        break;
                    Thread.Sleep(200);
                }
            }
            catch (Exception)
            {
               
            }    
        }

        /// <summary>
        /// Manual wait - not by Selenium package, wait in loop with try and catch till success
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <param name="iterator"></param>
        /// <returns></returns>
        public static IWebElement WaitForElement(this IWebDriver driver, By by, int iterator = 45)
        {
            Random number = new Random();

            int choosenNum = number.Next(1, 99);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int countdown = iterator;
            while (countdown > 0)
            {
                System.Threading.Thread.Sleep(500);

                try
                {
                    IWebElement element = driver.FindElement(by);
                    if (element.Displayed)
                        return element;
                    else countdown--;
                    
                }
                catch (StaleElementReferenceException)
                {
                    countdown--;
                }
                catch (NoSuchElementException)
                {
                    countdown--;
                }
                catch (Exception e)
                {
                    Log.WriteError("wait for element func: exception throw : ", e);
                    countdown--;
                }
            }

            stopwatch.Stop();
            
            return null;

        }

        public static IWebElement WaitForElementAjax(this IWebDriver driver, By by,double timeSpan=20)
        {
            //WaitForAjaxLoadingEnds(driver);
            try
            {
                IWebElement element = (new WebDriverWait(driver, TimeSpan.FromSeconds(timeSpan))).Until(ExpectedConditions.ElementIsVisible(by)); //.PresenceOfAllElementsLocatedBy(by));
                //IList<IWebElement> 
                //var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeSpan));
                //IWebElement element = wait.Until(d => driver.FindElement(by));
                //return element[0];
                return element;
            }
            catch (WebDriverTimeoutException) { IWebElement elem = null; return elem; }
            //    IList<IWebElement> element = (new WebDriverWait(m_webRunner.m_driver, TimeSpan.FromSeconds(120))).Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(Fantasy5RequiredOperation[0].myby));

        }

        public static IList<IWebElement> WaitForElementsAjax(this IWebDriver driver, By by, double timeSpan = 20)
        {
            //WaitForAjaxLoadingEnds(driver);
            try
            {
                IList<IWebElement> element = (new WebDriverWait(driver, TimeSpan.FromSeconds(timeSpan))).Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(by));
                //var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeSpan));
                //IWebElement element = wait.Until(d => driver.FindElement(by));
                return element;
            }
            catch (WebDriverTimeoutException) { IList<IWebElement> elem = null; return elem; }
            //    IList<IWebElement> element = (new WebDriverWait(m_webRunner.m_driver, TimeSpan.FromSeconds(120))).Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(Fantasy5RequiredOperation[0].myby));

        
        }

        public static void SwitchToFrameIndex(this IWebDriver driver, int frame, int iterator = 20000)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Random number = new Random();
            int countdown = 0;

            while (countdown < iterator)
            {
                try
                {                 
                    driver.SwitchTo().Frame(frame);
                    return;
                }
                catch (NoSuchFrameException)
                {
                    System.Threading.Thread.Sleep(500);
                    countdown = countdown + 500;
                }
                catch (Exception)
                {
                    break;
                }
            }

            stopwatch.Stop();
        }

        public static void SwitchToFrameName(this IWebDriver driver, string frame, int iterator = 20000)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Random number = new Random();
            int countdown = 0;

            while (countdown < iterator)
            {
                try
                {                    
                    driver.SwitchTo().Frame(frame);
                    return;                                 
                }
                catch (NoSuchFrameException)
                {
                    System.Threading.Thread.Sleep(500);
                    countdown = countdown + 500;
                }
                catch (Exception)
                {
                    break;
                }
            }

            stopwatch.Stop();
        }
    }


}