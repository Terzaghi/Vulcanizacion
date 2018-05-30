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

        public void AddPrensa(int id_Prensa)
        {
            try
            {
                this._motorSolicitudes.AddPrensa(id_Prensa);
            }catch(Exception ex)
            {
                log.Error("AddPrensa()", ex);
            }
        }

        public bool IsBarcodeValid(string barcode, int id_prensa)
        {
            bool sw = false;
            try
            {
               sw= this._motorSolicitudes.isBarcodeValid(barcode, id_prensa);
            }catch(Exception ex)
            {
                log.Error("IsBarcodeValid()", ex);
            }
            return sw;
        }

        public void MarkAs_Async(long id_Request, Estado_Solicitud state, int? id_User, int? id_Device)
        {
            try
            {
                this._motorSolicitudes.MarkAs_Async(id_Request, state, id_User, id_Device);

            }catch(Exception ex)
            {
                log.Error("MarkAs_Async", ex);
            }
        }

        public void ModifyPrensa()
        {
            try
            {
                this._motorSolicitudes.ModifyPrensa();
            }catch(Exception ex)
            {
                log.Error("ModifyPrensa", ex);
            }
        }

        public void RemovePrensa(int id_prensa)
        {
            try
            {
                this._motorSolicitudes.RemovePrensa(id_prensa);
            }
            catch (Exception ex)
            {
                log.Error("RemovePrensa", ex);
            }
        }

        public Tipo_Contramedidas getContramedidas(int id_prensa)
        {
            throw new NotImplementedException();
        }
    }
}
