using System.Web.Http;

using OperationsApi.BusinessLogic.Command;

namespace OperationsApi.Controllers
{
    // [Authorize]    
    [RoutePrefix("api/dbaas/aws")]
    public class AwsDatabaseController : ApiController
    {
        [HttpPost, Route("create-rds-instance")]
        public ICommandResult CreateRdsInstance(ApiRequest request)
        {
            return new AmazonRdsCommand().CreateDatabaseInstance(request);
        }

        [HttpPost, Route("modify-rds-instance")]
        public ICommandResult ModifyRdsInstance(ApiRequest request)
        {
            return new AmazonRdsCommand().ModifyDatabaseInstance(request);
        }

        [HttpGet, Route("sns-action-request")]
        public ICommandResult SnsActionRequest(ApiRequest request)
        {
            return new AmazonRdsCommand().ModifyDatabaseInstance(request);
        }
    }
}
