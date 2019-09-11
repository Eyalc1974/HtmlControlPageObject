using NG.Automation.Core.Utils.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using NG.Automation.Core.Logging;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NG.Automation.Core.CommonWorkflows;


namespace NG.Automation.Core.Utils.API
{
    public class CommonAPI
    {
        public enum LotteryGame
        {
            Lotto47,
            MegaMillion,
            PowerBall,
            Fantasy5
        }

        public static Dictionary<string, string> dbgLogoGamesDictionary = new Dictionary<string, string>()
        {
        
           {"PowerBall","500"},
           {"MegaMillion","502"},
           {"Fantasy5","503"},
           {"Lotto47","505"}        
        };

         public static Dictionary<int, string> gameBtpSeries1 = new Dictionary<int, string>()
         {
                {264,"1-=20"},//Diamond Payout
                {250,"5-1,1,1,1,1=0.5"},//Role the dice
                {474,"1-1,0,0=0.5"},//STAR Stripes
                {468,"1-1,0,0,0,0,0,0=10"},//Big League Raffle
                {467,"12-=0.05"},//Cash
                {443,"20-=0.25"},//HIT               
         };

         public static Dictionary<int, string> gameBtpSeries2 = new Dictionary<int, string>()
         {               
                {457,"6-=0.25"},//SWEET WINNER
                {431,"18-=0.05"},//Queen of diamond
                {413,"1-=0.5"},//Tax Free
                {410,"1-4,17,20,33,52,53,63,72,75,79=0.5"},//Fuzzball Keno
                {389,"1-=2"},//Cosmic Expender
                {390,"1-=0.50"},//Golden Island               

         };

         public static Dictionary<int, string> gameBtpSeries3 = new Dictionary<int, string>()
         {               
                {384,"1-1,0,0=0.5"},//Intant Football payout
                {364,"1-=0.5"},//Money in the Mitten
                {346,"3-1,1,1=1"},//Triplr money math
                {343,"1-=5"},//Wizard of All
                {272,"20-=0.5"},//VIP
                {273,"7-1,1,1,1,1,1,1=0.5"},//MEGA Bucks               

         };

         public static Dictionary<int, string> gameBtpSeries4 = new Dictionary<int, string>()
         {              
                {274,"7-=0.25"},//Triple 777
                {266,"8-=5"},//Pot O'Gold
                {270,"1-1,0,0=1"},//10xTheCash
                {259,"1-=2"},//WildTime
                {276,"1-1,0,0=0.50"},//Alfredo
                {275,"1-1,0,0=1"},//Pure Gold
                {260,"1-=2"},//Bingo Doubler

         };

         public static UriSapiBuilderBase.Enviroment GetEnumBasedOnURL(string myUrl)
         {

             if ((myUrl.Contains("promotest") || myUrl.Contains(".qa.") || myUrl.Contains("qa.") || myUrl.Contains("wwwint")) && !myUrl.Contains(".uat.") && !myUrl.Contains(".pre-uat."))
             {
                 return UriSapiBuilderBase.Enviroment.QA;
             }
             else
             {
                 if (!(myUrl.Contains("pre-uat.")))
                 {
                     if ((myUrl.Contains("review") || myUrl.Contains(".uat.") || myUrl.Contains("wwwqa")))
                         return UriSapiBuilderBase.Enviroment.UAT;
                 }
                 else
                     return UriSapiBuilderBase.Enviroment.PreUAT;
                 //production - optional               
                 //else
                 //    backEndPage.Login(backEndPage.userNamePreUAT, backEndPage.passwordPreUAT);                
             }
             return UriSapiBuilderBase.Enviroment.Prod;
         }

         public static JObject Michigan_API_Registration(string username, string pass, string url)
         {
             Log.WriteMessage("Sending API request for Create new JAUQLINE user, user : " + username);
             RegiParameters parms = new RegiParameters();
             parms.UserName = username;
             parms.Password = pass;
             parms.uniqeVistor = "2122E9E6A9FFB6B96D0D3FF3F589E582";
             UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             Michigan_UriSapiBuilder call = new Michigan_UriSapiBuilder(env, parms);                             
             HTTP_Request req = new HTTP_Request(call.RegiGenericUri,url);
             req.RunRegiPost_Request();
             //Assert.IsTrue(req.Response["S2C"]["RESULT"]["CODE"].ToString().Contains("0"), "Error code from Regi request is not 0");             
             return req.Response;
         }

       // public static JObject Michigan_API_StanelyRegistration(IWebDriver driver, string username, string pass, string url)
       // {

       //     OpenningPageMichigan michiganPage1 = new OpenningPageMichigan(driver);
       //     driver.Navigate().GoToUrl(url);

       //     AutomationFramework.Utils.API.RegiParameters parms = new AutomationFramework.Utils.API.RegiParameters();
       //     parms.Address = "7859 OCEANVIEW";
       //     parms.BirthDay = "1947-08-01";
       //     parms.City = "LANSING";
       //     parms.FirstName = "Stanely";
       //     parms.LastName = "Henshaw";
       //     parms.Password = pass;
       //     parms.PersonNationalID = "1321";
       //     parms.uniqeVistor = "";
       //     parms.UserName = username;
       //     parms.Zip = "48908";

       //     AutomationFramework.Utils.API.UriBuilder loginUri = new AutomationFramework.Utils.API.UriBuilder(AutomationFramework.Utils.API.UriBuilder.Enviroment.QA,
       //         parms);
       //     AutomationFramework.Utils.API.HTTP_Request req = new AutomationFramework.Utils.API.HTTP_Request(driver, loginUri.RegiGenericUri);
       //     req.RunPost_LoginRequest(pass);
       //     Log.WriteLine("Sending API request for Create new Stanely user, user : " + username);
       //     return req.Response;

       // }
       // /// <summary>
       // /// Login API for user name,
       // /// </summary>
       // /// <param name="driver"></param>
       // /// <param name="username"></param>
       // /// <returns></returns>

         public static JObject Michigan_API_Login(string username, string pass, string siteUrl)
         {
             Log.WriteMessage("Login with : " + username);
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(siteUrl);
             Michigan_UriSapiBuilder loginCall = new Michigan_UriSapiBuilder(env, username);
             HTTP_Request reqLogin = new HTTP_Request(loginCall.LoginUri,siteUrl);
             reqLogin.RunPost_LoginRequest(pass,username,"USD");

             //Assert.IsTrue(reqLogin.Response["S2C"]["RESULT"]["CODE"].ToString().Contains("0"),
             //    "Error code from Regi request is not 0, actual code error is:  " + reqLogin.Response["S2C"]["RESULT"]["CODE"].ToString());
             
             return reqLogin.Response;

         }
         /// <summary>
         /// Bet only can perform only when you pass the username,pass,url and ISID from login response - json["S2C"]["PLAYER"]["ISID"]
         /// 
         /// </summary>
         /// <param name="driver"></param>
         /// <param name="username"></param>
         /// <param name="pass"></param>
         /// <param name="url"></param>
         /// <param name="isid">get from login/regi response</param>
         /// <returns></returns>
         public static JObject Michigan_API_Bet_Depositor(string url, NG.Automation.Core.Utils.API.BetParameters parms)
         {     
        //bet
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             Michigan_UriSapiBuilder betCall = new Michigan_UriSapiBuilder(env, parms);
             //  AutomationFramework.Utils.API.UriBuilder betCall = new AutomationFramework.Utils.API.UriBuilder(env, AutomationFramework.Utils.API.UriBuilder.Calls.Bet, isid, username, gameid);
             HTTP_Request betRequest = new HTTP_Request(betCall.BET_Req_GameID,url);
             Log.WriteMessage("Bet request:" + betRequest.RequestURI);
             betRequest.RunGET_Request();
             Log.WriteMessage("Bet response:" + betRequest.Response);
             return betRequest.Response;

         }

         /// <summary>
         /// Depm API to return the player's last approved ACH details
         /// </summary>
         /// <param name="driver"></param>
         /// <param name="username"></param>
         /// <returns></returns>
         public static JObject Michigan_API_Depm(string username, string isid, string url)
         {
             Log.WriteMessage("Depm with : " + username);
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             Michigan_UriSapiBuilder depmCall = new Michigan_UriSapiBuilder(env,isid, username);
             HTTP_Request reqDepm = new HTTP_Request(depmCall.DepmUri,url);
             reqDepm.RunGET_DepmRequest();
             //Assert.IsTrue(reqDepm.Response["S2C"]["RESULT"]["CODE"].ToString().Contains("0"),
             //    "Error code from Depm request is not 0, actual code error is:  " + reqDepm.Response["S2C"]["RESULT"]["CODE"].ToString());             
             return reqDepm.Response;

         }

         /// <summary>
         /// Get games list instant and DBG
         /// </summary>
         /// <param name="driver"></param>
         /// <param name="env"></param>
         /// <returns></returns>
         public static JObject Michigan_API_GetGamesListResponse(string url)
         {
             Log.WriteMessage("Sending GetGamesListRequest");
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             HTTP_Request reqGGL = new HTTP_Request(NG.Automation.Core.Utils.API.Michigan_UriSapiBuilder.SetGetGameListUri(env), url);
             reqGGL.RunGET_Request();
             //Assert.IsTrue(reqGGL.Response["S2C"]["RESULT"]["CODE"].ToString().Contains("0"),
             //   "Error code from Depm request is not 0, actual code error is:  " + reqGGL.Response["S2C"]["RESULT"]["CODE"].ToString());
             return reqGGL.Response;
         }

         /// <summary>
         /// Deposit API with existing ACH
         /// </summary>
         /// <param name="driver"></param>
         /// <param name="username"></param>
         /// <param name="isid"></param>
         /// <param name="accountNum"></param>
         /// <param name="amount"></param>
         /// <param name="paymentMethodId"></param>
         /// <param name="personNantionalId"></param>
         /// <returns></returns>
         public static JObject Michigan_API_ACH_Deposit_With_Depositor(DepositACHParameters parms, string url)
         {
             Log.WriteMessage("Deposit with : " + parms.PlayerName);
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             DepositACHParameters parmsForDep = new DepositACHParameters();
             parmsForDep = CheckDepParm(parms, parmsForDep);             

             //string username, 
             Michigan_UriSapiBuilder depositCall = new Michigan_UriSapiBuilder(env, parmsForDep);
             HTTP_Request reqDeposit = new HTTP_Request(depositCall.DepositUri,url);
             reqDeposit.RunPOST_DepositRequest(parmsForDep);
             //Assert.IsTrue(reqDeposit.Response["S2C"]["RESULT"]["CODE"].ToString().Contains("0"),
             //    "Error code from Deposit request is not 0, actual code error is:  " + reqDeposit.Response["S2C"]["RESULT"]["CODE"].ToString());
             
             Log.WriteMessage("Deposit Response : " + reqDeposit.Response);
             return reqDeposit.Response;

         }

         private static DepositACHParameters CheckDepParm(DepositACHParameters parms, DepositACHParameters parmsForDep)
         {
             if (parms.ISID == null)
                 Assert.Fail("No ISID was define for Michigan_API_ACH_Deposit_With_Depositor() func in CommonAPI.cs");
             else parmsForDep.ISID = parms.ISID;
             if (parms.AccountNumber == null && parms.Depositor == true)
                 Assert.Fail("No AccountNumber was define for Michigan_API_ACH_Deposit_With_Depositor() func in CommonAPI.cs");
             else parmsForDep.AccountNumber = parms.AccountNumber;
             if (parms.AccountName == null)
                 Assert.Fail("No AccountName was define for Michigan_API_ACH_Deposit_With_Depositor() func in CommonAPI.cs");
             else parmsForDep.AccountName = parms.AccountName;
             if (parms.Amount == null)
                 Assert.Fail("No Amount was define for Michigan_API_ACH_Deposit_With_Depositor() func in CommonAPI.cs");
             else parmsForDep.Amount = parms.Amount;
             if (parms.PersonalNationalID == null)
                 Assert.Fail("No PersonalNationalID was define for Michigan_API_ACH_Deposit_With_Depositor() func in CommonAPI.cs");
             else parmsForDep.PersonalNationalID = parms.PersonalNationalID;
             if (parms.PaymentMethodID == null && parms.Depositor == true)
                 Assert.Fail("No PaymentMethodID was define for Michigan_API_ACH_Deposit_With_Depositor() func in CommonAPI.cs");
             else parmsForDep.PaymentMethodID = parms.PaymentMethodID;
             if (parms.PlayerName == null)
                 Assert.Fail("No PlayerName was define for Michigan_API_ACH_Deposit_With_Depositor() func in CommonAPI.cs");
             else parmsForDep.PlayerName = parms.PlayerName;
             parmsForDep.Depositor = parms.Depositor;
             return parmsForDep;

         }

         private static DepositCCParameters CheckCreditCardDepParm(DepositCCParameters parms, DepositCCParameters parmsForDep)
         {
             if (parms.ISID == null)
                 Assert.Fail("No ISID was define for Malta_API_Credit_Deposit_With_NonDepositor() func in CommonAPI.cs");
             else parmsForDep.ISID = parms.ISID;
             if (parms.AccountNumber == null && parms.Depositor == true)
                 Assert.Fail("No AccountNumber was define for Malta_API_Credit_Deposit_With_NonDepositor() func in CommonAPI.cs");
             else parmsForDep.AccountNumber = parms.AccountNumber;
             if (parms.AccountName == null)
                 Assert.Fail("No AccountName was define for Malta_API_Credit_Deposit_With_NonDepositor() func in CommonAPI.cs");
             else parmsForDep.AccountName = parms.AccountName;
             if (parms.Amount == null)
                 Assert.Fail("No Amount was define for Malta_API_Credit_Deposit_With_NonDepositor() func in CommonAPI.cs");
             else parmsForDep.Amount = parms.Amount;
             if (parms.LanguageCode == null)
                 Assert.Fail("No Languagecode was define for Malta_API_Credit_Deposit_With_NonDepositor() func in CommonAPI.cs");
             else parmsForDep.LanguageCode = parms.LanguageCode;
             if (parms.PaymentMethodID == null && parms.Depositor == true)
                 Assert.Fail("No PaymentMethodID was define for Malta_API_Credit_Deposit_With_NonDepositor() func in CommonAPI.cs");
             else parmsForDep.PaymentMethodID = parms.PaymentMethodID;
             if (parms.PlayerName == null)
                 Assert.Fail("No PlayerName was define for Malta_API_Credit_Deposit_With_NonDepositor() func in CommonAPI.cs");
             else parmsForDep.PlayerName = parms.PlayerName;
             parmsForDep.Depositor = parms.Depositor;
             if (parms.CurrencyCode == null)
                 Assert.Fail("No CurrencyCode was define for Malta_API_Credit_Deposit_With_NonDepositor() func in CommonAPI.cs");
             else parmsForDep.CurrencyCode = parms.CurrencyCode;
             if (parms.CVV == null)
                 Assert.Fail("No CVV was define for Malta_API_Credit_Deposit_With_NonDepositor() func in CommonAPI.cs");
             else parmsForDep.CVV = parms.CVV;
             if (parms.Expiration == null)
                 Assert.Fail("No Expiration was define for Malta_API_Credit_Deposit_With_NonDepositor() func in CommonAPI.cs");
             else parmsForDep.Expiration = parms.Expiration;
             if (parms.ExpirationMonth == null)
                 Assert.Fail("No ExpirationMonth was define for Malta_API_Credit_Deposit_With_NonDepositor() func in CommonAPI.cs");
             else parmsForDep.ExpirationMonth = parms.ExpirationMonth;
             if (parms.ExpirationYear == null)
                 Assert.Fail("No ExpirationYear was define for Malta_API_Credit_Deposit_With_NonDepositor() func in CommonAPI.cs");
             else parmsForDep.ExpirationYear = parms.ExpirationYear;
             parmsForDep.Depositor = parms.Depositor;
             return parmsForDep;

         }
        
         public static JObject Michigan_API_ACH_Deposit_Non_Depositor(DepositACHParameters parms, string url)
         {
             Log.WriteMessage("Deposit with : " + parms.PlayerName);
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             DepositACHParameters parmsForDep = new DepositACHParameters();
             parmsForDep = CheckDepParm(parms, parmsForDep);
             Michigan_UriSapiBuilder depositCall = new Michigan_UriSapiBuilder(env, parms);
             HTTP_Request reqDeposit = new HTTP_Request(depositCall.DepositNewUri,url);
             reqDeposit.RunPOST_DepositRequest(parmsForDep);

            // if ((reqDeposit.Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0")))
            //    Log.WriteMessage("Response from server is passed with code = 0");
            //else
            //{
            //    Log.WriteMessage("Request : " + reqDeposit);
            //    Log.WriteMessage("Response : " + reqDeposit.Response);
            //}

             return reqDeposit.Response;
         }


         public static void VA_API_Registration(string username, string pass, string url)
         {
             Log.WriteMessage("Sending API request for Create new user, user : " + username);
             RegiParameters parms = new RegiParameters();
             parms.UserName = username;
             parms.Password = pass;
             parms.uniqeVistor = "2122E9E6A9FFB6B96D0D3FF3F589E582";
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment envUrl = GetEnumBasedOnURL(url);
             Virginia_UriSapiBuilder call = new Virginia_UriSapiBuilder(envUrl,parms);
             HTTP_Request req = new HTTP_Request(call.RegiGenericUri, url);
             req.RunRegiPost_Request();
             Assert.IsTrue(req.Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0"), 
                 string.Format("Error code from Regi request is not 0, Error code is: {0}",req.Response["S2C"]["RESULT"]["CODE"].ToString()));
             
         }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pass"></param>
        /// <param name="siteUrl"></param>
        /// <returns>JSON, take the ISID for the next steps</returns>
         public static JObject VA_API_Login(string username, string pass, string siteUrl)
         {
             Log.WriteMessage("Login with : " + username);
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(siteUrl);
             Virginia_UriSapiBuilder loginCall = new Virginia_UriSapiBuilder(env, username);
             HTTP_Request reqLogin = new HTTP_Request(loginCall.LoginUri, siteUrl);
             reqLogin.RunPost_LoginRequest(pass,username,"USD");

             //Assert.IsTrue(reqLogin.Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0"),
             //    "Error code from Login request is not 0, actual code error is:  " + reqLogin.Response["S2C"]["RESULT"]["CODE"].ToString());             
             return reqLogin.Response;

         }

         public static JObject VA_API_Depm(string username, string isid, string url)
         {
             Log.WriteMessage("Depm with : " + username);
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             Virginia_UriSapiBuilder depmCall = new Virginia_UriSapiBuilder(env, isid, username);
             HTTP_Request reqDepm = new HTTP_Request(depmCall.DepmUri, url);
             reqDepm.RunGET_DepmRequest();
             //Assert.IsTrue(reqDepm.Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0"),
             //    "Error code from Depm request is not 0, actual code error is:  " + reqDepm.Response["S2C"]["RESULT"]["CODE"].ToString());             
             return reqDepm.Response;

         }
         /// <summary>
         /// first do VA_API_Login, take the ISID to VA_API_Depm() then take PersonalNationalID +AccountNumber + paymentMethodID
         /// and Set parms in DepositACHParameters, mandatory: PersonalNationalID + ISID +AccountNumber + Depositor + paymentMethodID
         /// </summary>
        /// <param name="parms"></param>
        /// <param name="url"></param>
        /// <returns></returns>
         public static JObject VA_API_ACH_Deposit_With_Depositor(DepositACHParameters parms, string url)
         {
             Log.WriteMessage("Deposit with: " + parms.PlayerName);             
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             DepositACHParameters parmsForDep = new DepositACHParameters();
             parmsForDep = CheckDepParm(parms, parmsForDep);

             //string username, 
             Virginia_UriSapiBuilder depositCall = new Virginia_UriSapiBuilder(env, parmsForDep);
             HTTP_Request reqDeposit = new HTTP_Request(depositCall.DepositUri, url);
             reqDeposit.RunPOST_DepositRequest(parmsForDep);
             //Assert.IsTrue(reqDeposit.Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0"),
             //    "Error code from Deposit request is not 0, actual code error is:  " + reqDeposit.Response["S2C"]["RESULT"]["CODE"].ToString());             
             return reqDeposit.Response;

         }
        /// <summary>
         /// first do VA_API_Login, take the ISID to VA_API_Depm() then take PersonalNationalID 
         /// and Set parms in DepositACHParameters, mandatory: PersonalNationalID + ISID 
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="url"></param>
        /// <returns>JSON with resutls </returns>
         public static JObject VA_API_ACH_Deposit_Non_Depositor(DepositACHParameters parms, string url)
         {
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             DepositACHParameters parmsForDep = new DepositACHParameters();
             parmsForDep = CheckDepParm(parms, parmsForDep);

             Virginia_UriSapiBuilder depositCall = new Virginia_UriSapiBuilder(env, parmsForDep);
             HTTP_Request reqDeposit = new HTTP_Request(depositCall.DepositNewUri, url);
             reqDeposit.RunPOST_DepositRequest(parmsForDep);
             //Assert.IsTrue(reqDeposit.Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0"),
             //   "Error code from Deposit request is not 0, actual code error is: " + reqDeposit.Response["S2C"]["RESULT"]["CODE"].ToString());
             Log.WriteMessage("Deposit with: " + parms.PlayerName);
             return reqDeposit.Response;
         }

         public static JObject VA_API_GetGamesListResponse(string url)
         {
             Log.WriteMessage("Sending Get Games list API request");
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             HTTP_Request reqGGL = new HTTP_Request(NG.Automation.Core.Utils.API.Virginia_UriSapiBuilder.SetGetGameListUri(env), url);
             reqGGL.RunGET_Request();
             //Assert.IsTrue(reqGGL.Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0"),
             //   "Error code from Get Games List request is not 0, actual code error is: "+reqGGL.Response["S2C"]["RESULT"]["CODE"].ToString());
             return reqGGL.Response;
         }

        /// <summary>
        /// We use it to get Max future draws of DBG game
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Return JObject for ticker call</returns>
         public static JObject VA_API_Ticker(string url)
         {
             Log.WriteMessage("Sending Ticker API request");
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             HTTP_Request reqTicker = new HTTP_Request(NG.Automation.Core.Utils.API.Virginia_UriSapiBuilder.SetTickerUri(env), url);
             reqTicker.RunGET_TickerRequest();
             //Assert.IsTrue(reqTicker.Response["RESULT"]["CODE"].ToString().Equals("0"),
             //   "Error code from Ticker request is not 0, actual code error is: " + reqTicker.Response["RESULT"]["CODE"].ToString());
             return reqTicker.Response;
         }

        /// <summary>
        /// Request PLI (Pre Login Information), we use to get min deposit amount
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
         public static JObject VA_API_PLI(string url)
         {
             Log.WriteMessage("Sending PLI API request");
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             HTTP_Request reqPLI = new HTTP_Request(NG.Automation.Core.Utils.API.Virginia_UriSapiBuilder.SetPLIUri(env), url);
             reqPLI.RunGET_Request();
             return reqPLI.Response;
         }

         /// <summary>
         /// Malta API registration non UK player
         /// </summary>
         /// <param name="username"></param>
         /// <param name="pass"></param>
         /// <param name="url"></param>
         ///<param name="brandDomain">brand domain lower case</param>
         /// <param name="brandId">Casino id of the brand</param>
         public static JObject Malta_API_Registration(string username, string pass, string url,RegiParameters parms)
         {
             Log.WriteMessage("Sending API request to create new user, username : " + username);             
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment envUrl = GetEnumBasedOnURL(url);

             if (string.IsNullOrEmpty(username)) parms.UserName = "automation"; else parms.UserName = username;
             if (string.IsNullOrEmpty(pass)) parms.Password = "12345678as"; else parms.Password = pass;
             if (string.IsNullOrEmpty(parms.uniqeVistor)) parms.uniqeVistor = "2122E9E6A9FFB6B96D0D3FF3F589E582";             
             if (string.IsNullOrEmpty(parms.Email)) parms.Email = username +"@neogames.com";
             if (string.IsNullOrEmpty(parms.CurrencyCode)) parms.CurrencyCode = "GBP";             
             if (string.IsNullOrEmpty(parms.Address)) parms.Address = CommonWorkflow.GenerateStreetNumber() + " ST";
             if (string.IsNullOrEmpty(parms.PhonePrefix)) parms.PhonePrefix = "44";             
             
             Malta_UriSapiBuilder call = new Malta_UriSapiBuilder(envUrl, parms);
             HTTP_Request req = new HTTP_Request(call.RegiGenericUri, url);
             req.RunRegiPost_Request(parms);
             Assert.IsTrue(req.Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0"),
                 string.Format("Error code from Regi request is not 0, Error code is: {0}", req.Response["S2C"]["RESULT"]["CODE"].ToString()));

             return req.Response;
         }

        /// <summary>
         /// Malta API Login
        /// </summary>
        /// <param name="username">Player username</param>
        /// <param name="pass">Player pass</param>
        /// <param name="siteUrl">Url from CSV</param>
        /// <param name="brandDomain">brand domain in lower case</param>
        /// <param name="casinoId">CSI as string</param>
        /// <param name="currencyCode">3 letters as string, for example: United Kingdom Pound will be send as GBP, Norway Krone as NOK</param>
        /// <returns></returns>
         public static JObject Malta_API_Login(string username, string pass, string siteUrl,string brandDomain, string casinoId,
             string currencyCode)
         {
             Log.WriteMessage("Login with : " + username);
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(siteUrl);
             Malta_UriSapiBuilder loginCall = new Malta_UriSapiBuilder(env, username,brandDomain,casinoId);
             HTTP_Request reqLogin = new HTTP_Request(loginCall.LoginUri, siteUrl);
             reqLogin.RunPost_LoginRequest(pass,username,currencyCode);
             Assert.IsTrue(reqLogin.Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0"),
                 string.Format("Error code from login request is not 0, Error code is: {0}", reqLogin.Response["S2C"]["RESULT"]["CODE"].ToString()));
             return reqLogin.Response;

         }

         /// <summary>
         /// Bet only can perform only when you pass the username,pass,url and ISID from login response - json["S2C"]["PLAYER"]["ISID"]
         /// 
         /// </summary>
         /// <param name="driver"></param>
         /// <param name="username"></param>
         /// <param name="pass"></param>
         /// <param name="url"></param>
         /// <param name="isid">get from login/regi response</param>
         /// <returns></returns>
         public static JObject Malta_API_Bet_Depositor(string url, NG.Automation.Core.Utils.API.BetParameters parms)
         {
             //bet
             Log.WriteMessage(string.Format("Bet API on game id {0}",parms.GID));
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             Malta_UriSapiBuilder betCall = new Malta_UriSapiBuilder(env, parms);
             
             HTTP_Request betRequest = new HTTP_Request(betCall.BET_Req_GameID, url);
             Log.WriteMessage("Bet request:" + betRequest.RequestURI);
             betRequest.RunGET_Request();
             Log.WriteMessage("Bet response:" + betRequest.Response);
             return betRequest.Response;

         }

        /// <summary>
        /// Malta Credit card API deposit
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="url">URL from CSV</param>
        /// <returns></returns>
         public static JObject Malta_API_CreditCard_Deposit_Non_Depositor(DepositCCParameters parms, string url)
         {
             Log.WriteMessage("Deposit with : " + parms.PlayerName);
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment env = GetEnumBasedOnURL(url);
             DepositCCParameters parmsForDep = new DepositCCParameters();
             parmsForDep = CheckCreditCardDepParm(parms, parmsForDep);
             Malta_UriSapiBuilder depositCall = new Malta_UriSapiBuilder(env, parms);
             HTTP_Request reqDeposit = new HTTP_Request(depositCall.DepositNewUri, url);
             reqDeposit.RunPOST_DepositCreditCardRequest(parmsForDep);
             //Log.WriteMessage("Deposit Response : " + reqDeposit.Response);
             return reqDeposit.Response;
         }

        /// <summary>
         /// Sazka API registration
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <param name="url">URL from CSV</param>
        /// <param name="parms">RegiParameters: regiParameters.CellPhone = CommonWorkflow_SZ.GeneratePhoneNumber(), 
        ///                     regiParameters.PersonNationalID = CommonWorkflow_SZ.GenerateDocumentNumber();
        /// </param>
        /// <returns>JObject to check that registration passed</returns>
         public static JObject Sazka_API_Registration(string email, string pass, string url, RegiParameters parms = null)
         {
             Log.WriteMessage("Sending API request to create new user, username : " + email);
             NG.Automation.Core.Utils.API.UriSapiBuilderBase.Enviroment envUrl = GetEnumBasedOnURL(url);
             if (parms == null)
             {
                 parms = new RegiParameters();
             }

             if (string.IsNullOrEmpty(email)) parms.Email = CommonWorkflow.GeneratePlayerName() + "@ng.com"; else parms.Email = email;
             if (string.IsNullOrEmpty(pass)) parms.Password = "A123456y"; else parms.Password = pass;
             if (string.IsNullOrEmpty(parms.CurrencyCode)) parms.CurrencyCode = "CZK";
             if (string.IsNullOrEmpty(parms.PhonePrefix)) parms.PhonePrefix = "420";
             if (string.IsNullOrEmpty(parms.BrandID)) parms.BrandID = "120";
             if (string.IsNullOrEmpty(parms.PersonNationalID)) parms.PersonNationalID = CommonWorkflows.CommonWorkflow.GenerateDocumentNumber();
             if (string.IsNullOrEmpty(parms.CellPhone)) parms.CellPhone = CommonWorkflows.CommonWorkflow.GeneratePhoneNumber();
             Sazka_UriSapiBuilder call = new Sazka_UriSapiBuilder(envUrl, parms);
             HTTP_Request req = new HTTP_Request(call.RegiGenericUri, url);
             req.RunRegiPost_Request(parms);
             return req.Response;
         }
    }
}
