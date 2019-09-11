using Automation.Core.Attributes;
using Automation.Core.Containers;
using Automation.Core.Infrastructure;
using Automation.Core.Logging;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Core.Controls
{
    public class HtmlControlLink : HtmlControlBase
    {        
        public string href
        {
            get
            {
                return _baseElement.GetAttribute("href");
            }
        }

        public HtmlControlLink(IWebElement elem, string display, HtmlControlAttribute attribute, IBaseContainer container)
            : base(elem, display, attribute, container)
        {            
            DisplayName = display;
            IsElementInArrayList = false;
        }

  
        public void Click()
        {
            WaitForPageContainerToLoad();
            Log.WriteMessage("HtmlControlLink: Click()" , true);
            Logging.Log.WriteMessage("Clicked on element name : " + DisplayName);
            
            if (Parent.WebRunner.CurrentBrowser.ToString().Contains("IE"))
              IE_Case();
            else
            {
               SwitchToControl();
               Refresh();
               _baseElement.Click();
            }
           
        }

        private void IE_Case()
        {
            Log.WriteMessage("HtmlControlLink: IE_Case(), click and open new win then past the new url ", true);

            WaitForPageContainerToLoad();
            SwitchToControl();
            Refresh();
            try
            {
                string url = _baseElement.GetAttribute("href");
                Log.WriteMessage("HtmlControlLink: IE_Case(),url is:" + url, true);
                Parent.WebRunner.Navigate(url);
            }
            catch (ArgumentNullException)
            {
                _baseElement.Click();
            }
            
        }

      

      
    }
}
