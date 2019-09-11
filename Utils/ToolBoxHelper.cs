
//using AutoItX3Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Automation.Core.Utils
{
    public class ToolBoxHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="popUpTitle">Set popUp title </param>
        /// <param name="picPath">set pictures path</param>
       public static void UploadWindowsAutoIt(string popUpTitle="Choose File to Upload",string picPath= @"C:\Users\Teimurazs\Downloads\cap1.Png" )
         {
             //AutoItX3 autoIt = new AutoItX3();

             //autoIt.WinActivate("Choose File to Upload");

             //autoIt.Send(picPath);
             //Thread.Sleep(10000);

             //autoIt.Send("{ENTER}");
             //Thread.Sleep(6000);

         }
    
        public static int GetRequestStatus(string requestUrl)
        {
            string regUrl=requestUrl;
            HttpWebResponse webResponse;
            int wRespStatusCode;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest
                                           .Create(regUrl);
            webRequest.AllowAutoRedirect = true;
    

            try
            {
                webResponse = (HttpWebResponse)webRequest.GetResponse();
                wRespStatusCode = (int)webResponse.StatusCode;
            }
            catch (WebException we)
            {
                wRespStatusCode =(int)((HttpWebResponse)we.Response).StatusCode;
            }
            return wRespStatusCode;
        }

        public static Dictionary<string, string> DesirializationJson(string jsonString)
        {
           // string json = "{\"id\":\"13\", \"value\": true}";

            var serializer = new JavaScriptSerializer(); //using System.Web.Script.Serialization;

            Dictionary<string, string> values = serializer.Deserialize<Dictionary<string, string>>(jsonString);

            return values;
        }

       public static string GetUrlParametersValue(string pageUrl,string parameter)
        {
            var Uri = new Uri(pageUrl);
           
           string paramValue =   HttpUtility.ParseQueryString(Uri.Query).Get(parameter);

           return paramValue;
        
        
          //  HttpUtility.ParseQueryString(queryString);
        }

        public string[] GetParametersFromURL(string myUrl)
        {
            string[] myList = new string[] { };

            int index = myUrl.IndexOf('?') + 1;
            string myNewURL = myUrl.Substring(index);

            myList = myNewURL.Split('&');
            
            return myList;
        }



        public static List<DateTime> ConvertList(List<string> elStr)
        {
            List<DateTime> dateNTimeList = new List<DateTime>();
            foreach (var item in elStr)
            {
                try
                {
                    DateTime dt = Convert.ToDateTime(item);
                    dateNTimeList.Add(dt);
                }
                catch (Exception)
                {

                }

            }
            return dateNTimeList;
        }

        public static List<DateTime> SortDescending(List<DateTime> list)
        {
            list.Sort((a, b) => b.CompareTo(a));
            return list;
        }

        public static string GenerateRandomNumber()
        {
            // User Name is random and based on current minutes and second
            Random number = new Random();
            int randomNum = number.Next(1, 9999999);
            //return "qa" + randomNum.ToString() + DateTime.Now.ToString("ddMMss");
            return randomNum.ToString();
        }


        /// <summary>
        /// Function return the brand name from URL 
        /// </summary>
        /// <param name="myUrl"></param>
        /// <returns></returns>
        public string GetMyBrandFromUrl(string myUrl)
        {
            string[] myList = new string[] { };
            // get hopa.com or karamba.com
            int index = myUrl.IndexOf('.') + 1;
            string myNewURL = myUrl.Substring(index);

            myList = myNewURL.Split('.');
            // the first parms is the brand
            return myList[0].ToString();
        }

        public static bool CompareLists(IList<string> first, IList<string> second)
        {
            if (first.Count != second.Count)
                Assert.Fail("first " + first.Count + "--second " + second.Count);
            int counter = 0;
            int numberOfElements = second.Count;
            foreach (string element1 in first)
                if (second.Contains(element1.Trim()))
                {
                    counter++;
                }
            if (counter == numberOfElements)
                return true;
            else return false;
        }


        public static string[] GetParameter_XMLFile(string filename, string descendant)
        {

           
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlNodeList nodes = doc.SelectNodes("/TestCases/" + descendant + "/str");

            string[] myresults = new string[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                myresults[i] = nodes[i].InnerText;
            }
           

            return myresults;         
        }


    }
}
