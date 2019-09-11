using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.IO;
using System.Configuration;

namespace NG.Automation.Core.Logging
{
    public class Log
    {        
        private static readonly ILog logger = LogManager.GetLogger(typeof(Log)); 
                   
        static Log()
        {
            XmlConfigurator.Configure();         
        }

        public static void WriteMessage(string msg, bool innerLogOnly=false)
        {                      
            string compname = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
            if(innerLogOnly)
                logger.Info(msg);
            else
            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString() + compname + " - " + " LOG: " + msg);
        }

        public static void WriteMessage(string msg, bool innerLogOnly=false, params object[] args)
        {
            string compname = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
            if(innerLogOnly)
             logger.Info(string.Format(msg, args));

            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString() + compname + " - " + " LOG: " + msg);

        }

        public static void WriteError(string msg, Exception ex, bool innerLogOnly=true)
        {
            string compname = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
            if(innerLogOnly)
                logger.Error(msg, ex);
            else
            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString() + compname + " - " + " LOG: " + msg + " Error:" + ex.ToString());

        }

    // ToDo:

    }
}
