using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NG.Automation.Core.Attributes
{
    public class HtmlGroupControlAttribute : Attribute
    {
        [System.ComponentModel.DefaultValue("")]
        public string ParentElementCSSSelector { get; set; }

        public HtmlGroupControlAttribute(string parentlocator)
        {
            ParentElementCSSSelector = parentlocator;
        }
    }
}
