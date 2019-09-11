using Automation.Core.Attributes;
using Automation.Core.Containers;
using Automation.Core.Exceptions;
using Automation.Core.Infrastructure;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Core.Controls
{
    public class HtmlControlButton : HtmlControlBase
    {
      
        public HtmlControlButton(IWebElement elem, string display, HtmlControlAttribute attribute, IBaseContainer container)
            : base(elem, display, attribute, container)
        {            
            DisplayName = display;
            IsElementInArrayList = false;
        }

        public void Click()
        {
            SwitchToControl();
            if (!IsElementInArrayList)
                Refresh(true);
            Logging.Log.WriteMessage("Clicked on : " + DisplayName);
            try { 
                _baseElement.Click(); 
               // WaitForPageContainerToLoad(); 
            }
            catch (Exception e) 
            { 
                Logging.Log.WriteError("Failed to click on :" + DisplayName, e,true);                
            }
        }

        public string Text
        {
            get
            {
                SwitchToControl();
                if (!IsElementInArrayList)
                    Refresh();
                try
                {
                    return _baseElement.Text;
                }
                catch (NoSuchElementException) 
                {
                    Logging.Log.WriteMessage("Failed to get text with control  :" + DisplayName, true);
                    throw new ControlNotFoundException(DisplayName); 
                }
            }
        }



        public bool IsEnabled()
        {
            SwitchToControl();
            if (!IsElementInArrayList)
                Refresh();
            return _baseElement.Enabled;
        }

        /// <summary>
        /// dont know if this is the right place
        /// </summary>
        public static string CSSSelector = "input";
    }
}
