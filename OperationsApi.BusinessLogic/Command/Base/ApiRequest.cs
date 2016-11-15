using System;
using System.Security.Principal;
using System.Threading;

namespace OperationsApi.BusinessLogic.Command
{
    public class ApiRequest
    {
        /// <summary>
        /// WhoRequested:  Currently should be an email, but could also be a cell phone number as this will be leveraged by AWS SNS ... longer term this can be deprecated
        /// </summary>
        public string WhoRequested { get; set; }

        /// <summary>
        /// Context:  JSON structure of actual parameters to be passed onto underlying API
        /// </summary>
        public dynamic Context { get; set; }
    }
}
