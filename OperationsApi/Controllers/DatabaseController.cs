using System.Web.Http;

using OperationsApi.BusinessLogic.Command;

namespace OperationsApi.Controllers
{
    // [Authorize]    
    [RoutePrefix("api/dbaas")]
    public class DatabaseController : ApiController
    {
        [HttpPost, Route("aws/create-rds-instance")]
        public ICommandResult CreateDatabaseInstance(ApiRequest request)
        {
            return new AmazonRdsCommand().CreateDatabaseInstance(request);
        }
    }
}
