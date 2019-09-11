using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NG.Automation.Core.Infrastructure
{
    public class TestProfile
    {
       
        public Dictionary<string, Object> capabilities = new Dictionary<string, object>();

        public int ServerPort {get;set;}

        public string ServerLocation {get; set;}

        public BrowserPlatform Browser {get; set;}

        public bool CloseBrowserWhenDone {get; set;}
      
        public static TestProfile ChromeProfile
        {
            get
            {
                var profile = new TestProfile() { Browser = BrowserPlatform.Chrome };
                //profile.Capabilities.Add("","");
                return profile;
            }
        }

        public static TestProfile IEProfile
        {
            get
            {
                return new TestProfile() { Browser = BrowserPlatform.IE };
            }
        }

        public static TestProfile FireFoxProfile
        {
            get
            {
                return new TestProfile() { Browser = BrowserPlatform.FireFox };
            }
        }
        /// <summary>
        /// Make sure to the define the minimum requeirement of capabilities to work with Mobile
        /// </summary>
        /// <param name="cap"></param>
        /// <returns></returns>
        public static TestProfile Android_4_4(Dictionary<string, Object> cap)
        {
            return new TestProfile() { capabilities = cap };
            
        }
            

    }
}
