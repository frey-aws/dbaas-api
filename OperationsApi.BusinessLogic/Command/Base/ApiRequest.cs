using System;
using System.Security.Principal;
using System.Threading;

namespace OperationsApi.BusinessLogic.Command
{
    public class ApiRequest
    {
        /// <summary>
        /// Context:  JSON structure of actual parameters to be passed onto underlying API
        /// </summary>
        public dynamic Context { get; set; }
    }
}
