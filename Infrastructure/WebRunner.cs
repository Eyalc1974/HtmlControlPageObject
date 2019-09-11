using Automation.Core.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Automation.Core.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Collections;
using WindowsInput;

namespace Automation.Core.Infrastructure
{
    public enum BrowserPlatform
    {
        IE,
        FireFox,
        Chrome,
        Safari
    }

    public enum Platform
    {
        Web = 0,
        Mobile_Android = 1, // Android
        Mobile_IOS = 2
    }

    public class WebRunner
    {
        public const string DEFAULT_FRAME = "DEFAULT";
        internal TestProfile _profile;
        private static WebRunner m_singleRunner;
        public Dictionary<int, string> m_RunnerActiveWindows = new Dictionary<int, string> { };
        private IList<string> m_HandlersInUsed = new List<string>(){};
        private static int numberOfWindowsOpen = 0;
        /// <summary>
        /// m_currentDriver is created for running two drivers at the same time, 
        /// the internal is only one driver in NG.Automation.Core.dll the we can use it inside dll
        /// </summary>
        public IWebDriver m_driver;
        public Platform Platform {get;set;}
     
        public BrowserPlatform CurrentBrowser
        {
            get
            {
                Log.WriteMessage("WebRunner: CurrentBrowser, browser of the current run is " + _profile.Browser.ToString(), true);
                return _profile.Browser;
            }
        }

        

        private string m_currentFrame = DEFAULT_FRAME;
        public string CurrentFrame
        {
            get { return m_currentFrame; }
            set 
            {
                //if (string.Compare(m_currentFrame, value, true) != 0) // && (!m_currentFrame.Equals("DEFAULT")
                if (!m_currentFrame.Contains("DEFAULT"))
                {
                    //Log.WriteMessage("WebRunner: CurrentFrame: frame is " + m_currentFrame, true);
                    m_currentFrame = value;
                    if (m_currentFrame.Contains("DEFAULT"))
                        m_driver.SwitchTo().DefaultContent();
                    else
                    {
                        m_driver.SwitchTo().DefaultContent();
                        m_driver.SwitchTo().Frame(m_currentFrame);
                        Log.WriteMessage("WebRunner: CurrentFrame: driver is switch to frame successfully :" + m_currentFrame, true);
                    }
                }
            }
        }

        public WebRunner(TestProfile profile, Platform platform = Platform.Web)
        {
            Platform = platform;
            if (profile == null)
                throw new ArgumentNullException("profile");
            _profile = profile;
            start();
            //SingleRunner = this;
        }

        public int NumberOfWindowsOpen
        {
            get
            {
                Log.WriteMessage("WebRunner: NumberOfWindowsOpen: is : " + m_RunnerActiveWindows.Count, true);
                return m_RunnerActiveWindows.Count;
            }

        }

        internal string GetWindowHandle(int hashNum)
        {
            // Case: if there is no hash-key that match to the new one...hashNum
            if (!m_RunnerActiveWindows.ContainsKey(hashNum))
            {
                foreach (var handel in m_driver.WindowHandles)
                { // if handel not in used then add it 
                    if (!(m_HandlersInUsed.Contains(handel)))
                    {
                        m_RunnerActiveWindows.Add(hashNum, handel);
                        m_HandlersInUsed.Add(handel);
                        Log.WriteMessage("WebRunner: GetWindowHandle: added new item ot dictionary, key-val : " + hashNum + " - " + handel, true);
                        numberOfWindowsOpen++;
                        return m_RunnerActiveWindows[hashNum].ToString();
                    }
                }
                m_RunnerActiveWindows.Add(hashNum, m_driver.WindowHandles.Last());
                return m_RunnerActiveWindows[hashNum].ToString(); 
                         
            }
            // in case hash exist then return the val 
            return m_RunnerActiveWindows[hashNum].ToString();


            #region
            //foreach (var handel in m_driver.WindowHandles)
            //{
            //    if (!(m_RunnerActiveWindows.ContainsValue(handel)) && !(m_RunnerActiveWindows.ContainsKey(hashNum)))
            //    {
            //        m_RunnerActiveWindows.Add(hashNum, handel);
            //        return m_RunnerActiveWindows[hashNum].ToString();
            //    }

            //    if (m_RunnerActiveWindows.ContainsValue(handel) && !(m_RunnerActiveWindows.ContainsKey(hashNum)))
            //    {
            //        m_RunnerActiveWindows.Add(hashNum, handel);
            //        return m_RunnerActiveWindows[hashNum].ToString();
            //    }
            //    else
            //    {
            //        if (((m_RunnerActiveWindows.ContainsKey(hashNum)) && (m_RunnerActiveWindows[hashNum].Contains(handel))))
            //        {
            //            return m_RunnerActiveWindows[hashNum].ToString();
            //        }

            //    }
                   
            // }

            //return m_RunnerActiveWindows[hashNum].ToString();
            #endregion
        }
        /// <summary>
        /// Function return handle of the previous window (second) or the first in case we don't have 3 windows open
        /// </summary>
        /// <param name="lastKey"></param>
        /// <returns></returns>
        public string GetPrevOpenWindowByKey(int lastKey)
        {
            string handler = string.Empty;
            int firstKey;

            var last = m_RunnerActiveWindows.Last();
            handler = last.Value;
            lastKey = last.Key;
            

            //int exists = m_RunnerActiveWindows.Keys.SingleOrDefault(k => k == lastKey);
            //if (exists == 0) //ToDo: change excepton type \ message
            //    throw new Exception("Cannot find key in the wondow distionary" + lastKey);

            int index = m_RunnerActiveWindows.Keys.ToList().IndexOf(lastKey);
            if (index != 0)
                index--;
            int prevKey = m_RunnerActiveWindows.Keys.ToList()[index];
            return m_RunnerActiveWindows[prevKey];
            //if(index == 0)
            //foreach (int item in m_RunnerActiveWindows.Keys)
            //{
            //    if (item != firstKey && item != lastKey)
            //        return m_RunnerActiveWindows[item];
            //}
            //return m_RunnerActiveWindows[firstKey];
        }

     
        private void start()
        {
            Log.WriteMessage("WebRunner: start() : init WebDriver ", true);
            Log.WriteMessage("WebRunner: start() : platform is: " + Platform.ToString(), true);
            if (Platform.Equals(Platform.Web))
            {
                switch (_profile.Browser)
                {
                    // Capabilities is for Grid only.
                    case BrowserPlatform.FireFox:
                        FirefoxProfile profile = new FirefoxProfile();
                        profile.AcceptUntrustedCertificates = true;
                        m_driver = new FirefoxDriver(profile);

                        // Marionette Code
                        //var driverService = FirefoxDriverService.CreateDefaultService();
                        //driverService.FirefoxBinaryPath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";                        
                        //driverService.SuppressInitialDiagnosticInformation = true;
                        //FirefoxOptions marionetOpt = new FirefoxOptions();
                        
                        //marionetOpt.AddAdditionalCapability("acceptSslCerts", true);
                        //marionetOpt.AddAdditionalCapability("secureTLS", true);
                        //marionetOpt.AddAdditionalCapability("accept_untrusted_certs", true);
                        //m_driver = new FirefoxDriver(driverService, marionetOpt, TimeSpan.FromSeconds(60));      
                        
                        break;
                    case BrowserPlatform.IE:
                        InternetExplorerOptions opt = new InternetExplorerOptions();
                        opt.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                        opt.EnableNativeEvents = false;                       
                        //opt.AddAdditionalCapability("INTRODUCE_FLAKINESS_BY_IGNORING_SECURITY_DOMAINS", true);                        
                        m_driver = new InternetExplorerDriver(opt);
                        break;
                    case BrowserPlatform.Chrome:
                        ChromeOptions co = new ChromeOptions();

                        co.AddArgument("--disable-extensions");
                        co.AddArgument("--ignore-certificate-errors");

                        m_driver = new ChromeDriver("C:\\SeleniumDrivers\\", co, TimeSpan.FromSeconds(60));
                        break;
                    case BrowserPlatform.Safari:
                        DesiredCapabilities capability = new DesiredCapabilities();
                        SafariOptions options = new SafariOptions();
                        options.AddAdditionalCapability("cleanSession", true);
                        options.AddAdditionalCapability("--ignore-certificate-errors", true);
                        options.AddAdditionalCapability("ignore-certificate-errors", true);
                        m_driver = new SafariDriver(options);
                        break;

                }

                m_driver.UpdateBrowserName(_profile.Browser.ToString());
                if (!((_profile.Browser.ToString()).Equals("Safari")))
                {
                    m_driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(180));
                    m_driver.Manage().Cookies.DeleteAllCookies();
                }

                System.Threading.Thread.Sleep(5000);
                m_driver.Manage().Cookies.ClearFlashCoockies();
                m_driver.Manage().Window.Maximize();      
            }
            else 
            {
                if (Platform.ToString().Equals("Mobile_Android"))
                {
                    DesiredCapabilities capabilities = DesiredCapabilities.Chrome();
                    foreach (var item in _profile.capabilities)
                        capabilities.SetCapability(item.Key, item.Value);
                    for (int i = 10; i > 0; i--)
                    {
                        try
                        {
                            m_driver = new RemoteWebDriver(new Uri("http://127.0.0.1:4723/wd/hub"), capabilities, TimeSpan.FromSeconds(240));
                            i = 0;
                        }
                        catch (Exception)
                        {
                            Log.WriteMessage("Waiting for Appium to start");
                        }
                    }
                }

                if (Platform.ToString().Equals("Mobile_IOS"))
                {
                    DesiredCapabilities desiredCapabilities3 = new DesiredCapabilities();
                    foreach (var item in _profile.capabilities)
                        desiredCapabilities3.SetCapability(item.Key, item.Value);
                    m_driver = new RemoteWebDriver(new Uri("http://192.168.8.2:4723/wd/hub"), desiredCapabilities3, TimeSpan.FromSeconds(240));
                }

                m_driver.Manage().Cookies.DeleteAllCookies();                    
            }

            Log.WriteMessage("WebRunner: start() : initialzation was done ", true);
                           
        }

        public void Navigate(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            Log.WriteMessage("WebRunner: Navigate() :to >> " + url, true);

            Log.WriteMessage("Opened page [" + url + "]");
            m_driver.Navigate().GoToUrl(url);

            if (this._profile.Browser == BrowserPlatform.IE)
                IE_CertificateVerificationFix();

            
        }

        public void ClearBrowserCache()
        {
            Log.WriteMessage("WebRunner: ClearBrowserCache() ", true);
            m_driver.Manage().Cookies.DeleteAllCookies();
            m_driver.Manage().Cookies.ClearFlashCoockies();
            Log.WriteMessage("Clear browser cache and flash cache");
        }

        public void RefreshNotFromCache()
        {
            Log.WriteMessage("WebRunner: RefreshNotFromCache() ", true);
            Log.WriteMessage("Refresh page");
            m_driver.Navigate().Refresh();            
        }
                          
        public void IE_CertificateVerificationFix()
        {
            System.Threading.Thread.Sleep(3000);
            try
            {

                // need to verify browser name for IE
                if ((string.Equals(m_driver.Capabilities().BrowserName.ToLower(), "internet explorer", StringComparison.OrdinalIgnoreCase)) && m_driver.Title.Contains("Certificate"))
                {
                    m_driver.FindElement(By.CssSelector("#overridelink")).SendKeys(Keys.Enter);
                    System.Threading.Thread.Sleep(5000);
                    if (m_driver.Title.Contains("Certificate"))
                    {
                        System.Threading.Thread.Sleep(5000);
                        m_driver.Navigate().GoToUrl("javascript:document.getElementById('overridelink').click()");
                        //m_Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("Unexpected error in IE_CertificateVerificationFix", ex);
            }
        }

        public void MoveRunnerByWindowUrl(string url)
        {
            string baseWindowHandle = m_driver.CurrentWindowHandle;
            ReadOnlyCollection<string> windowHandles = m_driver.WindowHandles;

            foreach (string windowHandle in windowHandles)
            {
                if (!windowHandle.Equals(baseWindowHandle))
                {
                    if (m_driver.SwitchTo().Window(windowHandle).Url.Equals(url))
                    {
                        System.Threading.Thread.Sleep(5000);
                        return;
                    }
                }
            }
            // secondary chance to move to the second page
            foreach (string windowHandle in windowHandles)
            {
                if (!windowHandle.Equals(baseWindowHandle))
                    m_driver.SwitchTo().Window(windowHandle);
            }
            System.Threading.Thread.Sleep(5000);
        }

        public void MoveTestRunnerToFrame(string framename)
        {
            m_driver.SwtichToFrame(framename);
        }

        public void Close()
        {
            Log.WriteMessage("WebRunner: Close() ", true);

#if (DEBUG)
            Log.WriteMessage("Mode=Debug");
            m_singleRunner = null;
            Log.WriteMessage("Close webdriver");
            m_driver.Quit();
            Utils.ProcessHelper.CloseProcess("chromedriver.exe");            
            if (_profile.Browser == BrowserPlatform.IE)
            {                
                Utils.ProcessHelper.CloseProcess("IEDriverServer.exe");
            }
            if (_profile.Browser == BrowserPlatform.FireFox)
            {
                Utils.ProcessHelper.CloseProcess("plugin-container.exe");
                Utils.ProcessHelper.CloseProcess("WerFault.exe");
                System.Threading.Thread.Sleep(1000);
            }
#else
           m_singleRunner = null;
           Log.WriteMessage("Close webdriver");
           m_driver.Quit();
           Utils.ProcessHelper.CloseProcess("chromedriver.exe");
           if (_profile.Browser == BrowserPlatform.Chrome)
             {
                Utils.ProcessHelper.CloseProcess("chrome.exe");
             }
           if (_profile.Browser == BrowserPlatform.IE)
             {
                 Utils.ProcessHelper.CloseProcess("iexplore.exe");
                 Utils.ProcessHelper.CloseProcess("IEDriverServer.exe");
             }
            if (_profile.Browser == BrowserPlatform.FireFox)
             {
                 Utils.ProcessHelper.CloseProcess("plugin-container.exe");
                 Utils.ProcessHelper.CloseProcess("WerFault.exe");
                 System.Threading.Thread.Sleep(1000);
              }
#endif
        }

        public void CloseBrowser()
        {
            Log.WriteMessage("WebRunner: CloseBrowser() ", true);

            string currentHandler = m_driver.CurrentWindowHandle;
            try 
            {
                foreach (var handle in m_driver.WindowHandles)
                {
                    if (!handle.Equals(currentHandler))
                    {
                        m_driver.SwitchTo().Window(currentHandler).Close();
                        m_driver.SwitchTo().Window(handle);
                        break;
                    }
                }
               
            }
            catch (Exception ex)
            {
                Log.WriteError("failed to close browser", ex);
            }

        }

        public void TakeScreenshot(string testName)
        {
            Log.WriteMessage("WebRunner: TakeScreenshot() ", true);

            string path = System.Configuration.ConfigurationManager.AppSettings["ScreenshotLocation"];
            if (string.IsNullOrEmpty(path))
            {
                Log.WriteMessage("Cannot find screenshot directory location in app.config");
                return;
            }
            if (!Directory.Exists(path))
                Directory.Exists(path);
            Log.WriteMessage(" TakeScreenshot() ");
            TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
            string dataNtime = span.TotalMilliseconds.ToString().Split('.')[0];
     
            ITakesScreenshot screenshotDriver = m_driver as ITakesScreenshot;
            string strPath = Path.Combine(path, testName + "_" + dataNtime + ".jpeg");
            Screenshot screenshot = screenshotDriver.GetScreenshot();

            //Save as a jpeg            
            Log.WriteMessage("file saved as : " + strPath);
            screenshot.SaveAsFile(strPath, System.Drawing.Imaging.ImageFormat.Jpeg);           
            Thread.Sleep(3000);
        }

        public void WaitForPageToLoad()
        {
            m_driver.WaitForPageToLoad();
        }

        public void WaitForPageToLoadAjax()
        {
            m_driver.WaitForAjaxLoadingEnds();
        }

        public static void CloseFlashWindowAfterLoaded(IWebDriver driver, string dialogBox = "NewUserDialog", int countTime = 60000)
        {
           
            int countTimer = 0;
            try
            {
                System.Threading.Thread.Sleep(1000);
                driver.SwitchTo().DefaultContent();
                System.Threading.Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                Log.WriteError("failed to swtich to default browser", ex);
            }
            
            driver.SwtichToFrame("ILobby");
           
            IWebElement CloseFlashBtn = null;
            string btnStatus = "disabled";

            if (dialogBox.Contains("LoginDialogBox"))
            {
                do
                {
                    try
                    {
                        if (driver.WaitForElement(By.CssSelector(".CloseButton")).Displayed)
                        {
                            CloseFlashBtn = driver.FindElement(By.CssSelector(".CloseButton"));
                            btnStatus = CloseFlashBtn.GetAttribute("disabled");
                            Thread.Sleep(1000);
                            countTimer = countTimer + 1000;
                        }
                    }
                    catch (Exception) { }
                }
                while (btnStatus != null && countTimer < countTime);
            }
            else
                do
                {
                    try
                    {
                        if (driver.WaitForElement(By.CssSelector("#divCloseButton")).Displayed)
                        {
                            CloseFlashBtn = driver.FindElement(By.CssSelector("#divCloseButton"));
                            btnStatus = CloseFlashBtn.GetAttribute("disabled");
                            Thread.Sleep(1000);
                            countTimer = countTimer + 1000;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteError("failed to click on closebutton of flash", ex);
                    }
                }
                while (btnStatus != null && countTimer < countTime);
            //System.Threading.Thread.Sleep(3000);
            try
            {
                if (btnStatus.Contains("true"))
                    Assert.Fail("Close Flash  button is disabled");
            }
            catch (NullReferenceException)
            {

                Log.WriteMessage("Error to find close button");
            }

            driver.WaitAndReturnElement(CloseFlashBtn).Click();
            // System.Threading.Thread.Sleep(8000);
            Log.WriteMessage("Clicked on closed window button");
        }
     
        public void DeleteWindowHandler(int key)
        {
            if ((m_RunnerActiveWindows.ContainsKey(key)))
            {
                m_RunnerActiveWindows.Remove(key);
                numberOfWindowsOpen--;
            }
           
        }

        public string CloseAlertAndGetItsText(bool acceptNextAlert = true)
        {
            try
            {
                IAlert alert = m_driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }

        public void CloseAlert(bool acceptNextAlert = true)
        {
            try
            {
                IAlert alert = m_driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }

            }
            catch (Exception ex)
            {
                Log.WriteError("failed to close alert message", ex);
            }
            finally
            {
                acceptNextAlert = true;
            }

        }
             
        
        public void Wait(int p)
        {
            System.Threading.Thread.Sleep(p);

        }

        public void RefreshPage()
        {
            m_driver.Navigate().Refresh(); 
            Log.WriteMessage("Refresh Page");

        }



        public bool IsHandlerDriverinOne()
        {
            if (m_driver.WindowHandles.Count == 1)
                return true;
            else return false;
        }
    }
}
