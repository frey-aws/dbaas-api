using System;
using System.Collections.Generic;

namespace OperationsApi.BusinessLogic
{
    public interface IAppPrincipal
    {
        int Id { get; }
        Guid Key { get; }

        ICollection<string> RoleList { get; }
    }
}
