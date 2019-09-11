using Microsoft.VisualStudio.TestTools.UnitTesting;
using NG.Automation.Core.Attributes;
using NG.Automation.Core.Containers;
using NG.Automation.Core.Infrastructure;
using NG.Automation.Core.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NG.Automation.Core.Controls
{
    public abstract class HtmlControlBase
    {
        public string DisplayName { get; set; }
        internal OpenQA.Selenium.IWebElement _baseElement;//changed from protected
        public bool IsElementInArrayList { get; set; }
        //protected WebRunner _webRunner;
        public HtmlControlAttribute HtmlControlAttribute { get; internal set; }
        //public BasePage CurrentPage { get; private set; }
        //public BaseFrame CurrentFrame { get; private set; }  
        public IBaseContainer Parent { get; internal set; }

        public string PlaceHolder
        {
            get
            {
                Refresh();
                return _baseElement.GetAttribute("placeholder");
            }
        }

        /// <summary>
        /// Returns true if the control is loaded by the automation framework
        /// </summary>
        public bool ControlLoaded
        {
            get
            {
                try
                {
                    if (_baseElement != null)
                        return _baseElement.Enabled;//.Displayed;
                    return false;
                }
                catch (Exception ex)
                {
                    Logging.Log.WriteError("Control is not loaded " + DisplayName, ex, true);
                    return false;
                }
            }

        }

        public bool Display
        {
            get
            {
                try
                {
                    SwitchToControl();
                    Refresh();
                    if (_baseElement.Displayed) return true; else return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

        }
        /// <summary>
        /// Checks if the control is loaded by the automation framework, if not, a refresh will be perfromed
        /// </summary>
        public virtual bool ControlFound
        {
            get
            {
                try
                {
                    if (!ControlLoaded)
                    {
                        Refresh();
                        return ControlLoaded;
                    }
                    return _baseElement.Displayed;//if the reference exists but not ready by selenium
                }
                catch (StaleElementReferenceException)
                {
                    Refresh(true);
                }
                return _baseElement.Displayed;
            }


        }

        public HtmlControlBase(OpenQA.Selenium.IWebElement elem, string displayName,
            HtmlControlAttribute attribute, IBaseContainer container)
        {
            _baseElement = elem;
            DisplayName = displayName;
            HtmlControlAttribute = attribute;
            if (container == null)
                throw new ArgumentNullException("container");
            Parent = container;
            ScrollToElement();
            //CurrentPage = container.PageContainer;
            //CurrentFrame = container.FrameContainer;
        }

        /// <summary>
        /// GetAtrr from element by Atrr name
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public string GetAttribute(string attributeName)
        {
            try
            {
                if (string.IsNullOrEmpty(attributeName))
                    throw new ArgumentNullException("attributeName");
                string attribute = string.Empty;
                Refresh();
                attribute = _baseElement.GetAttribute(attributeName);
                Log.WriteMessage("HtmlControlBase: GetAttribute , attribute is: " + attribute, true);
                return attribute;

            }
            catch (Exception e)
            {
                Logging.Log.WriteError("fail to use GetAttribute of IwebElement :" + DisplayName, e, true);
                return string.Empty;
            }

        }

        /// <summary>
        /// Set Atrr for element by value
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        public void SetAttribute(IWebElement element, string attributeName, string value)
        {
            IWrapsDriver wrappedElement = element as IWrapsDriver;
            if (wrappedElement == null)
                throw new ArgumentException("element", "Element must wrap a web driver");

            IWebDriver driver = wrappedElement.WrappedDriver;
            IJavaScriptExecutor javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
                throw new ArgumentException("element", "Element must wrap a web driver that supports javascript execution");
            Log.WriteMessage("HtmlControlBase: SetAttribute , with JS with the follow command: " + value, true);

            javascript.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", element, attributeName, value);
        }

        /// <summary>
        /// Refresh element in case control not laoaded,Ajax or refresh method is always
        /// </summary>
        /// <param name="force"></param>
        public virtual void Refresh(bool force = false)
        {
            // in case the base element was created from ArrayList don't refresh it again
            if (IsElementInArrayList) return;
            Log.WriteMessage("HtmlControlBase: Refresh() _baseElement", true);

            // if it is AJAX and control not found and atrr is refresh always
            if ((ControlLoaded) && (!HtmlControlAttribute.AjaxCall) && !force &&
                (HtmlControlAttribute.RefreshMethod != RefreshMethod.Always))
                return;

            if (HtmlControlAttribute.AjaxCall)
                _baseElement = WaitForElementAJAX();
            else
                _baseElement = WaitForElement();
            
            if (!(ControlLoaded || _baseElement != null))
            {
                Log.WriteMessage("HtmlControlBase:  _baseElement was null or empty", true);
                string[] arrayPage = Parent.ToString().Split('.');
                //throw new Exceptions.ControlNotFoundException(string.Format("The HtmlControl was not found : {0} , in Page: {1}", DisplayName, arrayPage[arrayPage.Length-1]));
                Assert.Fail(string.Format("The HtmlControl : {0} was not found in page: {1}", DisplayName, arrayPage[arrayPage.Length - 1]));
            }
            else ScrollToElement();
        }

        /// <summary>
        /// ToDo: rename to SwitchToControl
        /// </summary>
        protected void SwitchToControl()
        {
            Parent.Focus();
        }

        /// <summary>
        /// Clear text from element - usually from text box field
        /// </summary>
        public void Clear()
        {
            SwitchToControl();
            Refresh();
            _baseElement.Clear();
        }

        public void WaitForPageContainerToLoad()
        {
            //toDo: switch to and wait ?
            //CurrentContainer.WebRunner.m_driver.WaitForPageToLoad
            Parent.WaitToLoad();
            //CurrentContainer.PageContainer.WaitForPagToBeLoaded();

        }

        protected internal IWebElement WaitForElementAJAX()
        {
            IWebElement elem = null;
            //try
            //{
            //    if (Parent.GetType().IsSubclassOf(typeof(BaseFrame)))
            //    {
            //        Parent.WebRunner.m_driver.SwitchTo().DefaultContent();
            //        Parent.WebRunner.m_driver.SwitchTo().Frame(Parent.WebRunner.CurrentFrame);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    NG.Automation.Core.Logging.Log.WriteError("Failed waiting for Html element (ajax). continue", ex);
            //}

            try
            {
                Attributes.HtmlControlAttribute html = HtmlControlAttribute;
                switch (html.SearchBy)
                {
                    case NG.Automation.Core.Attributes.SearchBy.Id:
                        elem = Parent.WebRunner.m_driver.WaitForElementAjax(By.Id(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.Name:
                        elem = Parent.WebRunner.m_driver.WaitForElementAjax(By.Name(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.CssSelector:
                        elem = Parent.WebRunner.m_driver.WaitForElementAjax(By.CssSelector(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.ClassName:
                        elem = Parent.WebRunner.m_driver.WaitForElementAjax(By.ClassName(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.Xpath:
                        elem = Parent.WebRunner.m_driver.WaitForElementAjax(By.XPath(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.Link:
                        elem = Parent.WebRunner.m_driver.WaitForElementAjax(By.LinkText(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.PartialLink:
                        elem = Parent.WebRunner.m_driver.WaitForElementAjax(By.PartialLinkText(html.DomAddressLocator));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                NG.Automation.Core.Logging.Log.WriteError("Failed waiting for Html element (ajax).", ex, true);
                elem = null;
            }

            return elem;
        }

        protected internal IWebElement WaitForElement()
        {
            IWebElement elem = null;
            //try
            //{
            //    if (Parent.GetType().IsSubclassOf(typeof(BaseFrame)))
            //    {
            //        Parent.WebRunner.m_driver.SwitchTo().DefaultContent();
            //        Parent.WebRunner.m_driver.SwitchTo().Frame(Parent.WebRunner.CurrentFrame);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    NG.Automation.Core.Logging.Log.WriteError("Failed waiting for Html element. continue", ex);
            //}

            try
            {
                Attributes.HtmlControlAttribute html = HtmlControlAttribute;
                switch (html.SearchBy)
                {
                    case NG.Automation.Core.Attributes.SearchBy.Id:
                        elem = Parent.WebRunner.m_driver.WaitForElement(By.Id(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.Name:
                        elem = Parent.WebRunner.m_driver.WaitForElement(By.Name(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.CssSelector:
                        elem = Parent.WebRunner.m_driver.WaitForElement(By.CssSelector(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.ClassName:
                        elem = Parent.WebRunner.m_driver.WaitForElement(By.ClassName(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.Xpath:
                        elem = Parent.WebRunner.m_driver.WaitForElement(By.XPath(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.Link:
                        elem = Parent.WebRunner.m_driver.WaitForElement(By.LinkText(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.PartialLink:
                        elem = Parent.WebRunner.m_driver.WaitForElement(By.PartialLinkText(html.DomAddressLocator));
                        break;
                    case NG.Automation.Core.Attributes.SearchBy.JavaScript:
                        elem = (IWebElement)GetElementFromJavaScript();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                NG.Automation.Core.Logging.Log.WriteError("Failed waiting for Html element", ex, true);
                elem = null;
            }

            return elem;
        }

        protected internal object GetElementFromJavaScript()
        {
            try
            {
                return ((IJavaScriptExecutor)Parent.WebRunner.m_driver).ExecuteScript(" return document.querySelector(" + HtmlControlAttribute.DomAddressLocator + ");");
            }
            catch (Exception ex)
            {
                NG.Automation.Core.Logging.Log.WriteError("Failed getting element from javascript", ex);
            }
            return string.Empty;
        }


        public void ForceClick()
        {
            WaitForPageContainerToLoad();
            SwitchToControl();
            Refresh(true);
            Log.WriteMessage("HtmlControlBase: ForceClick() val is :arguments[0].click() ", true);
            ((IJavaScriptExecutor)Parent.WebRunner.m_driver).ExecuteScript("arguments[0].click();", _baseElement);

            WaitForPageContainerToLoad();
        }

        public void ScrollToElement()
        {

            try
            {
                ((IJavaScriptExecutor)Parent.WebRunner.m_driver).ExecuteScript("document.querySelector('" + HtmlControlAttribute.DomAddressLocator + "').scrollIntoView(true);");
            }
            catch (Exception)
            {

                //  Log.WriteMessage(e.Message);
            }

        }



    }
}
