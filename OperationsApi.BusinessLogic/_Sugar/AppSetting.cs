using System;
using System.Configuration;

namespace OperationsApi.BusinessLogic
{
    public class AppSetting
    {
        private static AppSettingsReader _reader;
        private static AppSettingsReader reader
        {
            get
            {
                if(null == _reader)
                {
                    _reader = new AppSettingsReader();
                }

                return _reader;
            }
        }
       
        // public static string CONNECTION_DEFAULT = "conn.default";

        // public static string ACTIVE_PUBLIC_KEY = Items.GetConfig<string>("active.public", string.Empty);
        // public static string ACTIVE_PRIVATE_KEY = Items.GetConfig<string>("active.private", string.Empty);

        public static string AWS_DEFAULT_REGION = Items.GetConfig<string>("aws.default.region", string.Empty);
        public static string AWS_DEFAULT_VPC = Items.GetConfig<string>("root-vpc", string.Empty);

        public static string AWS_LOG_GROUP_NAME = Items.GetConfig<string>("aws.log.groupname", string.Empty);        

        public class Items
        {
            public static T GetConfig<T>(string key, dynamic _default)
            {
                var result = reader.GetValue(key, typeof(T));

                if(null == result)
                {
                    result = _default;
                }

                return (T)result;                
            }
        }
    }
}
