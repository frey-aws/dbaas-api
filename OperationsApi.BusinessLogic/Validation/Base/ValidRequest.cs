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

        private ICollection<string> errorList = new List<string>();
        public ICollection<string> ErrorList
        {
            get
            {
                return errorList;
            }
        }

        public void AddError(string errorMessage)
        {
            ErrorList.Add(errorMessage);
            isValid = false;
        } 
    }
}
