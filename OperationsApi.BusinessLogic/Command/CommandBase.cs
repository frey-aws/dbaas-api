using System;
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
                commandResult.ReturnItem = item;            // auto-assumed HttpResponse: 200, Success: true                                
                Logger.Log(commandResult);
            }

            return commandResult;
        }

        protected void ExceptionResult(Exception ex)
        {
            commandResult.Exception = ex;
            commandResult.Success = false;
            commandResult.HttpResponse = "500";             // TODO:  If required to be more granular than 500 - Internal Server Error, can adjust
            Logger.Error(ex);     
        }

        protected void InvalidResult(string message)
        {
            commandResult.Success = false;
            commandResult.Valid = false;
            commandResult.HttpResponse = "500";             // TODO:  If required to be more granular than 500 - Internal Server Error, can adjust            
            commandResult.PrimaryMessage += message;
            Logger.Log(commandResult);
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
