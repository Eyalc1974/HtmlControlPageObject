using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NG.Automation.Core.Attributes
{
    public class HtmlFrameAttribute : Attribute
    {
        public string FrameName { get; private set; }

        public HtmlFrameAttribute(string frameName)
        {
            if (string.IsNullOrEmpty(frameName))
                throw new ArgumentException("frameName");
            FrameName = frameName;
        }
    }
}
