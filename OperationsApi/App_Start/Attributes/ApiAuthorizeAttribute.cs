using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;

using OperationsApi.BusinessLogic;

namespace OperationsApi
{
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //var request = actionContext.Request;

            //if (!actionContext.Request.IsLocal() || request.Headers.Any(p => p.Key == LocalAppSetting.API_AUTH_HEADER))
            //{
            //    if (!request.Headers.Any(p => p.Key == LocalAppSetting.API_AUTH_HEADER))
            //    {
            //        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "You are not authorized to access this API.");
            //        return;
            //    }

            //    var token = request.Headers.GetValues(LocalAppSetting.API_AUTH_HEADER).FirstOrDefault();
            //    var decryptedToken = EncryptionHelper.Decrypt(token, true);

            //    /// Slight difference to this app as every request will need to be validated, every single request as most requests will be coming from a mobile
            //    /// device.  Revisit this logic after setting up backend security approach

            //    //if (!ApplicationState.Current.ValidUsers.Any(p => p.Token == decryptedToken))
            //    //{
            //    //    var result = new ValidateMobile(decryptedToken).Execute();

            //    //    if (result.Success)
            //    //    {
            //    //        ApplicationState.Current.ValidUsers.Add(new Base.UserCredential()
            //    //        {
            //    //            Token = decryptedToken,
            //    //            UserPrincipal = (UserPrincipal)result.ReturnItem,
            //    //            Expiration = DateTime.Now.AddHours(8)
            //    //        });
            //    //    }
            //    //}

            //    //var principal = ApplicationState.Current.ValidUsers.Where(p => p.Token == decryptedToken).FirstOrDefault();

            //    //if (null == principal)
            //    //{
            //    //    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "You are not authorized to access this API.");
            //    //    return;
            //    //}

            //    //if (this.Roles.Count() > 0)
            //    //{
            //    //    bool isAuthorized = false;

            //    //    foreach (string role in this.Roles.Split(','))
            //    //    {
            //    //        if (principal.UserPrincipal.User.RoleList.Any(p => p.RoleName == role))
            //    //        {
            //    //            isAuthorized = true;
            //    //            break;
            //    //        }
            //    //    }

            //    //    if (!isAuthorized)
            //    //    {
            //    //        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "You are not authorized to access this API.");
            //    //        return;
            //    //    }
            //    //}

            //    //UserCredential.CurrentPrincipal = principal.UserPrincipal;
            //    //Thread.CurrentPrincipal = principal.UserPrincipal;
            //}
            //else
            //{
            //    //Thread.CurrentPrincipal = (UserPrincipal)new LogUserIn("admin@efrdata.com", "password").Execute().ReturnItem;
            //}

            base.OnAuthorization(actionContext);
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return true;
        }
    }

    public class UserCredential
    {
        public static AccountPrincipal CurrentPrincipal { get; set; }
    }

    public static class HttpRequestMessageExtensions
    {
        public static bool IsLocal(this HttpRequestMessage request)
        {
            var localFlag = request.Properties["MS_IsLocal"] as Lazy<bool>;
            return localFlag != null && localFlag.Value;
        }
    }
}
