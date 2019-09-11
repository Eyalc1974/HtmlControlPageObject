using NG.Automation.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NG.Automation.Core.Containers
{
    public abstract class BaseContainer : IBaseContainer
    {
        public WebRunner WebRunner { get; internal set; }
        public IBaseContainer Parent { get; internal set; }
        public BaseContainer(WebRunner webRunner)
        {
            if (webRunner == null)
                throw new ArgumentNullException("webRunner");
            WebRunner = webRunner;                  
        }
       
        public abstract void Focus();
        
        public abstract void WaitToLoad();

        public abstract void WaitToLoadAjax();
    }
}
