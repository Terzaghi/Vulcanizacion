using Model.BL.DTO.Enums;
using RequestManager.DTO;
using System.Collections.Generic;

namespace RuleManager.DTO
{
    public class RequestWithStates
    {
        public RequestGenerated Request { get; set; }
        public bool Active { get; set; }
        public Estado_Solicitud State { get; set; }
    }

   
}
