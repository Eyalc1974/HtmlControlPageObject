using Automation.Core.Attributes;
using Automation.Core.Containers;
using Automation.Core.Controls;
using Automation.Core.Infrastructure;
using Automation.Core.Logging;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Automation.Core.Factory
{
    static class HtmlControlFactory
    {
        /// <summary>
        /// Generates the container properties dynamically
        /// </summary>
        /// <param name="container"></param>
        public static void GenerateProperties(IBaseContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("page");
            
            container.Focus();
            InitializeControlProperties(container);
        }
       
        private static void InitializeControlProperties(IBaseContainer container)
        {           
            if (container == null)
                throw new Exception("Page cannot be null");

            //ToDo: swithTo migth also wait or create also SwitchToAndWait
            if(container is BasePage)
                (container as BasePage).WaitToLoad();

            Automation.Core.Logging.Log.WriteMessage("-------------  HtmlControlFactory:InitializeControlProperties of page " + container.GetType().ToString() +  "------------- ", true);
            container.WaitToLoad();

            PropertyInfo[] properties = container.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (PropertyInfo pi in properties)
            {
                Attributes.HtmlControlAttribute html = pi.GetCustomAttribute<Attributes.HtmlControlAttribute>();
               
                if (PropertyIsHtmlArrayElement(pi))
                {
                    // if HtmlArrayControl[HtmlGroupControl]
                    if (GetGenericInnerType(pi).IsSubclassOf(typeof(HtmlGroupControl)))
                    {
                        Controls.HtmlControlBase[] array = Factory.HtmlControlFactory.CreateElements(pi, container);
                        object grpCtrl = Activator.CreateInstance(pi.PropertyType, array, pi, html, container);
                        pi.SetValue(container, grpCtrl, null);
                    }
                    else
                    {
                        // if HtmlArrayControl but not HtmlGroupControl
                        Controls.HtmlControlBase[] array = Factory.HtmlControlFactory.CreateElements(pi, container);
                        //create array of objects
                        object baseControl = Activator.CreateInstance(pi.PropertyType, array, pi, html, container);
                        Automation.Core.Logging.Log.WriteMessage("The control: " + baseControl.GetType().ToString() +
                            " , genreated and set in page: " + container.GetType().ToString(), true);
                        pi.SetValue(container, baseControl, null);
                    }
                }// if HtmlGroupControl
                else if (PropertyIsGroupControl(pi))
                {
                    IWebElement elm = null;
                    HtmlControlAttribute atrContrl = pi.GetCustomAttribute<HtmlControlAttribute>();
                    try
                    {
                        elm = container.WebRunner.m_driver.FindElement(By.CssSelector(atrContrl.DomAddressLocator));
                        // need to check : elm = container.WebRunner.m_driver.FindElement(html.GetSelector());
                    }
                    catch (Exception) { }

                    //ToDo: container should be BasePage but won't work if HtmlGroupControl contains HtmlGroupControl 
                    var grpCtrl = Activator.CreateInstance(pi.PropertyType, elm,
                        "", atrContrl, container as BasePage) as HtmlGroupControl;
                    grpCtrl.Parent = container;
                    GenerateProperties(grpCtrl);                    
                    pi.SetValue(container, grpCtrl, null);
                } // if BaseGroupControlPage
                else if (PropertyIsGroupPage(pi))
                {
                    BaseGroupControlPage baseGroupControl;
                    //find the page object in case of control within control
                    BaseGroupControlPage groupCntrl = container as BaseGroupControlPage;
                    if (groupCntrl != null)
                    {
                        //find its parent
                        baseGroupControl = Activator.CreateInstance(pi.PropertyType, groupCntrl.PageContainer) as BaseGroupControlPage;
                    }
                    //create group object                   
                    else baseGroupControl = Activator.CreateInstance(pi.PropertyType, container) as BaseGroupControlPage;
                    
                    baseGroupControl.Parent = container;
                    GenerateProperties(baseGroupControl);                    
                    pi.SetValue(container, baseGroupControl, null);
                }
                else if (PropertyIsFrame(pi))
                {
                    BaseFrame baseFrame;
                    Attributes.HtmlFrameAttribute fname = pi.GetCustomAttribute<Attributes.HtmlFrameAttribute>();
                    if (!(fname.FrameName.Equals("1") || fname.FrameName.Equals("2")))
                        baseFrame = Activator.CreateInstance(pi.PropertyType, container.WebRunner, fname.FrameName, container) as BaseFrame;
                    else
                    {
                        int indexFrame = Int32.Parse(fname.FrameName);
                        baseFrame = Activator.CreateInstance(pi.PropertyType, container.WebRunner, indexFrame, container) as BaseFrame;
                    }
                    pi.SetValue(container, baseFrame);
                    container.Focus();
                }
                else if (PropertyIsHtmlElement(pi))
                {
                    object baseControl = Factory.HtmlControlFactory.CreateElement(pi, container);
                    if (pi.ReflectedType.BaseType.Name.Equals("HtmlGroupControl"))
                        (baseControl as HtmlControlBase).IsElementInArrayList = true;
                    pi.SetValue(container, baseControl, null);
                }
            }        
        }

        private static bool PropertyIsFrame(PropertyInfo pi)
        {
            return pi.PropertyType.IsSubclassOf(typeof(BaseFrame));
        }

        private static Type GetGenericInnerType(PropertyInfo pi)
        {
            Type arrayControl = pi.PropertyType;
            if(!arrayControl.IsGenericType)
                return null;
            
            if (arrayControl.GenericTypeArguments.Length != 1)
                return null;
            return arrayControl.GenericTypeArguments[0];
        }        
                    
        private static bool PropertyIsHtmlArrayElement(PropertyInfo pi)
        {
            return pi.PropertyType.Name.StartsWith("HtmlArrayControl");

        }

        private static bool PropertyIsHtmlElement(PropertyInfo pi)
        {
            Attributes.HtmlControlAttribute html = pi.GetCustomAttribute<Attributes.HtmlControlAttribute>();
            return html != null;
        }

        private static bool PropertyIsGroupPage(PropertyInfo pi)
        {
            return pi.PropertyType.IsSubclassOf(typeof(BaseGroupControlPage));
        }

        private static bool PropertyIsGroupControl(PropertyInfo pi)
        {
            return pi.PropertyType.IsSubclassOf(typeof(HtmlGroupControl));
        }
                     
        /// <summary>
        /// Create Control based on proprty and frame name 
        /// </summary>
        /// <param name="pi">propery info</param>
        /// <param name="FrameName"></param>
        /// <param name="webRunner"></param>
        /// <returns></returns>
        private static Controls.HtmlControlBase CreateElement(PropertyInfo pi, IBaseContainer container)
        {
            IWebDriver driver = container.WebRunner.m_driver;
            IWebElement elem = null;

            Attributes.HtmlControlAttribute html = pi.GetCustomAttribute<Attributes.HtmlControlAttribute>();           
            if (html == null)
                throw new Exception("Proerty is not Html Element");

            if ((container as HtmlGroupControl) != null)
            {
                if ((container as HtmlGroupControl).HtmlControlAttribute != null)
                    elem = LocateElement((container as HtmlGroupControl).baseElement, html, container);
                else
                    elem = LocateElement(container.WebRunner, html);
            }

            return CreateControl(pi.PropertyType.Name, elem, pi.Name, html, container);

        }

        private static IWebElement LocateElement(IWebElement grpCtrlElment, HtmlControlAttribute atrrCtrl, IBaseContainer container)
        {
            IWebDriver driver = container.WebRunner.m_driver;
            IWebElement elem = null;                     

            try
            {
                switch (atrrCtrl.SearchBy)
                {
                    case Automation.Core.Attributes.SearchBy.Id:
                        elem = grpCtrlElment.FindElement(By.Id(atrrCtrl.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.Name:
                        elem = grpCtrlElment.FindElement(By.Name(atrrCtrl.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.CssSelector:
                        elem = grpCtrlElment.FindElement(By.CssSelector(atrrCtrl.DomAddressLocator));                        
                        break;
                    case Automation.Core.Attributes.SearchBy.ClassName:
                        elem = grpCtrlElment.FindElement(By.ClassName(atrrCtrl.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.Xpath:
                        elem = grpCtrlElment.FindElement(By.XPath(atrrCtrl.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.Link:
                        elem = grpCtrlElment.FindElement(By.LinkText(atrrCtrl.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.PartialLink:
                        elem = grpCtrlElment.FindElement(By.PartialLinkText(atrrCtrl.DomAddressLocator));
                        break;
                    default:
                        break;
        }
            }
            catch (NoSuchElementException ex)
            {
                Automation.Core.Logging.Log.WriteError("Failed locating Html element", ex);
                elem = null;
            }

            return elem;


        }

        public static Controls.HtmlControlBase CreateElement(Type control, Attributes.HtmlControlAttribute html, IBaseContainer container)
        {
            IWebDriver driver = container.WebRunner.m_driver;
                        
            if (html == null)
                throw new Exception("Proerty is not Html Element");

            IWebElement elem = LocateElement(container.WebRunner, html);
            Automation.Core.Logging.Log.WriteMessage("HtmlControlFactory :CreateElement(), creating Control wrapped with element above");
            return CreateControl(control.Name, elem, "", html, container);

        }

        private static Controls.HtmlControlBase CreateControl(string controlName, IWebElement elem, string name,
            HtmlControlAttribute html, IBaseContainer container) //string controlName
        {
            switch (controlName)
            {
                case "HtmlControlTable":
                    return new Controls.HtmlControlTable(elem, name, html, container);
                case "HtmlControlTextBox":
                    return new Controls.HtmlControlTextBox(elem, name, html, container);
                case "HtmlControlComboBox":
                    return new Controls.HtmlControlComboBox(elem, name, html, container);
                case "HtmlControlRadioButton":
                    return new Controls.HtmlControlRadioButton(elem, name, html, container);
                case "HtmlControlButton":
                    return new Controls.HtmlControlButton(elem, name, html, container);
                case "HtmlControlCheckBox":
                    return new Controls.HtmlControlCheckBox(elem, name, html, container);
                case "HtmlControlLink":
                    return new Controls.HtmlControlLink(elem, name, html, container);
                case "HtmlControlLabel":
                    return new Controls.HtmlControlLabel(elem, name, html, container);
                case "HtmlControlDynamicTable":
                    return new Controls.HtmlControlDynamicTable(elem, name, html, container);
                default:
                    if (container.GetType().IsSubclassOf(typeof(HtmlGroupControl)))
                        return Activator.CreateInstance(container.GetType(), elem, html, container) as Controls.HtmlControlBase;
                    else return null;
                     
                    //return new HtmlGroupControl(elem, name, html, container);
                    
            }
            return null;
        }

        public static Controls.HtmlControlBase[] CreateElements(PropertyInfo pi, IBaseContainer container)
        {
            IWebDriver driver =  container.WebRunner.m_driver;

            Attributes.HtmlControlAttribute html = pi.GetCustomAttribute<Attributes.HtmlControlAttribute>();
            Attributes.HtmlTableAttribute tableAtrr = pi.GetCustomAttribute<Attributes.HtmlTableAttribute>();

            if (html == null)
                throw new Exception("Proerty is not Html Element");

            IList<IWebElement> elems = LocateElements(container.WebRunner, html);
            List<Controls.HtmlControlBase> result = new List<Controls.HtmlControlBase>();
            
            string controlName = pi.PropertyType.Name;
            if (pi.PropertyType.Name.StartsWith("HtmlArrayControl"))
            {
                controlName = pi.PropertyType.GetGenericArguments()[0].Name;
            }            

            Automation.Core.Logging.Log.WriteMessage("HtmlControlFactory :CreateElements(), creating Controls:" + controlName ,true );

            foreach (IWebElement elem in elems)
            {
                if (pi.PropertyType.GetGenericArguments()[0].IsSubclassOf(typeof(HtmlGroupControl)))
                {
                    //IWebElement elem, string display, HtmlControlAttribute attribute,IBaseContainer container
                    IBaseContainer innerContainer = Activator.CreateInstance(pi.PropertyType.GetGenericArguments()[0], elem, controlName, html, container) as IBaseContainer; 
                    InitializeControlProperties(innerContainer);
                    result.Add(innerContainer as HtmlControlBase);
                }
                else result.Add(CreateControl(pi.PropertyType.GetGenericArguments()[0].Name, elem, pi.Name, html.Clone() as Attributes.HtmlControlAttribute, container));
   
            }
            return result.ToArray();
        }        
        private static IList<IWebElement> LocateElements(Automation.Core.Containers.BasePage container, Attributes.HtmlControlAttribute html, string parentlocator)
        {
            IList<IWebElement> elemnts = null;

            //if (!container.WebRunner.CurrentFrame.Equals("DEFAULT"))
            //{
            //    try
            //    {
            //        container.WebRunner.m_driver.SwitchTo().DefaultContent();
            //        container.WebRunner.m_driver.SwitchTo().Frame(container.WebRunner.CurrentFrame);
            //    }
            //    catch (Exception ex)
            //    {
            //        NG.Automation.Core.Logging.Log.WriteError("Failed locating Html elements. continue", ex);
            //    }
            //}

            try
            {
                switch (html.SearchBy)
                {
                    case Automation.Core.Attributes.SearchBy.Id:
                        elemnts = container.WebRunner.m_driver.FindElement(By.CssSelector(parentlocator)).FindElements(By.Id(html.DomAddressLocator));
                        //myby = By.Id(html.DomAddressLocator);
                        break;
                    case Automation.Core.Attributes.SearchBy.Name:
                        elemnts = container.WebRunner.m_driver.FindElement(By.CssSelector(parentlocator)).FindElements(By.Name(html.DomAddressLocator));
                        //myby = By.Name(html.DomAddressLocator);
                        break;
                    case Automation.Core.Attributes.SearchBy.CssSelector:
                        elemnts = container.WebRunner.m_driver.FindElement(By.CssSelector(parentlocator)).FindElements(By.CssSelector(html.DomAddressLocator));
                        //myby = By.CssSelector(html.DomAddressLocator);
                        break;
                    case Automation.Core.Attributes.SearchBy.ClassName:
                        elemnts = container.WebRunner.m_driver.FindElement(By.CssSelector(parentlocator)).FindElements(By.ClassName(html.DomAddressLocator));
                        //myby = By.ClassName(html.DomAddressLocator);
                        break;
                    case Automation.Core.Attributes.SearchBy.Xpath:
                        elemnts = container.WebRunner.m_driver.FindElement(By.CssSelector(parentlocator)).FindElements(By.XPath(html.DomAddressLocator));
                        //myby = By.XPath(html.DomAddressLocator);
                        break;
                    case Automation.Core.Attributes.SearchBy.Link:
                        elemnts = container.WebRunner.m_driver.FindElement(By.CssSelector(parentlocator)).FindElements(By.LinkText(html.DomAddressLocator));
                        //myby = By.LinkText(html.DomAddressLocator);
                        break;
                    case Automation.Core.Attributes.SearchBy.PartialLink:
                        elemnts = container.WebRunner.m_driver.FindElement(By.CssSelector(parentlocator)).FindElements(By.PartialLinkText(html.DomAddressLocator));
                        //myby = By.PartialLinkText(html.DomAddressLocator);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Automation.Core.Logging.Log.WriteError("Failed locating Html elements.", ex);
                elemnts = null;
            }

            return elemnts;
        }

        private static IWebElement LocateElement(WebRunner runner, Attributes.HtmlControlAttribute html)
        {
            int counter = 1;
            Attributes.HtmlControlAttribute _html = html;
            IWebElement elem = null;
           
            try
            {
                switch (html.SearchBy)
                {
                    case Automation.Core.Attributes.SearchBy.Id:
                        elem = runner.m_driver.FindElement(By.Id(html.DomAddressLocator));
                        //myby = (By.Id(html.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.Name:
                        elem = runner.m_driver.FindElement(By.Name(html.DomAddressLocator));
                        //myby = (By.Name(html.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.CssSelector:
                        elem = runner.m_driver.FindElement(By.CssSelector(html.DomAddressLocator));
                        //myby = (By.CssSelector(html.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.ClassName:
                        elem = runner.m_driver.FindElement(By.ClassName(html.DomAddressLocator));
                        //myby = (By.ClassName(html.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.Xpath:
                        elem = runner.m_driver.FindElement(By.XPath(html.DomAddressLocator));
                        //myby = (By.XPath(html.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.Link:
                        elem = runner.m_driver.FindElement(By.LinkText(html.DomAddressLocator));
                        //myby = (By.LinkText(html.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.PartialLink:
                        elem = runner.m_driver.FindElement(By.PartialLinkText(html.DomAddressLocator));
                        //myby = (By.PartialLinkText(html.DomAddressLocator));
                        break;
                    default:
                        break;
                }
            }
            catch (NoSuchElementException ex) 
            {
                Automation.Core.Logging.Log.WriteMessage("Element not found, perhaps page is not loaded yet or element not found in page:", true);
                Automation.Core.Logging.Log.WriteMessage("Failed locating Html element in page by: "
                    + html.DomAddressLocator + " , Error :" + ex.Message, true);
                elem = null; 
            }
            catch (WebDriverException ex)
            {                
                runner = new WebRunner(runner._profile, runner.Platform);
                if(counter==1) LocateElement(runner, _html);
                counter++;
                if (counter >= 2)
                {
                    Log.WriteError("Factory:LocateElement, Couldn't recover from WebDriverException , driver died", ex, true);
                    throw new Exception("Couldn't recover from WebDriverException , driver died");
                }
            }

            return elem;
        }

        private static IWebElement WaitForElementAJAX(BaseContainer CurrentContainer, Attributes.HtmlControlAttribute html)
        {
            IWebElement elem = null;

            //try
            //{
            //    CurrentContainer.WebRunner.m_driver.SwitchTo().DefaultContent();
            //    CurrentContainer.WebRunner.m_driver.SwitchTo().Frame(CurrentContainer.WebRunner.CurrentFrame);
            //}
            //catch (Exception ex) 
            //{
            //    NG.Automation.Core.Logging.Log.WriteError("Failed waiting for Html element (ajax). continue", ex);
            //}

            try
            {
                Attributes.HtmlControlAttribute _html = html;
                switch (_html.SearchBy)
                {
                    case Automation.Core.Attributes.SearchBy.Id:
                        elem = CurrentContainer.WebRunner.m_driver.WaitForElementAjax(By.Id(_html.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.Name:
                        elem = CurrentContainer.WebRunner.m_driver.WaitForElementAjax(By.Name(_html.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.CssSelector:
                        elem = CurrentContainer.WebRunner.m_driver.WaitForElementAjax(By.CssSelector(_html.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.ClassName:
                        elem = CurrentContainer.WebRunner.m_driver.WaitForElementAjax(By.ClassName(_html.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.Xpath:
                        elem = CurrentContainer.WebRunner.m_driver.WaitForElementAjax(By.XPath(_html.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.Link:
                        elem = CurrentContainer.WebRunner.m_driver.WaitForElementAjax(By.LinkText(_html.DomAddressLocator));
                        break;
                    case Automation.Core.Attributes.SearchBy.PartialLink:
                        elem = CurrentContainer.WebRunner.m_driver.WaitForElementAjax(By.PartialLinkText(_html.DomAddressLocator));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Automation.Core.Logging.Log.WriteError("Failed waiting for Html element (ajax).", ex);
                elem = null;
            }

            return elem;
        }

        private static IList<IWebElement> LocateElements(WebRunner runner, Attributes.HtmlControlAttribute html)
        {
            Attributes.HtmlControlAttribute _html = html;
            int counter = 1;
            IList<IWebElement> elemnts = null;
            //if (!runner.CurrentFrame.Equals("DEFAULT"))
            //{
            //    try
            //    {
            //        runner.m_driver.SwitchTo().DefaultContent();
            //        runner.m_driver.SwitchTo().Frame(runner.CurrentFrame);
            //    }
            //    catch (Exception ex)
            //    {
            //        NG.Automation.Core.Logging.Log.WriteError("Failed locating Html elements. continue", ex);
            //    }
            //}

            try
            {
                switch (html.SearchBy)
                {
                    case Automation.Core.Attributes.SearchBy.Id:
                        elemnts = runner.m_driver.FindElements(By.Id(html.DomAddressLocator));
                        //myby = By.Id(html.DomAddressLocator);
                        break;
                    case Automation.Core.Attributes.SearchBy.Name:
                        elemnts = runner.m_driver.FindElements(By.Name(html.DomAddressLocator));
                        //myby = By.Name(html.DomAddressLocator);
                        break;
                    case Automation.Core.Attributes.SearchBy.CssSelector:
                        elemnts = runner.m_driver.FindElements(By.CssSelector(html.DomAddressLocator));
                        //myby = By.CssSelector(html.DomAddressLocator);
                        break;
                    case Automation.Core.Attributes.SearchBy.ClassName:
                        elemnts = runner.m_driver.FindElements(By.ClassName(html.DomAddressLocator));
                        //myby = By.ClassName(html.DomAddressLocator);
                        break;
                    case Automation.Core.Attributes.SearchBy.Xpath:
                        elemnts = runner.m_driver.FindElements(By.XPath(html.DomAddressLocator));
                        //myby = By.XPath(html.DomAddressLocator);
                        break;
                    case Automation.Core.Attributes.SearchBy.Link:
                        elemnts = runner.m_driver.FindElements(By.LinkText(html.DomAddressLocator));
                        //myby = By.LinkText(html.DomAddressLocator);
                        break;
                    case Automation.Core.Attributes.SearchBy.PartialLink:
                        elemnts = runner.m_driver.FindElements(By.PartialLinkText(html.DomAddressLocator));
                        //myby = By.PartialLinkText(html.DomAddressLocator);
                        break;
                    default:
                        break;
                }
            }
            catch (NoSuchElementException ex)
            {
                Automation.Core.Logging.Log.WriteMessage("Element not found, perhaps page is not loaded yet or element not found in page:", true);
                Automation.Core.Logging.Log.WriteMessage("Failed locating Html element in page by: "
                    + html.DomAddressLocator + " , Error :" + ex.Message, true);
                elemnts = null; 
            }
            catch (WebDriverException ex)
            { // recover from died driver
                runner = new WebRunner(runner._profile, runner.Platform);
                if (counter == 1) LocateElement(runner, _html);
                counter++;
                if (counter >= 2)
                {
                    Log.WriteError("Factory:LocateElement, Couldn't recover from WebDriverException , driver died", ex, true);
                    throw new Exception("Couldn't recover from WebDriverException , driver died");
                }
            }

            return elemnts;
        }
           
    
    }
}
