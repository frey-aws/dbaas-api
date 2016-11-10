using System;

namespace OperationsApi
{
    public partial class LocalAppSetting
    {
        public static string API_INIT_HEADER = Items.GetConfig<string>("api.init.key", "ps.cheyenne");
        public static string API_AUTH_HEADER = Items.GetConfig<string>("api.auth.key", "ps.maunsell");

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
