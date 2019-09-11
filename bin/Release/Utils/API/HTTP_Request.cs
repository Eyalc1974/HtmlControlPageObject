using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NG.Automation.Core.Logging;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace NG.Automation.Core.Utils.API
{
    public class HTTP_Request
    {

       
        private string _siteUrl;
        public string RequestURI { get; set; }
        public JObject Response { get; set; }
        public string HeaderUserAgent { get; set; }                    
        public static string DefaultCookiesWebDriver  {get; set;}

        public HTTP_Request(string apiUri,string siteUrl, string header = "None", string domainCookies = ".michiganlottery.com")
        {
            _siteUrl = siteUrl;
            RequestURI = apiUri;
            if (header == "None")
                //HeaderUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
                HeaderUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1";
            else HeaderUserAgent = header;

            if ((CookiesAPI.Cookies == null || CookiesAPI.Cookies == ""))
                CookiesAPI.Cookies = SetDefaultCookeisInHeaderByWebDriver(domainCookies);
        }

        public void RunRegiPost_Request()
        {
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.UserAgent] = HeaderUserAgent;
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            webClient.Headers[HttpRequestHeader.Cookie] = SetDefaultCookeisInHeaderByWebDriver(_siteUrl);
            string responseBytes = webClient.UploadString(RequestURI, "POST");
            CookiesAPI.Cookies = webClient.ResponseHeaders["Set-Cookie"];

            webClient.Dispose();
            Response = JObject.Parse(responseBytes);
            if ((Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0")))
                Log.WriteMessage("Response from server is passed with code = 0");
            else
            {
                Log.WriteMessage("Request : " + RequestURI);
                Log.WriteMessage("Response : " + responseBytes);
            }

        }

        public void RunRegiPost_Request(RegiParameters regiParameters)
        {
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.UserAgent] = HeaderUserAgent;
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            webClient.Headers[HttpRequestHeader.Cookie] = SetDefaultCookeisInHeaderByWebDriver(_siteUrl);
            string responseBytes = string.Empty;
            //Sazka
            if (regiParameters.BrandID.Equals("120"))
            {
                responseBytes = webClient.UploadString(RequestURI, "POST", "email=" + regiParameters.Email +
                "&username=" + regiParameters.Email + "&password="+regiParameters.Password+"&Citizenship=CZ&CountryCode=CZ&CurrencyCode=" + regiParameters.CurrencyCode +
                "&DocumentNumber=" + regiParameters.PersonNationalID + "&address=" + CommonWorkflows.CommonWorkflow.GenerateStreetNumber()
                 + "&cellPhone=" + regiParameters.PhonePrefix + regiParameters.CellPhone +
                 "&city=autoc&firstname=Wui&lastname=NNGUYEN&phone=" + regiParameters.CellPhone +
                 "&ZipCode=68654&IntendedGamblingActivityList=medium&UniqueVisitorID="
                + regiParameters.uniqeVistor);
            }                         
            else
            {
                responseBytes = webClient.UploadString(RequestURI, "POST", "email=" + regiParameters.Email + "&username=" + regiParameters.UserName +
                    "&password=" + regiParameters.Password + "&birthday=1996-03-03&gender=male&CurrencyCode=" + regiParameters.CurrencyCode +
                    "&address=" + CommonWorkflows.CommonWorkflow.GenerateStreetNumber() + "&cellPhone=" + regiParameters.PhonePrefix +
                    "99832133&phone=99832133&city=autoc&firstname=automation&lastname=autom&ZipCode=68654&IntendedGamblingActivityList=medium&UniqueVisitorID="
                    + regiParameters.uniqeVistor);
            }            

            webClient.Dispose();
            CookiesAPI.Cookies = webClient.ResponseHeaders["Set-Cookie"];
            Response = JObject.Parse(responseBytes);
            if ((Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0")))
                Log.WriteMessage("Response from server is passed with code = 0");
            else
            {
                Log.WriteMessage("Request : " + RequestURI);
                Log.WriteMessage("Response : " + responseBytes);
            }

        }        

        public void RunGET_Request()
        {
            WebClient webClient = new WebClient();        
            webClient.Headers[HttpRequestHeader.UserAgent] = HeaderUserAgent;
            webClient.Headers[HttpRequestHeader.ContentType] = "text/plain; charset=utf-8";
            webClient.Headers[HttpRequestHeader.Accept] = "application/json, text/plain, */*"; //"*/*";
            webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en-US,en;q=0.8";
            //webClient.Headers[HttpRequestHeader.Host] = "gamesrv1.qa.michiganlottery.com";
            if (string.IsNullOrEmpty(CookiesAPI.Cookies))
                CookiesAPI.Cookies = DefaultCookiesWebDriver;
            webClient.Headers[HttpRequestHeader.Cookie] = CookiesAPI.Cookies;

            // remove certificate of SSL connection
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback( delegate { return true; } );

            System.Threading.Thread.Sleep(1000);
            string responseBytes = webClient.DownloadString(RequestURI);            
            if(!RequestURI.Contains("CM=BET"))
                CookiesAPI.Cookies = webClient.ResponseHeaders["Set-Cookie"];
            Thread.Sleep(3000);
            string jsonText = string.Empty;
            if (responseBytes.Contains("xml"))
            {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(responseBytes);
                jsonText = JsonConvert.SerializeXmlNode(doc);
            }
            else jsonText = responseBytes;

            webClient.Dispose();
            Response = JObject.Parse(jsonText);
            if ((Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0")))
                Log.WriteMessage("Response from server is passed with code = 0");
            else
            {
                Log.WriteMessage("Request : " + RequestURI);
                Log.WriteMessage("Response : " + responseBytes);
            }
        }

        public void RunGET_DepmRequest()
        {
            WebClient webClient = new WebClient();
            //webClient.Headers["User-Agent"] = "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1";
            webClient.Headers["User-Agent"] = HeaderUserAgent;
            webClient.Headers[HttpRequestHeader.Cookie] = CookiesAPI.Cookies;
            webClient.Headers["Content-Type"] = "text/plain; charset=utf-8";            
            webClient.Headers["Accept"] = "application/json, text/plain, */*";
            webClient.Headers["Accept-Language"] = "en-US,en;q=0.8,fi;q=0.6";
            string responseBytes = webClient.DownloadString(RequestURI);            
            // Cookies set is disabled as it's not needed in VA Widget
            //CookiesAPI.Cookies = webClient.ResponseHeaders["Set-Cookie"];
            Thread.Sleep(3000);
            string jsonText = responseBytes;
            webClient.Dispose();
            Response =JObject.Parse(jsonText);
            if ((Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0")))
                Log.WriteMessage("Response from server is passed with code = 0");
            else
            {
                Log.WriteMessage("Request : " + RequestURI);
                Log.WriteMessage("Response : " + responseBytes);
            }

        }

        public void RunGET_TickerRequest()
        {
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.UserAgent] = HeaderUserAgent;
            webClient.Headers[HttpRequestHeader.ContentType] = "text/plain; charset=utf-8";
            webClient.Headers[HttpRequestHeader.Accept] = "application/json, text/plain, */*"; //"*/*";
            webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en-US,en;q=0.8";
            //webClient.Headers[HttpRequestHeader.Host] = "gamesrv1.qa.michiganlottery.com";
            if (string.IsNullOrEmpty(CookiesAPI.Cookies))
                CookiesAPI.Cookies = DefaultCookiesWebDriver;
            webClient.Headers[HttpRequestHeader.Cookie] = CookiesAPI.Cookies;

            // remove certificate of SSL connection
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

            System.Threading.Thread.Sleep(1000);
            string responseBytes = webClient.DownloadString(RequestURI);
            //Logging.Log.WriteLine("Reposnse : " + responseBytes);

            CookiesAPI.Cookies = webClient.ResponseHeaders["Set-Cookie"];
            Thread.Sleep(3000);
            string jsonText = string.Empty;
            if (responseBytes.Contains("xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(responseBytes);
                jsonText = JsonConvert.SerializeXmlNode(doc);
            }
            else jsonText = responseBytes;

            webClient.Dispose();
            Response = JObject.Parse(jsonText);
            if ((Response["RESULT"]["CODE"].ToString().Equals("0")))
                Log.WriteMessage("Response from server is passed with code = 0");
            else
            {
                Log.WriteMessage("Request : " + RequestURI);
                Log.WriteMessage("Response : " + responseBytes);
            }
        }

        public void RunPOST_DepositRequest(DepositACHParameters parms)
        {
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Cookie] = CookiesAPI.Cookies;         
            webClient.Headers[HttpRequestHeader.Accept] = "application/json, text/plain, */*";
            webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1";
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en-US,en;q=0.8,fi;q=0.6";           
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });
            string resp = webClient.UploadString(RequestURI, "POST", "AccountEmail=" + "" + "&AccountName=" + parms.AccountName +"&AccountNo=" + parms.AccountNumber + "&PersonNationalID=" + parms.PersonalNationalID);
            CookiesAPI.Cookies = webClient.ResponseHeaders["Set-Cookie"];
            Thread.Sleep(3000);
            string jsonText = resp;
            webClient.Dispose();
            Response = JObject.Parse(jsonText);
            if ((Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0")))
                Log.WriteMessage("Response from server is passed with code = 0");
            else
            {
                Log.WriteMessage("Request : " + RequestURI);
                Log.WriteMessage("Response : " + Response);
            }
        }

        public void RunPOST_DepositCreditCardRequest(DepositCCParameters parms)
        {
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Cookie] = CookiesAPI.Cookies;
            webClient.Headers[HttpRequestHeader.Accept] = "application/json, text/plain, */*";
            webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1";
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en-US,en;q=0.8,fi;q=0.6";
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });
            string resp = webClient.UploadString(RequestURI, "POST", "AccountEmail=&Authentication=" + parms.CVV + "&Expiration=" + parms.Expiration + "&ExpirationMonth=" + parms.ExpirationMonth + "&ExpirationYear=" + parms.ExpirationYear + "&AccountName=" + parms.AccountName + "&AccountNo=" + parms.AccountNumber);
            CookiesAPI.Cookies = webClient.ResponseHeaders["Set-Cookie"];
            Thread.Sleep(3000);
            string jsonText = resp;
            webClient.Dispose();
            Response = JObject.Parse(jsonText);
            if ((Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0")))
                Log.WriteMessage("Response from server is passed with code = 0");
            else
            {
                Log.WriteMessage("Request : " + RequestURI);
                Log.WriteMessage("Response : " + Response);
            }
        }

        // send password of user to the body
        public void RunPost_LoginRequest(string pass, string username, string currencyCode)
        {
            WebClient webClient = new WebClient();
            webClient.Headers["User-Agent"] = HeaderUserAgent;
            webClient.Headers[HttpRequestHeader.Cookie] = CookiesAPI.Cookies;
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";           
            string resp = webClient.UploadString(RequestURI, "POST", "username="+username+"&password="+pass+"&ClientTimezone=180&UniqueVisitorID=9FBBAF966753BAC2D8BF82E833571755&CurrencyCode="+currencyCode);
            //Log.WriteMessage("Reposnse : " + resp);
            CookiesAPI.Cookies = webClient.ResponseHeaders["Set-Cookie"];
            webClient.Dispose();

            Response = JObject.Parse(resp);
            if ((Response["S2C"]["RESULT"]["CODE"].ToString().Equals("0")))
                Log.WriteMessage("Response to login API from server is passed with code = 0");
            else
            {
                Log.WriteMessage("Request : " + RequestURI);
                Log.WriteMessage("Response : " + resp);
            }
        }


 
        private string SetDefaultCookeisInHeaderByWebDriver(string domainCookies = ".michiganlottery.com")
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                try
                {
                    //Change SSL checks so that all checks pass
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };                        
                }
                catch (Exception ex)
                {
                    Log.WriteMessage(ex.ToString());
                }
            
        
            webClient.DownloadString(_siteUrl);
            CookiesAPI.Cookies = webClient.Headers[HttpRequestHeader.Cookie];
            DefaultCookiesWebDriver = webClient.Headers[HttpRequestHeader.Cookie];
            return DefaultCookiesWebDriver;
            
            //IList<OpenQA.Selenium.Cookie> cookies = _driver.Manage().Cookies.AllCookies;
            //string cookiesString = string.Empty;
            //foreach (var cookie in cookies)
            //{
            //    if (cookie.Domain.Contains("gamesrv") || cookie.Domain.Equals(domainCookies))
            //    //if (cookie.Domain.Contains("gamesrv1.qa.michiganlottery.com") )
            //    cookiesString = cookiesString + cookie.Name +"=" + cookie.Value + ";";
            //}
            
            //DefaultCookiesWebDriver = cookiesString;

            //return cookiesString;           
        }         

        public void RunGET_PreLoginRequest()
        {
            Log.WriteMessage("The Request of the Pre login was sent... ");
            RequestURI = "https://gamesrv1.qa.michiganlottery.com/ScratchCards/sapi.aspx?AFI=113&AR=&CSI=113&CurrencyCode=USD&IUA=neoc&LNG=ENU&MMI=0&PAR=&cm=PLI&rst=j";
            RunGET_Request();
        }
    }  
}

public static class CookiesAPI
{
    public static string Cookies { get; set; }

    public static void DeleteCookies()
    {
        Cookies = "";
        //driver.Manage().Cookies.DeleteAllCookies();

    }

}
