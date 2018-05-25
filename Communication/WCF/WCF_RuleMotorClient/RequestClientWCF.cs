using LoggerManager;
using Model.BL.DTO.Enums;
using Newtonsoft.Json;
using RuleManager.DTO;
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
        public RequestClientWCF(int port): base()
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

        public void MarkAllAs_Async(long[] Ids_RequestsGenerateds, Estado_Solicitud state, int? id_User, int? id_Device)
        {
            try
            {
                if (this.Conectado)
                {
                    this._client.MarkAllAs_Async(Ids_RequestsGenerateds, (int)state, id_User, id_Device);
                }
            }
            catch (Exception er)
            {
                log.Error("MarkAllAs_Async()", er);
            }
        }

        /// <summary>
        /// Devuelve un listado de notificaciones generadas con detalles que indican si la regla está activa o no, filtradas por el destinatario
        /// </summary>
        /// <param name="Id_User"></param>
        /// <param name="Id_Device"></param>
        /// <returns></returns>
        public List<RequestWithStates> ListPendingRequestsWithState(int? Id_User, int? Id_Device,int numeroElementos)
        {
            List<RequestWithStates> r = null;

            try
            {
                if (this.Conectado)
                {
                    string json = this._client.ListPendingRequestsWithState(Id_User, Id_Device,numeroElementos);

                    r = JsonConvert.DeserializeObject<List<RequestWithStates>>(json, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });

                }
            }
            catch (Exception er)
            {
                log.Error("ListPendingRequestsWithState()", er);
            }

            return r;
        }





    }
}
