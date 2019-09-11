using NG.Automation.Core.Attributes;
using NG.Automation.Core.Containers;
using NG.Automation.Core.Infrastructure;
using NG.Automation.Core.Logging;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NG.Automation.Core.Controls
{
    public class HtmlControlCheckBox : HtmlControlBase
    {

        public HtmlControlCheckBox(IWebElement elmnt, string display, HtmlControlAttribute attribute, IBaseContainer container)
            : base(elmnt, display, attribute, container)
        {
            WaitForPageContainerToLoad();
            DisplayName = display;
            IsElementInArrayList = false;
        }

        public bool Checked
        {
            get
            {
                try
                {
                    Logging.Log.WriteMessage("HtmlControlCheckBox:Checked, tring to get val   :" + DisplayName, true);
                WaitForPageContainerToLoad();
                SwitchToControl();
                if (!IsElementInArrayList)
                    Refresh();
                return _baseElement.Selected;
            }
                catch (StaleElementReferenceException e)
                {
                    Logging.Log.WriteError("HtmlControlCheckBox :The Checked() prop of HtmlControlCheckBox was failed, check the _baseElement :"
                         + _baseElement.ToString() + "exception is ", e,true);
                     throw new Exception("Problem in element in page , selector or checkbox is not displayed in page");
                }
            }
            set
            {
                try
                {
                WaitForPageContainerToLoad();
                if (value)
                {
                    SwitchToControl();
                    if (!IsElementInArrayList)
                        Refresh();
                    if (!_baseElement.Selected)
                    {
                        JavaScriptCheck();
                        Log.WriteMessage("Element [" + DisplayName + "] was unchecked");
                    }
                }
                else
                {
                    SwitchToControl();
                    if (!IsElementInArrayList)
                        Refresh();
                        if (_baseElement.Selected)
                        {
                            JavaScriptCheck();
                            Log.WriteMessage("Element [" + DisplayName + "] was checked");
                        }
                    }                
            }
                catch (StaleElementReferenceException e)
                {
                    Logging.Log.WriteError("HtmlControlCheckBox :The Checked() prop of HtmlControlCheckBox was failed, check the _baseElement :"
                        + _baseElement.ToString() + "exception is ", e, true);
                    throw new Exception("HtmlControlCheckBox: Problem in element in page , selector or checkbox is not displayed in page");
                }
            }
        }
                    
        private void JavaScriptCheck()
        {
            WaitForPageContainerToLoad();
            if (Parent.WebRunner.m_driver is IJavaScriptExecutor)
            {
                ((IJavaScriptExecutor)Parent.WebRunner.m_driver).ExecuteScript("javascript:document.getElementById('" + _baseElement.ID() + "').click()");
            }
        }

        public void Click()
        {
            try
            {
            Parent.WebRunner.m_driver.WaitForPageToLoad();
            SwitchToControl();
                Logging.Log.WriteMessage("HtmlControlCheckBox : click() , trying to click by Selenium", true);
            if (!IsElementInArrayList)
                Refresh();
           _baseElement.Click();
        }
            catch (StaleElementReferenceException e)
            {
                Logging.Log.WriteError("HtmlControlCheckBox: Click() prop was failed to click, check the _baseElement :"
                        + _baseElement.ToString() + "exception is ", e, true);
                throw new Exception("Problem in element selector in page , or checkbox is not displayed in page, or refresh() not func well");
            }
            catch (ElementNotVisibleException e)
            {

                Logging.Log.WriteError("HtmlControlCheckBox: Click() prop of HtmlControlCheckBox was failed, check the _baseElement :"
                        + _baseElement.ToString() + "exception is ", e, true);
                throw new Exception("Problem in element selector in page , or checkbox is not displayed in page, or refresh() not func well");
            }

        }

    }
}
