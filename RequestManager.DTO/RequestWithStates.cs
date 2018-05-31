using Model.BL.DTO.Enums;

namespace RequestManager.DTO
{
    public class RequestWithStates
    {
        public RequestGenerated Request { get; set; }
        public bool Active { get; set; }
        public Estado_Solicitud State { get; set; }
    }
}
