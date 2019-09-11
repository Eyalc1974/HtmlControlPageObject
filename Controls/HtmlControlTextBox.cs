using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Automation.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automation.Core.Infrastructure;
using Automation.Core.Attributes;
using System.Threading;
using Automation.Core.Factory;
using Automation.Core.Containers;



namespace Automation.Core.Controls
{
    public class HtmlControlTextBox : HtmlControlBase
    {
        private string m_text;                                   

        public HtmlControlTextBox(OpenQA.Selenium.IWebElement elem, string displayName, Attributes.HtmlControlAttribute attribute, Containers.IBaseContainer container)
            : base(elem, displayName, attribute, container)
        {
            WaitForPageContainerToLoad();
            IsElementInArrayList = false;
        }
        

        public string Text
        {
            get 
            {
                try
                {
                    WaitForPageContainerToLoad();

                    SwitchToControl();

                    if (!IsElementInArrayList)
                        Refresh();
                   
                    return GetTextORValueAtrr(_baseElement); 
                }
                catch (StaleElementReferenceException e)
                {
                    string val = Parent.WebRunner.m_driver.FindElement(HtmlControlAttribute.GetSelector()).Text;
                    if (string.IsNullOrEmpty(val)) return string.Empty;
                    else return val;
                }
            }
            set
            {
                WaitForPageContainerToLoad();
                SwitchToControl();
                if (!IsElementInArrayList)
                    Refresh();
                
                _baseElement.Clear();
                _baseElement.SendKeys(value);
                Logging.Log.WriteMessage("Field " + DisplayName + " was set with value " + value);
                m_text = value;
            }
        }

        private string GetTextORValueAtrr(IWebElement base_element)
        {
            string val = base_element.GetAttribute("value");//string val = m_webRunner.m_driver.WaitAndReturnElement(base_element).GetAttribute("value");
            string innerVal = base_element.Text;
            if (string.IsNullOrEmpty(val))
            {
                Logging.Log.WriteMessage("Element value of " + DisplayName + " is  " + innerVal);
                return innerVal;
            }
            Logging.Log.WriteMessage("Element value of " + DisplayName + " is  " + m_text);

            Logging.Log.WriteMessage("HtmlControlTextBox: GetTextORValueAtrr() of control :" + DisplayName + " , val is :" + val, true);
            return val;            
        }

        public string SetValueAttr
        {
            set
            {
                SwitchToControl();
                if (!IsElementInArrayList)
                    Refresh();
                SetAttribute(_baseElement, "value", value); 
                Thread.Sleep(500);
            }
        }

        public void Click()
        {
            WaitForPageContainerToLoad();
            SwitchToControl();
            Refresh();
            try
            {
               _baseElement.Click();
            }
            catch (Exception)
            {
                
                
            }

        }

       

    }
}
