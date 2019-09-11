using NG.Automation.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NG.Automation.Core.Containers
{
    //Base class for group control.
    public abstract class BaseGroupControlPage : BaseContainer
    {        
        public BasePage PageContainer { get; set; }

        public BaseGroupControlPage(BasePage page)  
            :base(page.WebRunner)
        {
            if (page == null)
                throw new ArgumentNullException("page");
            PageContainer = page;            
        }

        public override void Focus()
        {
            PageContainer.Focus();
        }

        public override void WaitToLoad()
        {
            //PageContainer.WaitToLoad(); 
            // Use the WebRunner
            WebRunner.WaitForPageToLoad();
        }

        public override void WaitToLoadAjax()
        {

            WebRunner.WaitForPageToLoadAjax();
        }
        
    }
}
