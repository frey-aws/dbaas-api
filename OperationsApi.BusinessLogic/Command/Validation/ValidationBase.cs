using System;
using System.Threading;

namespace OperationsApi.BusinessLogic.Validation
{
    public abstract class ValidationBase
    {
        protected ValidRequest validRequest;
        protected ValidRequest ValidRequest
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
