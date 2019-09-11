using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Core.Utils
{
    public static class DBUtil
    {
        public static string ExecuteScalar(string sqlquery, string connStr = "", string dbCatalog = "CentralSystem")
        {            
            if (string.IsNullOrWhiteSpace(sqlquery))
                throw new ArgumentNullException("sqlquery");


            if (string.IsNullOrWhiteSpace(connStr))
            {
                if (ConfigurationManager.ConnectionStrings[dbCatalog] == null)
                    throw new Exception(string.Format("Cannot find connection string ({0}) in configuration file", dbCatalog));
                connStr = ConfigurationManager.ConnectionStrings[dbCatalog].ConnectionString.ToString();
            }

            string results = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(sqlquery, conn);
                conn.Open();
                object temp = cmd.ExecuteScalar();                
                conn.Close();
                results = temp.ToString();
            }
            return results;
        }

        public static DataTable ExecuteQuery(string sqlquery, string connStr = "", string dbCatalog = "CentralSystem")
        {
            if (string.IsNullOrWhiteSpace(sqlquery))
                throw new ArgumentNullException("sqlquery");

            if (string.IsNullOrWhiteSpace(connStr))
            {
                if (ConfigurationManager.ConnectionStrings[dbCatalog] == null)
                    throw new Exception(string.Format("Cannot find connection string ({0}) in configuration file", dbCatalog));
                connStr = ConfigurationManager.ConnectionStrings[dbCatalog].ConnectionString.ToString();
            }

            DataTable tblResult = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, conn);
                adapter.Fill(tblResult);                             
            }
            return tblResult;
        }

        public static DataTable GetUserNamePassword(string TestCaseID)
        {
            DataTable tblResult = new DataTable();
            tblResult = ExecuteQuery("Select UserName,Password from Users where TestCaseID like '" + TestCaseID + "'",
                           "", "AutomationDB");
            Logging.Log.WriteMessage(string.Format("Test case using this player: Username: {0}, Password {1}",tblResult.Rows[0][0],tblResult.Rows[0][1]));
            return tblResult;
        }
    }
}
