using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NG.Automation.Core.Infrastructure
{
    public class JSWaiter
    {        
        
         private static IWebDriver m_driver;

         private static WebDriverWait jsWait;
        
         private static IJavaScriptExecutor jsExec;

            // Get the driver 
        public static void SetRunner(WebRunner runner)
         {
             m_driver = runner.m_driver;
             jsWait = new WebDriverWait(m_driver, TimeSpan.FromSeconds(20));
             jsExec = ((IJavaScriptExecutor)(m_driver));
         }

            // Wait for JQuery Load
        public static void WaitForJQueryLoad() 
        {
                try
                {
                    var ajaxIsComplete = jsExec.ExecuteScript("return jQuery.active");
                    bool jscommand = jsWait.Until<bool>(d => ExpectedConditions.Equals(jsExec.ExecuteScript("return jQuery.active").ToString(), "0"));                 
                }
                catch (Exception)
                {                   
                }              
         }
            
            // Wait for Angular Load
        public static void WaitForAngularLoad() 
        {
            IJavaScriptExecutor jsExec = ((IJavaScriptExecutor)(m_driver));
            String angularReadyScript = "return angular.element(document).injector().get(\'$http\').pendingRequests.length === 0";
            // Wait for ANGULAR to load
            try
            {
                var ajaxIsComplete = jsExec.ExecuteScript(angularReadyScript);
                bool jscommand = jsWait.Until<bool>(d => ExpectedConditions.Equals(jsExec.ExecuteScript(angularReadyScript).ToString(), "true"));                 

                //jsWait.Until<bool>((m_driver1) =>
                //  {
                //      var ajaxIsComplete = (bool)jsExec.ExecuteScript(angularReadyScript);
                //      if (ajaxIsComplete) return true; else return false;
                //  });
            }
            catch (Exception e)
            {                              
            }      
        }

            // Wait Until JS Ready
        public static void WaitUntilJSReady() 
        {
           WebDriverWait wait = new WebDriverWait(m_driver, TimeSpan.FromSeconds(15));
           IJavaScriptExecutor jsExec = ((IJavaScriptExecutor)(m_driver));
           // Wait for Javascript to load

           try
           {
               var ajaxIsComplete = jsExec.ExecuteScript("return document.readyState");
               bool jscommand = jsWait.Until<bool>(d => ExpectedConditions.Equals(jsExec.ExecuteScript("return document.readyState").ToString(), "complete"));                 
               //jsWait.Until<bool>((m_driver1) =>
               //   {
               //       var ajaxIsComplete = (string)jsExec.ExecuteScript("return document.readyState");
               //       if (ajaxIsComplete.Equals("complete") & ajaxIsComplete != null) return true; else return false;
               //   });
           }
           catch (Exception)
           {              
           }                 
        }

            // Wait Until JQuery and JS Ready
        public static void WaitUntilJQueryReady() 
        {
         IJavaScriptExecutor jsExec = ((IJavaScriptExecutor)(m_driver));
        // First check that JQuery is defined on the page. If it is, then wait AJAX
        Boolean jQueryDefined = ((Boolean)(jsExec.ExecuteScript("return typeof jQuery != \'undefined\'")));
        if ((jQueryDefined == true)) 
            {
            // Pre Wait for stability (Optional)
            JSWaiter.Sleep(5);
            // Wait JQuery Load
            JSWaiter.WaitForJQueryLoad();
            // Wait JS Load
            JSWaiter.WaitUntilJSReady();
            // Post Wait for stability (Optional)
            JSWaiter.Sleep(5);
            }
            else 
                Console.WriteLine("jQuery is not defined on this site!");                     
        }

       // Wait Until Angular and JS Ready
        public static void WaitUntilAngularReady() 
       {
        jsExec = ((IJavaScriptExecutor)(m_driver));
        // First check that ANGULAR is defined on the page. If it is, then wait ANGULAR
        Boolean angularUnDefined = ((Boolean)(jsExec.ExecuteScript("return window.angular === undefined")));
        if (!angularUnDefined) 
        {
            Boolean angularInjectorUnDefined = ((Boolean)(jsExec.ExecuteScript("return angular.element(document).injector() === undefined")));
            if (!angularInjectorUnDefined) 
            {
                // Pre Wait for stability (Optional)
                JSWaiter.Sleep(5);
                // Wait Angular Load
                JSWaiter.WaitForAngularLoad();
                // Wait JS Load
                JSWaiter.WaitUntilJSReady();
                // Post Wait for stability (Optional)
                JSWaiter.Sleep(5);
            }
            else 
                Console.WriteLine("Angular injector is not defined on this site!");                        
        }
        else 
            Console.WriteLine("Angular is not defined on this site!");                
        }

        // Wait Until JQuery Angular and JS is ready
        public static void WaitJQueryAngular()
          {
             JSWaiter.WaitUntilJQueryReady();
             JSWaiter.WaitUntilAngularReady();
          }

        public static void WaitForPageLoad()
        {
            WaitJQueryAngular();
        }

        public static void Sleep(int seconds)
         {
           long secondsLong = ((long)(seconds));
            try
             {
               Thread.Sleep(seconds);
             }
            catch (Exception)
                {                    
                }
          }
        
    }    
}
