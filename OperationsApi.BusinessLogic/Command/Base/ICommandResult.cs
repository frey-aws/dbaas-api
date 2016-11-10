using System;

using Amazon.RDS;
using Amazon.RDS.Model;

namespace OperationsApi.BusinessLogic.Command
{
    public interface ICommandResult
    {
        bool Success { get; }
        string PrimaryMessage { get; }
        object ReturnItem { get; }
        int? ReturnKey { get; }
    }
}
