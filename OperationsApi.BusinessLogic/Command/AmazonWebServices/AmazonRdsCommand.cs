using System;

using Amazon.RDS;
using Amazon.RDS.Model;

namespace OperationsApi.BusinessLogic.Command
{
    public partial class AmazonRdsCommand : CommandBase
    {
        private AmazonRDSClient _rdsClient;
        private AmazonRDSClient rdsClient
        {
            get
            {
                if(null == _rdsClient)
                {
                    _rdsClient = new AmazonRDSClient(Amazon.RegionEndpoint.USEast1);
                }

                return _rdsClient;
            }
        }

        // TODO:  Determine if we're going to use virtual/override pattern or interface implementation pattern ... challenge is to determine how we figure out which platform is being called
        /// <summary>
        /// CreateDatabaseInstance:  Amazon implementation for modification of an RDS instance
        /// </summary>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        public ICommandResult CreateDatabaseInstance(ApiRequest apiRequest)
        {
            CreateDBInstanceRequest request = SerializeHelper.GetObject<CreateDBInstanceRequest>(apiRequest.Context.ToString());

            try
            {
                var result = rdsClient.CreateDBInstance(request);

                // TODO:  Add in a repo call here ...

                AssignReturnData(result);

            }
            catch(Exception ex)
            {
                // TODO:  Add in exception handling and make more elegant, but we'll at least return the error in the response
                InvalidResult(ex.Message + "\r\n " + ex.StackTrace);
            }                            

            return commandResult;
        }
        
        /// <summary>
        /// ModifyDatabaseInsance:  Amazon implementation for modification of an RDS instance
        /// </summary>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        public ICommandResult ModifyDatabaseInstance(ApiRequest apiRequest)
        {
            ModifyDBInstanceRequest request = SerializeHelper.GetObject<ModifyDBInstanceRequest>(apiRequest.Context.ToString());

            try
            {
                var result = rdsClient.ModifyDBInstance(request);

                // TODO:  Add in a repo call here ...

                AssignReturnData(result);

            }
            catch (Exception ex)
            {
                // TODO:  Add in exception handling and make more elegant, but we'll at least return the error in the response
                InvalidResult(ex.Message + "\r\n " + ex.StackTrace);
            }

            return commandResult;
        }

    }
}
