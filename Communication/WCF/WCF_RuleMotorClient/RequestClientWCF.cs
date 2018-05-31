using LoggerManager;
using Model.BL.DTO.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;

namespace WCF_RequestMotorClient
{
    public class RequestClientWCF
    {
        ILogger log = LogFactory.GetLogger(typeof(RequestClientWCF));

        #region Funcionalidades de cliente
        private IRequestMotorWCF _client = null;

        public bool Conectado { get; private set; }

        public string Host { get; set; }
        public int Port { get; set; }


        public RequestClientWCF()
        {
            // Configuración por defecto
            this.Host = "127.0.0.1";
            this.Port = 44002;

            // Cargamos la configuración de web.config
            if (ConfigurationManager.AppSettings["WCF_ServerHost"] != null) this.Host = ConfigurationManager.AppSettings["WCF_ServerHost"];
            if (ConfigurationManager.AppSettings["WCF_ServerPort"] != null)
            {
                int puerto;
                if (int.TryParse(ConfigurationManager.AppSettings["WCF_ServerPort"], out puerto))
                    this.Port = puerto;
            }

            Conectar();
        }

        // Otro contructor. Se utiliza para cuando se conecta desde el client manager, pasando el puerto sin 
        // tener archivo con configuración
        public RequestClientWCF(int port) : base()
        {
            this.Host = "127.0.0.1";
            this.Port = port;

            Conectar();
        }

        private bool Conectar()
        {
            bool sw = false;

            try
            {
                string strConexion = string.Format("net.tcp://{0}:{1}/RequestManager", this.Host, this.Port);

                log.Debug("Conectar. Conectando al WCF ({0})...", strConexion);

                EndpointAddress endPoint = new EndpointAddress(strConexion);
                NetTcpBinding _elbinding = new NetTcpBinding();
                _elbinding.Security.Mode = SecurityMode.None;


                _elbinding.CloseTimeout = new TimeSpan(0, 10, 0);
                _elbinding.SendTimeout = new TimeSpan(0, 10, 0);
                _elbinding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                //_elbinding.OpenTimeout = new TimeSpan(0, 10, 0);
                _elbinding.OpenTimeout = new TimeSpan(0, 10, 0);
                _elbinding.MaxConnections = 20000;

                _elbinding.MaxBufferPoolSize = int.MaxValue;

                _elbinding.MaxBufferSize = int.MaxValue;
                _elbinding.MaxReceivedMessageSize = int.MaxValue;


                _elbinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;
                _elbinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;


                ChannelFactory<IRequestMotorWCF> factory = new ChannelFactory<IRequestMotorWCF>(_elbinding, endPoint);
                factory.Closed += Factory_Closed;
                factory.Faulted += Factory_Faulted;
                factory.Opened += Factory_Opened;
                _client = factory.CreateChannel();

                sw = true;

                if (factory.State == CommunicationState.Opened)
                    this.Conectado = sw;
            }
            catch (Exception er)
            {
                log.Error("Conectar()", er);
            }

            //this.Conectado = sw;

            if (sw)
                log.Debug("Conectado");
            else
                log.Debug("No se pudo conectar");

            return sw;
        }

        private void Factory_Opened(object sender, EventArgs e)
        {
            this.Conectado = true;
        }

        private void Factory_Faulted(object sender, EventArgs e)
        {
            this.Conectado = false;
        }

        private void Factory_Closed(object sender, EventArgs e)
        {
            this.Conectado = false;
        }

        #endregion

        public void MarkAs_Async(long Ids_Request, Estado_Solicitud state, int? id_User, int? id_Device)
        {
            try
            {
                if (this.Conectado)
                {
                    this._client.MarkAs_Async(Ids_Request, (int)state, id_User, id_Device);
                }
            }
            catch (Exception er)
            {
                log.Error("MarkAs_Async()", er);
            }
        }

        public void AddPrensa(int id_Prensa)
        {
            try
            {
                if (this.Conectado)
                {
                    this._client.AddPrensa(id_Prensa);
                }
            }
            catch (Exception er)
            {
                log.Error("AddPrensa()", er);
            }
        }
        public void ModifyPrensa()
        {
            try
            {
                if (this.Conectado)
                {
                    this._client.ModifyPrensa();
                }
            }
            catch (Exception er)
            {
                log.Error("ModifyPrensa()", er);
            }
        }
        public void RemovePrensa(int id_prensa)
        {
            try
            {
                if (this.Conectado)
                {
                    this._client.RemovePrensa(id_prensa);
                }
            }
            catch (Exception er)
            {
                log.Error("RemovePrensa()", er);
            }
        }
        public bool IsBarcodeValid(string barcode, int id_prensa)
        {
            try
            {
                if (this.Conectado)
                {
                    return true;
                }
            }
            catch (Exception er)
            {
                log.Error("RemovePrensa()", er);
            }
            return false;
        }
        Tipo_Contramedidas getContramedidas(int id_prensa)
        {
            try
            {
                if (this.Conectado)
                {
                    return Tipo_Contramedidas.Pinchar;
                }
            }
            catch (Exception er)
            {
                log.Error("getContramedidas()", er);
            }
            return Tipo_Contramedidas.Pinchar;
        }






    }
}
