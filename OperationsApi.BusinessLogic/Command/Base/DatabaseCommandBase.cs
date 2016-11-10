using System;

using Amazon.RDS;
using Amazon.RDS.Model;

namespace OperationsApi.BusinessLogic.Command
{
    public partial class DatabaseCommmand : CommandBase
    {

        // TODO:  Determine if we're going to use virtual/override pattern or interface implementation pattern ... challenge is to determine how we figure out which platform is being called
        /// <summary>
        /// CreateDatabaseInstance:  Amazon implementation for creation of a database
        /// </summary>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
    }
}
