﻿using System;
using System.Configuration;

namespace OperationsApi.BusinessLogic
{
    public class AppSetting
    {
        // public static string CONNECTION_DEFAULT = "conn.default";

        // public static string ACTIVE_PUBLIC_KEY = Items.GetConfig<string>("active.public", string.Empty);
        // public static string ACTIVE_PRIVATE_KEY = Items.GetConfig<string>("active.private", string.Empty);

        public static string AWS_DEFAULT_REGION = Items.GetConfig<string>("aws.default.region", string.Empty);
        public static string AWS_DEFAULT_VPC = Items.GetConfig<string>("root-vpc", string.Empty);

        public class Items
        {
            public static T GetConfig<T>(string key, dynamic _default)
            {
                return (T)Convert.ChangeType(((null != System.Configuration.ConfigurationManager.AppSettings[key]) ? System.Configuration.ConfigurationManager.AppSettings[key] :
                                                                                                  _default), typeof(T));
            }
        }
    }
}