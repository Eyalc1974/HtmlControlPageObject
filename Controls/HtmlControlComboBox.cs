using NG.Automation.Core.Attributes;
using NG.Automation.Core.Containers;
using NG.Automation.Core.Infrastructure;
using NG.Automation.Core.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NG.Automation.Core.Controls
{
    public class HtmlControlComboBox : HtmlControlBase
    {
        private SelectElement m_Element = null;


        public HtmlControlComboBox(IWebElement elem, string display, HtmlControlAttribute attribute, IBaseContainer container)
            : base(elem, display, attribute, container)
        {
            WaitForPageContainerToLoad();
            IsElementInArrayList = false;
            if (elem != null)
                m_Element = new SelectElement(elem);
            DisplayName = display;
        }

        public IList<string> Options
        {
            get
            {
                WaitForPageContainerToLoad();
                SwitchToControl();
                Refresh();
                m_Element = new SelectElement(_baseElement);
                if (m_Element != null)
                {
                    IList<string> options = new List<string>();
                    IList<IWebElement> elemnts = m_Element.Options;
                    for (int i = 0; i < elemnts.Count; i++)
                    {
                        options.Add(elemnts[i].Text);
                    }
                    Log.WriteMessage("HtmlControlComboBox of, " + DisplayName + ": ,options are :" + options.ToString(), true);
                    return options;

                }
                return null;
            }

        }

        public string GetSelectedText
        {
            get
            {
                try
                {
                WaitForPageContainerToLoad();
                SwitchToControl();
                Refresh();
                m_Element = new SelectElement(_baseElement);
                return m_Element.SelectedOption.Text;
            }
                catch (StaleElementReferenceException e)
                {
                     Logging.Log.WriteError("HtmlControlComboBox: GetSelectedText() prop was failed to get selected option, check the _baseElement :"
                        + _baseElement.ToString() + "exception is ", e, true);
                    throw new Exception("Problem in element selector in page , or combo is not displayed in page, or refresh() not func well");
                   
                }
            }
        }

        public int SelectedIndex
        {
            set
            {
                try
                {
                WaitForPageContainerToLoad();
                SwitchToControl();
                Refresh();
                m_Element = new SelectElement(_baseElement);
                m_Element.SelectByIndex(value);
                Log.WriteMessage("Element [" + DisplayName + "] selected [" + value + "]");
                Parent.WebRunner.m_driver.WaitForPageToLoad();
            }
                catch (NoSuchElementException e)
                {
                    Logging.Log.WriteError("HtmlControlComboBox: SelectedIndex() prop was failed to get selected option, check the _baseElement :"
                      + _baseElement.ToString() + "exception is ", e, true);
                    throw new Exception("Problem in element selector in page , or combo is not displayed in page, or refresh() not func well");                   
                }
            }
        }

        public string SelectedValue
        {
            get
            {
                try
                {
                WaitForPageContainerToLoad();
                SwitchToControl();
                Refresh();
                m_Element = new SelectElement(_baseElement);
                return m_Element.SelectedOption.Text;
            }
                catch (StaleElementReferenceException e)
                {
                     Logging.Log.WriteError("HtmlControlComboBox: SelectedValue() prop was failed to get selected val, check the _baseElement :"
                      + _baseElement.ToString() + "exception is ", e, true);
                    throw new Exception("Problem in element selector in page , or combo is not displayed in page, or refresh() not func well");                                     
                }
            }

            set
            {
                try
                {
                WaitForPageContainerToLoad();
                SwitchToControl();
                Refresh();
                m_Element = new SelectElement(_baseElement);
                m_Element.SelectByText(value);
                Log.WriteMessage("Field [" + DisplayName + "] updated with value: [" + value + "]");
                WaitForPageContainerToLoad();
                Thread.Sleep(1000);
            }
                catch (NoSuchElementException e)
                {
                    Logging.Log.WriteError("HtmlControlComboBox: SelectedValue() prop was failed to set selected option, check the _baseElement :"
                      + _baseElement.ToString() + "exception is ", e, true);
                    throw new Exception("Problem in element selector in page , or combo is not displayed in page, or refresh() not func well");
        }
            }
        }

     

    }
}
