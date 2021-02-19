using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SRWBPostpaidPayments.Payment
{
    public class WebConfigValues
    {
        public static string API = "SRWBPostpaid API";
        public static string AppName()
        {
            string appName = ConfigurationManager.AppSettings["AppName"];

            return appName;
        }
        public static string Password()
        {
            string password;
            try
            {
                password = ConfigurationManager.AppSettings["Password"];
            }
            catch (Exception)
            {
                password = ConfigurationManager.AppSettings["Password"];
            }

            return password;
        }

        public static string Username()
        {
            string name;
            try
            {
                name = ConfigurationManager.AppSettings["Username"];
            }
            catch (Exception)
            {
                name = ConfigurationManager.AppSettings["Username"];
            }

            return name;
        }

        public static string ProfileID()
        {
            string profileId = ConfigurationManager.AppSettings["ProfileID"];

            return profileId;
        }

        public static int T24Port()
        {
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["T24Port"]);

            return port;
        }

        public static string FtName()
        {
            string ft = ConfigurationManager.AppSettings["FtName"];

            return ft;
        }

        internal static string GetSetting(string key)
        {
            string value = ConfigurationManager.AppSettings[key];

            return value;
        }

        public static string FtPrefix()
        {
            string prefix = ConfigurationManager.AppSettings["FtPrefix"];

            return prefix;
        }
        public static string PaymentDetails()
        {
            string paymentDetails = ConfigurationManager.AppSettings["PaymentDetails"];

            return paymentDetails;
        }

        public static string Key()
        {
            string key = ConfigurationManager.AppSettings["Key"];

            return key;

        }
    }
}