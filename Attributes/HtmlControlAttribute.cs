using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automation.Core.Attributes
{
    public class HtmlControlAttribute : Attribute, ICloneable
    {
        public string DomAddressLocator { get; set; }
        public SearchBy SearchBy { get; set; }     
        public RefreshMethod RefreshMethod { get; set; }
        public ControlNotFoundMode ControlNotFoundMode { get; set; }
        public bool AjaxCall { get; set; }
        public bool RefreshArrayControl { get; set; }

        public HtmlControlAttribute(string locator, SearchBy searchBy, bool ajaxCall = false, RefreshMethod refreshMethod=RefreshMethod.OnlyOnceUntilFound, ControlNotFoundMode notFounfMode= ControlNotFoundMode.Continue, bool refreshArrayControl = true)
        {
            DomAddressLocator = locator;
            this.SearchBy = searchBy;           
            RefreshMethod = refreshMethod;
            ControlNotFoundMode = notFounfMode;
            AjaxCall = ajaxCall;
            RefreshArrayControl = refreshArrayControl;
        }

        public By GetSelector()
        {
            switch (SearchBy)
            {
                case SearchBy.Id:
                    return By.Id(DomAddressLocator);
                case SearchBy.Name:
                    return By.Name(DomAddressLocator);
                case SearchBy.CssSelector:
                    return By.CssSelector(DomAddressLocator);                    
                case SearchBy.ClassName:
                    return By.ClassName(DomAddressLocator);
                case SearchBy.Xpath:
                    return By.XPath(DomAddressLocator);
                case SearchBy.Link:
                     return By.LinkText(DomAddressLocator);                    
                case SearchBy.PartialLink:
                     return By.PartialLinkText(DomAddressLocator);
                default:
                     throw new Exception("No Search By selector ...");
            }
        }

        public object Clone()
        {
            return new HtmlControlAttribute(DomAddressLocator = this.DomAddressLocator, SearchBy = this.SearchBy, AjaxCall = this.AjaxCall, RefreshMethod = this.RefreshMethod, ControlNotFoundMode = this.ControlNotFoundMode);
        }
    }


    public enum SearchBy
    {
        Id,
        Name,
        CssSelector,
        ClassName,
        Xpath,
        Link,
        PartialLink,
        JavaScript
    }

    public enum RefreshMethod
    {
        Always,       
        OnlyOnce,
        OnlyOnceUntilFound
    }

    public enum ControlNotFoundMode
    {
        ThrowException,
        Continue
    }
}
