using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MyTrelloLearn.Web.Models
{
    public static class AuthHelper
    {
        private static string _ApplicationKey = null;
        private static string _ApplicationSecret = null;

        public static string ApplicationKey 
        { 
            get 
            {
                if (_ApplicationKey == null)
                {
                    _ApplicationKey = ConfigurationManager.AppSettings["ApplicationKey"];
                }
                return _ApplicationKey;
            } 
        }
        public static string ApplicationSecret
        {
            get
            {
                if (_ApplicationSecret == null)
                {
                    _ApplicationSecret = ConfigurationManager.AppSettings["ApplicationSecret"];
                }
                return _ApplicationSecret;
            }
        }

    }
}