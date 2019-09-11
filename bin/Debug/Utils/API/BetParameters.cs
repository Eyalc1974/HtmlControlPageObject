using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NG.Automation.Core.Utils.API
{
    public class BetParameters
    {

        public string BTP {get;set;}
        public string GAV { get; set; }
        public string PlayMode { get; set; }
        public int GID { get; set; }
        public string UN { get; set; }
        public string ISID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string GM { get; set; } //F - Free games, M - Money bet
        public string LanguageCode { get; set; }
        public string BrandDomain { get; set; }
        public string BrandID { get; set; }
    }
}
