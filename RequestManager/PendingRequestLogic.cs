using System;
using System.Runtime.Serialization;
using LoggerManager;
using Model.BL.DTO.Enums;


namespace RequestManager
{
    /// <summary>
    /// Gestiona y valida que si la solicitud ha de eliminarse de la memoria de notificaciones pendientes
    /// </summary>
    [DataContract]
    public class PendingRequestLogic
    {
        ILogger log = LogFactory.GetLogger(typeof(PendingRequestLogic));

        private int Id_Prensa;
        private long Id_Request;
        private Estado_Solicitud Request_State;
        private int? AssociatedUser;
        private int? AssociatedDevice;
        private DateTime Request_DateGeneration;
        private double ttl;

        public int GetIdPrensa
        {
            get { return this.Id_Prensa; }
        }
        public long GetIdRequest
        {
            get { return this.Id_Request; }

        }
        public Estado_Solicitud GetRequestState
        {
            get { return this.Request_State; }
        }
        public int? GetAssociatedUser
        {
            get { return this.AssociatedUser; }
        }
        public int? GetAssociatedDevice
        {
            get { return this.AssociatedDevice; }
        }
        public DateTime GetDateGeneration
        {
            get { return this.Request_DateGeneration; }
        }
        public double GetTTL
        {
            get { return this.ttl; }
        }
        public void SetConfiguration(Estado_Solicitud state, int? Id_User, int? Id_Device)
        {
            this.Request_State = state;
            this.AssociatedUser = Id_User;
            this.AssociatedDevice = Id_Device;
        }
        
    }
}
