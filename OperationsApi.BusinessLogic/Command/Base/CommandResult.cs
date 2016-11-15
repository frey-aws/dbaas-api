using System;

namespace OperationsApi.BusinessLogic.Command
{
    public sealed class CommandResult : ICommandResult
    {
        private bool valid = true;        
        internal bool Valid
        {
            get
            {
                return valid;
            }
            set
            {
                valid = value;
            }
        }

        private bool success = false;
        public bool Success
        {
            get
            {
                return success;
            }
            internal set
            {
                success = value;
            }
        }

        // "If you got this message, something has gone horribly wrong and the developer was too lazy to catch the exception (or simply didn't realize you were going to do what you did).  Check the command/query executed to review the issue.";
        private string primaryMessage = string.Empty; 
        public string PrimaryMessage
        {
            get
            {
                return primaryMessage;
            }
            internal set
            {
                primaryMessage = value;
            }
        }

        private object returnItem;
        public object ReturnItem
        {
            get
            {
                return returnItem;
            }
            internal set
            {
                returnItem = value;
            }
        }

        private int? returnKey;
        public int? ReturnKey
        {
            get
            {
                return returnKey;
            }
            internal set
            {
                returnKey = value;
            }
        }

        private Exception _exception;
        public Exception Exception
        {
            get
            {
                return _exception;
            }
            internal set
            {
                // TODO: Review this implementation for this code base ...
                _exception = value;
                primaryMessage = "Transaction Failed:" + _exception.InnerException.Message;
            }
        }
    }
}
