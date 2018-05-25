using Communication.SignalR;
using LoggerManager;
using RequestManager;
using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using WCF_RequestMotorServer.Clases;
using WCF_RuleMotorServer.Interfaz;

namespace WCF_RequestMotorServer
{
    public class RequestServerWCF : IDisposable
    {
        ILogger log = LogFactory.GetLogger(typeof(RequestServerWCF));

        private RequestMotor _motorSolicitudes = null;
        private SignalRManager _signalR = null;

        #region Establecimiento de la conexión
        private ServiceHost _host = null;        

        public bool Conectado { get; private set; }

        public RequestServerWCF(ref RequestMotor motorSolicitudes, SignalRManager instanceSignalR)
        {
            this.Conectado = false;

            this._motorSolicitudes = motorSolicitudes;
            this._signalR = instanceSignalR;
        }

        /// <summary>
        /// Activa el servicio WCF poniendolo a la escucha
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            bool sw = false;

            try
            {
                log.Debug("Start(). Cargando configuración");

                // Configuración predeterminada
                string host = "127.0.0.1";
                int port = 44002;

                // Cargamos la configuración de app.config
                if (ConfigurationManager.AppSettings["WCF_ListenIP"] != null) host = ConfigurationManager.AppSettings["WCF_ListenIP"];
                if (ConfigurationManager.AppSettings["WCF_ListenPort"] != null)
                {
                    log.Debug("Cargando configuración del puerto de WCF");
                    int puerto;
                    if (int.TryParse(ConfigurationManager.AppSettings["WCF_ListenPort"], out puerto))
                        port = puerto;
                }
                else
                    log.Warning("No se ha encontrado configuración del puerto WCF en el archivo de configuración. Utilizando configuración predeterminada: {0}", port);


                // Iniciamos el proceso
                log.Debug("Iniciar(). Configurando conexión WCF/TCP en {0}:{1}", host, port);

                Uri miurl = new Uri(
                    string.Format("net.tcp://{0}:{1}/RuleManager", host, port)
                    );                    

                // Preparamos los parámetros para el servicehost
                _host = new RequestManagerServiceHost(ref this._motorSolicitudes, ref this._signalR, typeof(RequestServer), miurl);
              
                // Creamos un binding con compresión en gzip de los datos a enviar
                var elbinding = new CustomBinding();
                var be1 = new BinaryMessageEncodingBindingElement();
                be1.CompressionFormat = CompressionFormat.GZip;
                elbinding.Elements.Add(be1);
                var be2 = new TcpTransportBindingElement();
                be2.MaxBufferSize = 100000000;
                be2.MaxReceivedMessageSize = 100000000;
                

                be2.MaxBufferPoolSize = 100000000;                
                be2.MaxPendingAccepts = 20000;
                be2.MaxPendingConnections = 20000;  // *********************** TODO. REVISAR                

                elbinding.Elements.Add(be2);
                elbinding = new CustomBinding(elbinding);
                                

                //----------------------------------------------
                // Esta sección se activa solo para generar el código del cliente
                // luego la podemos comentar.
                // svcutil.exe /language:cs /out:RuleManagerProxy.cs /config:app.config http://127.0.0.1:22229/RuleManager
                PublicarPorHTTP();
                // Fin del código para generación usando SVCUTIL.EXE
                //----------------------------------------------                

                _host.AddServiceEndpoint(typeof(IRequestMotorWCF), elbinding, "");

                _host.Open();

                sw = true;

                log.Debug("Servicio iniciado. A la escucha en {0}:{1}", host, port);
            }
            catch (Exception er)
            {
                log.Error("Iniciar()", er);
            }

            this.Conectado = sw;

            return sw;
        }

        private void PublicarPorHTTP()
        {
            try
            {
                //----------------------------------------------
                // Esta sección se activa solo para generar el código del cliente
                // luego la podemos comentar.
                // svcutil.exe /language:cs /out:RuleManagerProxy.cs /config:app.config http://127.0.0.1:22229/RequestManager
#if DEBUG
                System.ServiceModel.Description.ServiceMetadataBehavior smb = new System.ServiceModel.Description.ServiceMetadataBehavior();
                smb.HttpGetUrl = new Uri("http://127.0.0.1:22229/RequestManager");
                smb.HttpGetEnabled = true;
                _host.Description.Behaviors.Add(smb);
#endif
                // Fin del código para generación usando SVCUTIL.EXE
                //----------------------------------------------
            }
            catch (Exception er)
            {
                log.Error("PublicarPorHTTP()", er);
            }
        }

        /// <summary>
        /// Detiene el servicio WCF
        /// </summary>
        public void Stop()
        {
            try
            {
                log.Debug("Cerrando conexión WCF/TCP");

                if (_host != null)
                {
                    _host.Close();
                    _host = null;
                }

                this.Conectado = false;
            }
            catch (Exception er)
            {
                log.Error("Detener()", er);
            }
        }
        #endregion

        #region Interfaz IDisposeable
        // Indica si ya se llamo al método Dispose. (default = false)
        private Boolean disposed;

        /// <summary>
        /// Implementación de IDisposable. No se sobreescribe.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            // GC.SupressFinalize quita de la cola de finalización al objeto.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Limpia los recursos manejados y no manejados.
        /// </summary>
        /// <param name="disposing">
        /// Si es true, el método es llamado directamente o indirectamente
        /// desde el código del usuario.
        /// Si es false, el método es llamado por el finalizador
        /// y sólo los recursos no manejados son finalizados.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Preguntamos si Dispose ya fue llamado.
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Finalizamos
                    this.Stop();                    
                }

                // Aqui finalizamos correctamente los RECURSOS NO MANEJADOS
                // ...

            }
            this.disposed = true;
        }

        public bool Reload()
        {
            //TODO: Reload (se puede detener y reiniciar, pero no haría nada, se pueden borrar las variables de memoria)
            return false;
        }

        /// <summary>
        /// Destructor de la instancia
        /// </summary>
        ~RequestServerWCF()
        {
            this.Dispose(false);
        }
        #endregion
       
    }

}
