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
    public class HtmlControlRadioButton: HtmlControlBase
    {
        public HtmlControlRadioButton(IWebElement elem, string display, HtmlControlAttribute attribute, IBaseContainer container)
            : base(elem, display, attribute, container)
        {
            WaitForPageContainerToLoad();        
            DisplayName = display;
            IsElementInArrayList = false;
        }

        public string Text
        {
            get
            {
                try 
	                {	        
		                Log.WriteMessage("HtmlControlRadioButton: Text()", true);
                WaitForPageContainerToLoad();
                SwitchToControl();
                if(!IsElementInArrayList)
                    Refresh();
                return _baseElement.Text;
            }
	           catch (StaleElementReferenceException e)
                {
                     Logging.Log.WriteError("HtmlControlRadioButton: Text() prop was failed to get text , check the _baseElement :"
                      + _baseElement.ToString() + "exception is ", e, true);
                    throw new Exception("Problem in element selector in page , or combo is not displayed in page, or refresh() not func well");                                     
                }
            }
        }

        public void Click()
        {
            try
            {
                WaitForPageContainerToLoad();
                SwitchToControl();
                if (!IsElementInArrayList)
                    Refresh();
                _baseElement.Click();
                
            }
            catch (StaleElementReferenceException e)
            {
                Logging.Log.WriteError("HtmlControlRadioButton: Text() prop was click, try with JS :", e, true);
                _baseElement = Parent.WebRunner.m_driver.FindElement(HtmlControlAttribute.GetSelector());                
                ((IJavaScriptExecutor)Parent.WebRunner.m_driver).ExecuteScript("arguments[0].click();", _baseElement);
            }

        }


    }
}
