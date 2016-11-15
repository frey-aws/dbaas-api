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
        
        // Log4Net configuration settings               
        public static string LOG4NET_LOGGER = Items.GetConfig<string>("log4net.logger", string.Empty);

        // AWS configuration settings
        public static string AWS_DEFAULT_REGION = Items.GetConfig<string>("AWSRegion", string.Empty);
        public static string AWS_LOG_GROUP_NAME = Items.GetConfig<string>("aws.log.group.name", string.Empty);
        public static string AWS_LOG_STREAM_ACTION = Items.GetConfig<string>("aws.log.stream.action", string.Empty);
        public static string AWS_LOG_STREAM_EXCEPTION = Items.GetConfig<string>("aws.log.stream.exception", string.Empty);

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
