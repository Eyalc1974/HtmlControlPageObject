using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NG.Automation.Core.Attributes
{
    public class HtmlPageAtrribute : Attribute
    {
        public bool Page { get; private set; }

        public HtmlPageAtrribute(bool page)
        {
            Page = page;
        }
    }
}
