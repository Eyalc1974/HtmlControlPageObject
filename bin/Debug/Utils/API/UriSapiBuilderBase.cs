using Microsoft.VisualStudio.TestTools.UnitTesting;
using NG.Automation.Core.Utils.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NG.Automation.Core.Utils.API
{
    public abstract class UriSapiBuilderBase
    {
        Enviroment _enviroment;              
        protected string _playername;
        protected string _username { get; set; }
        protected string _paymentMethodId { get; set; }
        protected string _amount { get; set; }
        protected string _accountNum { get; set; }
        protected string _personNationalId { get; set; }
        protected string _pass { get; set; }
        protected string _uniqeVisitor { get; set; }
        protected string _accountName { get; set; }
        
        public enum Enviroment
        {
            QA,
            UAT,
            PreUAT,
            Prod
        }                  

        public string RegiUri { get; set; }
        public string LoginUri { get; set; }
        public string ISID {get; set;}
        public string BET_Req_GameID {get; set;}
        public string LoginSilentUri {get; set;}
        public string DepmUri {get; set;}
        public string DepositUri { get; set; }
        public string DepositNewUri { get; set; }
        public string GetBalanceUri { get; set; }
        public string GetGamesListUri { get; set; }
        public string RegiGenericUri { get; set; }
        public string DefaultUri { get; set; }
        
           
        protected abstract void SetLoginUri(string playername, Enum enviroment, string brandDomain, string casinoID);        

        protected abstract void SetDepositUri(string username, string isid, string routingNum, string amount, string paymentMethodId, string personNationalId, Enum env);

        protected abstract void SetDepositNewUri(string username, string isid, string routingNum, string amount, string personNationalId, string accountName, Enum env);

        protected abstract void SetDepositNewUri(DepositCCParameters depositParams, Enum env);

    }
}
