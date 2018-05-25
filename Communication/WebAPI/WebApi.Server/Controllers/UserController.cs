using LoggerManager;
using Model.BL;
using Model.BL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Server.Controllers
{
    public class UserController : ApiController
    {
        ILogger log = LogFactory.GetLogger(typeof(UserController).ToString());

        [HttpPost]
        [Route("api/User/GetUserByIdentityCode")]
        public HttpResponseMessage GetUserByIdentityCode([FromBody] string identityCode)
        {
            try
            {
                log.Information("Acceso a GetUserByIdentityCode()");

                Usuarios model = new Usuarios();
                var user = model.GetUserByIdentityCode(identityCode);

                return Request.CreateResponse<Usuario>(HttpStatusCode.Created, user);
            }
            catch (Exception er)
            {
                log.Error("GetUserByIdentityCode()", er);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, "Error message");
        }
    }
}
