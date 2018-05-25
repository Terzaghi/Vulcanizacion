using LoggerManager;
using Model.BL.DTO.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WCF_RequestMotorClient;
using WebApi.Server.Interfaces;

namespace WebApi.Server.Controllers
{
    public class RequestMotorController : ApiController, IRequestMotor
    {
        ILogger log = LogFactory.GetLogger(typeof(RequestMotorController));
        
        [HttpGet]
        [ResponseType(typeof(bool))]
        [Route("api/RequestMotor/GetTest/{message}")]
        public bool GetTest(string message)
        {
            log.Information(string.Format("Acceso a GetTest({0})", message != null ? message : string.Empty));
            //http://localhost:54672/api/RuleMotor/GetTest/HolaMundo
            return true;
        }

     
    }
}
