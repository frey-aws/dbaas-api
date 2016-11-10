using System;
using System.Security.Principal;
using System.Threading;

namespace OperationsApi.BusinessLogic
{
    public abstract class BusinessBase
    {
        protected AccountPrincipal CurrentPrincipal
        {
            get
            {
                AccountPrincipal principal = (AccountPrincipal)Thread.CurrentPrincipal;
                return principal;
            }
        }
    }
}
