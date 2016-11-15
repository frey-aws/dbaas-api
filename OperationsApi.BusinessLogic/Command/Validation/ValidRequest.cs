using System;
using System.Collections.Generic;

namespace OperationsApi.BusinessLogic.Validation
{
    public class ValidRequest : IValidRequest
    {
        private bool isValid = true;        
        public bool IsValid
        {
            get
            {
                return isValid;
            }
            set
            {
                isValid = value;
            }
        }

        private ICollection<string> errorList;
        public ICollection<string> ErrorList
        {
            get
            {
                if(null == errorList)
                {
                    errorList = new List<string>();
                }

                return errorList;
            }           
        }        
    }
}
