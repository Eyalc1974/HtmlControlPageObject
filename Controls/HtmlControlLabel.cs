using NG.Automation.Core.Attributes;
using NG.Automation.Core.Containers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NG.Automation.Core.Controls
{
    public class HtmlControlLabel: HtmlControlBase
    {
        private string m_text;

        public HtmlControlLabel(OpenQA.Selenium.IWebElement elem, string displayName, HtmlControlAttribute attribute, IBaseContainer container)
            : base(elem, displayName, attribute, container)
        {
            WaitForPageContainerToLoad();
            DisplayName = displayName;
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
                catch (StaleElementReferenceException )
                {
                    string val = Parent.WebRunner.m_driver.FindElement(HtmlControlAttribute.GetSelector()).Text;
                    if (string.IsNullOrEmpty(val)) return string.Empty;
                    else return val;
                }
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
            return val;
        }        
    }
}
