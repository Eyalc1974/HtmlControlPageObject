using NG.Automation.Core.Infrastructure;
using NG.Automation.Core.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace NG.Automation.Core.Containers
{
    /// <summary>
    /// Base page for all pages in the test projects
    /// </summary>
    public abstract class BasePage : BaseContainer
    {
        public static string m_CurrentWindowHandler;

        public string PageTitle
        {
            get { return WebRunner.m_driver.Title; }
            set { }

        }

        public string PageWindowHandler
        {
            get
            {
                m_CurrentWindowHandler = WebRunner.GetWindowHandle(this.GetHashCode());
                return m_CurrentWindowHandler;
            }
        }

        /// <summary>
        /// When using muklti-instance  of selenium obejects you must pass a test runner object
        /// </summary>
        /// <param name="runner"></param>
        public BasePage(WebRunner runner)
            : base(runner)
        {
            Log.WriteMessage("BasePage of" + GetType().ToString() + " , ctor >> GenerateProperties - started", true);
            Factory.HtmlControlFactory.GenerateProperties(this);
        }

        /// <summary>
        /// Close the current window and back to the prev window
        /// </summary>
        public void Close()
        {
             try
                {
                    Log.WriteMessage("BasePage , Close()", true);
                    //ToDo: is this a duplicity? PageWindowHandler prop
                    WebRunner.m_driver.SwitchTo().Window(PageWindowHandler).Close();
                    Log.WriteMessage("BasePage , Close() current page of page " + this.GetType().ToString(), true);
                    WebRunner.m_driver.SwitchTo().Window(WebRunner.GetPrevOpenWindowByKey(GetHashCode()));
                    Log.WriteMessage("BasePage , Close(), switch to prev window", true);
                    WebRunner.DeleteWindowHandler(GetHashCode());
                }
             catch (NoSuchWindowException ex)
               {
                   Log.WriteError("Second try to switch and close window", ex, true);
                   WebRunner.m_driver.SwitchTo().Window(WebRunner.GetPrevOpenWindowByKey(GetHashCode()));
                   WebRunner.DeleteWindowHandler(GetHashCode());
               }
        }

        /// <summary>
        ///  move runner driver to the current page
        /// </summary>
        public void FocusOnWindow()
        {
            throw new NotImplementedException("TBD");
            //Attributes.HtmlFrameAttribute frame = this.GetType().GetCustomAttribute<Attributes.HtmlFrameAttribute>();
            //try
            //{
            //    if (frame == null)
            //        _webRunner.m_driver.SwitchTo().Window(PageWindowHandler);
            //    else _webRunner.m_driver.SwitchTo().Frame(frame.FrameName);
            //}
            //catch (Exception) { }
        }

        public override void Focus()
        {
            // How do you know what to do here, this is page, do we need to swichTo() - for frame? - only switch to Window() - right, but 
            // what will happened when after create new page - which mean 2 instacnes with 1 page active?
            //try
            //{
            //    Log.WriteMessage("BasePage:Focus(), Switching to new page and window",true);
            //    WebRunner.m_driver.SwitchTo().DefaultContent();
            //}
            //catch (WebDriverException ex)
            //{
            //    Log.WriteError("failed to SwitchTo to default content", ex, true);
            //    WebRunner = new WebRunner(WebRunner._profile, WebRunner.Platform);
            //}
            //catch (Exception ex)
            //{
            //    Log.WriteError("failed to SwitchTo to default content", ex,true);
            //    throw new Exception("Please run the test again due to Selenium failure...");
            //}
            /**
             if(parent != null)
             *  parent.Focus();
             if(this is PasePage)
             *  --> switch to window
             else if(this is paseFrame)
             *  --> switvh to frame
             else -- control
             *     switch to parent only
             */
            if (Parent != null)
                Parent.Focus();

            if (!((WebRunner.Platform.ToString().Contains("IOS"))))
                WebRunner.m_driver.SwitchTo().Window(PageWindowHandler);

            //var contexts = ((IContextAware)WebRunner.m_driver).Contexts;
            //string webviewContext = null;
            //for (int i = 0; i < contexts.Count; i++)
            //{
            //    Console.WriteLine(contexts[i]);
            //    if (contexts[i].Contains("WEBVIEW"))
            //    {
            //        webviewContext = contexts[i];
            //        break;
            //    }
            //}

            Log.WriteMessage("BasePage:Focus(), driver move to new win , Switch() to new page and window has passed",true);
        }

        public override void WaitToLoad()
        {
            WebRunner.m_driver.WaitForPageToLoad();
       //     WebRunner.Wait(1500);
        }

        public override void WaitToLoadAjax()
        {
            try
            {
                WebRunner.WaitForPageToLoadAjax();
        //        WebRunner.Wait(1500);
            }
            catch (Exception)
            {                                
            }
        }
    }
}
