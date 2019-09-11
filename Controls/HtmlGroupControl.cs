using NG.Automation.Core.Attributes;
using NG.Automation.Core.Controls;
using NG.Automation.Core.Infrastructure;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NG.Automation.Core.Containers
{
    
    public class HtmlGroupControl : HtmlControlBase, Containers.IBaseContainer
    {
        public IWebElement baseElement;

        public HtmlGroupControl(IWebElement elem, string display, HtmlControlAttribute attribute, IBaseContainer page)
            : base(elem, display, attribute, page)
        {
            baseElement = elem;
            WaitForPageContainerToLoad();            
        }

        public void Focus()
        {
            Parent.Focus();
        }

        public void WaitToLoad()
        {
            Parent.WaitToLoad();
        }

        public void WaitToLoadAjax()
        {
            Parent.WaitToLoadAjax();
        }

        public WebRunner WebRunner
        {
            get { return Parent.WebRunner; }
        }
    }
}
