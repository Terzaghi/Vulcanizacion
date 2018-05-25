using Communication.SignalR;
using LoggerManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WCF_RuleMotorServer.Interfaz;
using RequestManager;
using Model.BL.DTO.Enums;

namespace WCF_RequestMotorServer
{
    public class RequestServer : IRequestMotorWCF
    {
        ILogger log =LogFactory.GetLogger(typeof(RequestServer));

        private RequestMotor _motorSolicitudes = null;
      
        private SignalRManager _signalR = null;

        public RequestServer(ref RequestMotor motorSolicitudes, ref SignalRManager signalR)
        {
            this._motorSolicitudes = motorSolicitudes;
           
            this._signalR = signalR;
        }

        #region Infomación del servidor
        public bool IsActive()
        {
            bool sw = false;
            try
            {
                sw = this._motorSolicitudes.IsActive();
            }
            catch (Exception er)
            {
                log.Error("IsActive()", er);
            }
            return sw;

        }
        #endregion

        #region Solicitudes  
        public string ListActiveRequest(List<int> Ids_Requests)
        {
            string result = string.Empty;

            try
            {
                var lst = this._motorSolicitudes.ListActiveRequest(Ids_Requests);

                // Convertimos el resultado a json
                result = Newtonsoft.Json.JsonConvert.SerializeObject(lst, new Newtonsoft.Json.JsonSerializerSettings
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
                });
            }
            catch (Exception er)
            {
                log.Error("ListActiveRules()", er);
            }

            return result;
        }
        public string ListPendingRequestsWithState(int? Id_User, int? Id_Device, int numeroElementos)
        {
            string result = string.Empty;
            int numRegistros = -1;

            try
            {
                var lst = this._motorSolicitudes.ListPendingRequestsWithState(Id_User, Id_Device, numeroElementos);

                if (lst != null)
                    numRegistros = lst.Count;

                // Convertimos el resultado a json
                result = Newtonsoft.Json.JsonConvert.SerializeObject(lst, new Newtonsoft.Json.JsonSerializerSettings
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
                });
            }
            catch (Exception er)
            {
                log.Error("ListPendingNotificationsWithState(). Registros rule: {0}", er, numRegistros);
            }

            return result;
        }
        public void MarkAllAs_Async(long[] Ids_RequestGenerateds, int state, int? id_User, int? id_Device)
        {
            try
            {
                this._motorSolicitudes.MarkAllAs_Async(Ids_RequestGenerateds, (Estado_Solicitud)state, id_User, id_Device);
            }
            catch (Exception er)
            {
                log.Error("MarkAllAs_Async()", er);
            }
        }
        #endregion


        #region Tags 
        public object ReadValue(string tag)
        {
            object result = null;

            try
            {
                result = this._motorSolicitudes.ReadValue(tag);
            }
            catch (Exception er)
            {
                log.Error("ReadValue()", er);
            }

            return result;
        }
        #endregion
    }
}
