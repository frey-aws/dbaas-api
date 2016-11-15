using System;
using System.Collections.Generic;

namespace OperationsApi.BusinessLogic.Validation
{
    public interface IValidRequest
    {
        bool IsValid { get; }
        ICollection<string> ErrorList { get; }        
    }
}
