using System;
using System.Threading;

namespace OperationsApi.BusinessLogic.Validation
{
    public abstract class ValidationBase
    {
        protected IValidRequest validRequest;
        protected IValidRequest ValidRequest
        {
            get
            {
                if (null == validRequest)
                {
                    validRequest = new ValidRequest();
                }

                return validRequest;
            }
        }        
    }
}
