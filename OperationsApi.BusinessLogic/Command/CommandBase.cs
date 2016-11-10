using System;
using System.Security.Principal;
using System.Threading;

namespace OperationsApi.BusinessLogic.Command
{
    public abstract class CommandBase : BusinessBase, IDisposable
    {
        protected CommandResult _commandResult;
        protected CommandResult commandResult
        {
            get
            {
                if (null == _commandResult)
                {
                    _commandResult = new CommandResult();
                }

                return _commandResult;
            }
        }

        protected AccountPrincipal CurrentPrincipal
        {
            get
            {
                AccountPrincipal principal = (AccountPrincipal)Thread.CurrentPrincipal;
                return principal;
            }
        }

        protected ICommandResult AssignReturnData(dynamic item)
        {
            if (null != item)
            {
                if (item.GetType().GetProperty("Id") != null)
                {
                    commandResult.ReturnKey = item.Id;
                }

                commandResult.ReturnItem = item;
                commandResult.Success = true;
            }

            return commandResult;
        }

        protected void ExceptionResult(Exception ex)
        {
            commandResult.Exception = ex;
            commandResult.Success = false;

            //TODO: log error implementation            
        }

        protected void InvalidResult(string message)
        {
            commandResult.Success = false;
            commandResult.Valid = false;
            commandResult.PrimaryMessage += message;
            //TODO: log error implementation            
        }        

        #region IDisposable Members

        /// <summary>
        /// Returns <see langword="true"/> if this instance has been disposed of, <see langword="false"/> otherwise
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposes of the directory object
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            if (!IsDisposed)
            {
                try
                {
                    Dispose(true);
                }
                finally
                {
                    IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Disposes of the objects
        /// </summary>
        /// <param name="Disposing">True to dispose of all resources, false only disposes of native resources</param>
        protected virtual void Dispose(bool Disposing)
        {
            // TODO: Determine if anything requires disposal
        }

        #endregion

    }
}
