using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Core.Attributes
{
    public class HtmlTableAttribute : HtmlControlAttribute
    {
        [System.ComponentModel.DefaultValue("th")]
        public string TableHeaderTageName { get; set; }
        [System.ComponentModel.DefaultValue("tr")]
        public string TableRowTageName { get; private set; }
        [System.ComponentModel.DefaultValue("td")]
        public string TableDateTageName { get; private set; }

        public HtmlTableAttribute(string locator, SearchBy searchBy, string theader,string trow,string tdata, bool ajaxCall = false, 
            RefreshMethod refreshMethod = RefreshMethod.OnlyOnceUntilFound, ControlNotFoundMode notFounfMode = ControlNotFoundMode.Continue)
            :base(locator, searchBy, ajaxCall, refreshMethod, notFounfMode)
        {
            TableHeaderTageName = theader;
            TableRowTageName = trow;
            TableDateTageName = tdata;
        }       
    }
}
