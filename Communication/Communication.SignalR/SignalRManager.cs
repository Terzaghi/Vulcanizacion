using System;
using Microsoft.Owin.Hosting;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using LoggerManager;
using Communication.Interfaces;
using Communication.SignalR.Clases;
using Communication.SignalR.DTO;

namespace Communication.SignalR
{
    public class SignalRManager : ICommunication
    {
        ILogger log = LogFactory.GetLogger(typeof(SignalRManager));

        #region Attributes

        public ActiveConnections Connections;

        private static SignalRManager _server;

        private IDisposable SignalR { get; set; }

        private string ServerURI;
        
        #endregion

        #region Properties

        public static SignalRManager GetInstance
        {
            get { return _server ?? (_server = new SignalRManager()); }
        }

        #endregion

        #region Constructor

        private SignalRManager()
        {
            // Gestiona los usuarios conectados
            this.Connections = new ActiveConnections();            
        }

        #endregion

        #region Events

        public event ClientConnectedEventHandler OnClientConnected;
        public event ClientDisconnectedEventHandler OnClientDisconnected;    
        
        #endregion

        #region ICommunication Interface

        public bool Start()
        {
            var started = false;

            try
            {                
                int puerto = 6969;
                if (ConfigurationManager.AppSettings["SignalR_Port"] != null)
                {
                    int.TryParse(ConfigurationManager.AppSettings["SignalR_Port"], out puerto);

                    log.Information("SignalR. Configuración para el puerto de singalR (Puerto: {0})", puerto);
                }
                else
                {
                    log.Information("SignalR. No se ha encontrado la configuración del puerto, usando configuración por defecto (Puerto: {0})", puerto);
                }
                ServerURI = string.Format("http://+:{0}", puerto);

                // Si ya está levantado, lo eliminamos antes y lo volvemos a levantar
                if (SignalR != null)
                {
                    SignalR.Dispose();
                    SignalR = null;
                    GC.Collect();
                }                
                SignalR = WebApp.Start<Startup>(ServerURI);                                

                started = SignalR != null ? true : false;

                log.Trace("StartSignalRServer: SignalR Server started at " + ServerURI);

                PingPong();
            }
            catch (TargetInvocationException ex)
            {
                log.Error("RequestHub. Start() SignalR Server failed to start. A server is already running on [{0}]", ServerURI != null ? ServerURI : "null", ex);
            }
            catch (Exception ex)
            {
                log.Error("RequestHub. Start()", ex);
            }

            return started;
        }

        public bool Stop()
        {
            var stopped = false;

            try
            {
                log.Debug("RequestHub. Stop() Se Procede a parar el servidor");

                if (SignalR != null)
                {
                    SignalR.Dispose();

                    stopped = true;                    
                }
            }
            catch (Exception ex)
            {
                log.Error("RequestHub. Stop()", ex);
            }
            return stopped;
        }

        #endregion

        #region Server to Client

        public bool SendPrensaAbierta(List<int> IdsUsuarios, long Id_Solicitud, int Id_Prensa, string Nombre_Prensa, DateTime fecha)
        {
            return RequestHub.SendPrensaAbierta(IdsUsuarios, Id_Solicitud, Id_Prensa, Nombre_Prensa, fecha);
        }

        public bool SendRequestStateChanged(List<int> IdsUsuarios, long Id_Solicitud, int Id_Prensa, StateToSend State)
        {
            return RequestHub.SendRequestStateChanged(IdsUsuarios, Id_Solicitud, Id_Prensa, State);
        }

        #endregion

        #region Connections

        public void Hub_ClientConnected(string connectionId, string ip, int Id_User)
        {
            ClientConnectedEventHandler clientConnected = OnClientConnected;
            if (clientConnected != null)
                clientConnected(connectionId, ip, Id_User);
        }

        public void Hub_ClientDisconnected(string connectionId, string ip, int Id_User)
        {
            ClientDisconnectedEventHandler clientDisconnected = OnClientDisconnected;
            if (clientDisconnected != null)
                clientDisconnected(connectionId, ip, Id_User);
        }

        #endregion

        #region Métodos Privados

        #region Ping Pong

        private double lapse = 60000; // 1 minute

        //Delegate for Async operation.
        private delegate void AsyncMethodCaller();
        private AsyncMethodCaller caller;

        private bool alive = true;

        private void PingPong()
        {
            caller = new AsyncMethodCaller(AsyncKeepAlive);

            if (!alive) alive = true;

            IAsyncResult result = caller.BeginInvoke(null, null);
        }

        private void AsyncKeepAlive()
        {
            bool createNew;

            log.Debug("Se crea el bucle asíncrono de ping pong");

            EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, Guid.NewGuid().ToString(), out createNew);

            if (!createNew)
            {
                eventWaitHandle.Set(); //Inform other process to stop
            }

            do
            {
                // La clase en principo no es estática, así que accedemos al contexto
                var context = GlobalHost.ConnectionManager.GetHubContext<RequestHub>();

                this.Connections.CheckPings(context);

                eventWaitHandle.WaitOne(TimeSpan.FromMilliseconds(lapse));

            } while (alive);
        }
        
        #endregion
        
        #endregion

        public List<Tuple<string, Connection>> GetConnections()
        {
            // Lo usaremos para validar si existe la conexión en el servidor
            var conexionesInternasSignalR = RequestHub.GetInternalConnections();

            var usuariosConectados = this.Connections.GetConnections();

            return usuariosConectados;
        }

        #region Interfaz IDisposeable

        // Indica si ya se llamo al método Dispose. (default = false)
        private bool disposed;

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
                    this.Stop();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Destructor de la instancia
        /// </summary>
        ~SignalRManager()
        {
            this.Dispose(false);
        }
        #endregion

    }
}
