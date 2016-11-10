using System;
using System.Security.Principal;

namespace OperationsApi.BusinessLogic
{
    public class AccountPrincipal : GenericPrincipal
    {
        public AccountPrincipal(IAppPrincipal account, IIdentity identity, string[] roleList)
            : base(identity, roleList)
        {
            this._account = account;
            // Initialize();
        }

        private IAppPrincipal _account;
        public IAppPrincipal Account
        {
            get
            {
                return this._account;
            }
            internal set
            {
                this._account = value;
            }
        }        
    }
}
