using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using System;
using Microsoft.Owin.Hosting;
using System.Reflection;
using Microsoft.AspNet.SignalR.Hubs;
using Common.Security;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using LoggerManager;

namespace Communication.SignalR
{
    [HubName("RequestHub")]
    public class RequestHub : Hub
    {
        ILogger log = LogFactory.GetLogger(typeof(RequestHub));

        #region Hub Interface

        public override Task OnConnected()
        {
            try
            {
                var token = Context.QueryString["token"];               
                string remoteIP = GetIpAddress();                

                log.Trace(string.Format("SignalR. Cliente conectado: [{0},{1}]", Context.ConnectionId, remoteIP));

                RegisterUserConnection(Context.ConnectionId, remoteIP, token);
            }
            catch (Exception ex)
            {
                log.Error("RequestHub. OnConnected()", ex);
            }
            return base.OnConnected();
        }


        public override System.Threading.Tasks.Task OnReconnected()
        {
            try
            {
                // Comprobamos a ver si la conexión estaba ya establecida            
                string clientId = Context.ConnectionId;


                var token = Context.QueryString["token"];


                // Desconectamos el usuario y la conexión
                string remoteIP = GetIpAddress();
                RemoveUserConnection(clientId, remoteIP);


                // El usuario no estaba logueado, enviamos el comando para volver a solicitar los datos de autenticación                
                log.Debug("OnReconnected(). Solicitando datos para la reconexión");

                if (token != null && token.Length > 0)
                {
                    log.Debug("Volviendo a reestablecer la configuración de la conexión del usuario");                    
                    RegisterUserConnection(Context.ConnectionId, remoteIP, token);
                }

                //log.Debug("Actualizando reconexión de cliente");
                
                Clients.Client(clientId).reconnectUser();
            }
            catch (Exception er)
            {
                log.Error("OnReconnected()", er);
            }

            return base.OnReconnected();
        }
        

        // TODO: Estaba comentado, no se si para que cuando se cierra una conexión no se elimine y se pueda reconectar? probar
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            try
            {
                string clientId = Context.ConnectionId;


                var token = Context.QueryString["token"];

                if (stopCalled)
                {
                    log.Debug("OnDisconnected(). El cliente {0} cerró la conexión", clientId);
                }
                else
                {
                    log.Debug("OnDisconnected(). El cliente {0} dió timeout", clientId);                    
                }

                // Desconectamos el usuario y la conexión
                string remoteIP = GetIpAddress();
                RemoveUserConnection(clientId, remoteIP);                
            }
            catch (Exception er)
            {
                log.Error("OnDisconnected()", er);
            }
            
            return base.OnDisconnected(stopCalled);
        }
        
        #endregion

        #region Gestiona las conexiones/desconexiones de los usuarios

        /// <summary>
        /// Registramos la conexión con los datos del usuario siempre y cuando nos llegue un token válido
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="token"></param>
        private void RegisterUserConnection(string connectionId, string ip, string token)
        {
            try
            {
                log.Trace("RegisterUserConnection(). ConnectionId: {0}, IP: {1}, Token: {2}", connectionId, ip, token);

                // Leemos el token que nos envían
                //string pwdConex = ""; // Se podría securizar cifrando el token por IP de la conexión para mejorar la seguridad

                Token tokenUsr = new Token(token);

                // Validamos que haya llegado un token válido
                if (tokenUsr != null && tokenUsr.EsValido)
                {
                    // Antes de agregarle, validamos tb que no hayan copiado y pegado uno antiguo ya caducado
                    if (DateTime.Now < new DateTime(tokenUsr.TickHora))
                    {
                        RegistrarConexionUsuario(connectionId, ip, tokenUsr);

                        log.Debug("Registrada conexión (IP: {0}, Conexión: {1}, Id_Usuario: {2})", ip, connectionId, tokenUsr.Id_Usuario);
                    }
                    else
                    {
                        // El token está caducado
                        log.Warning("El token enviado está caducado (IP: {0}, Token: {1}, Conexión: {2}, Id_Usuario: {3})", ip, token, connectionId, tokenUsr.Id_Usuario);
                    }
                }
                else
                {
                    // Token no válido
                    log.Warning("Token no válido (IP: {0}, Token: {1}, Conexión: {2})", ip, token, connectionId);
                }
            }
            catch (Exception er)
            {
                log.Error("RegisterUserConnection()", er);
            }
        }

        /// <summary>
        /// Registra la conexión del usuario y configura la conexión con los grupos del usuario, etc
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="token"></param>
        private static void RegistrarConexionUsuario(string connectionId, string ip, Token token)
        {
            ILogger log = LogFactory.GetLogger(typeof(RequestHub));

            try
            {
                log.Debug("RegistrarConexionUsuario(connectionId: {0}, IP: {1}, token: {2})", connectionId, ip, token);

                if (token != null)
                {
                    if (token.EsValido)
                    {
                        // Guardamos en una variable la lista de usuarios conectados y su relación con el id_Usuario
                        var connection = new Clases.Connection
                        {
                            IP = ip,
                            Token = token
                        };

                        // La clase en principo no es estática, así que accedemos al contexto
                        var context = GlobalHost.ConnectionManager.GetHubContext<RequestHub>();

                        // Gestional a conexión y vinculación al grupo
                        SignalRManager.GetInstance.Connections.ClientConnected(connectionId, connection, context, token);
                    }
                    else
                    {
                        log.Warning("SignalR. Token no válido");
                    }
                }
                else
                {
                    log.Warning("RegistrarConexionUsuario(). Token NULL (ClientID: {0})", connectionId);
                }
            }
            catch (Exception er)
            {
                log.Error("RegistrarConexionUsuario()", er);
            }
        }

        private static void RemoveUserConnection(string connectionId, string ip) //, Token token)
        {
            ILogger log = LogFactory.GetLogger(typeof(RequestHub));

            try
            {
                log.Debug("RemoveUserConnection(). ConnectionId: {0}, IP: {1}", connectionId, ip);

                // La clase en principo no es estática, así que accedemos al contexto
                var context = GlobalHost.ConnectionManager.GetHubContext<RequestHub>();

                // Gestiona la desconexión y liberación del grupo
                SignalRManager.GetInstance.Connections.ClientDisconnected(connectionId, context);
            }
            catch (Exception er)
            {
                log.Error("RemoveUserConnection()", er);
            }
        }

        #endregion

        #region Información de la conexión       
        /// <summary>
        /// Devuelve la IP de la conexión actual
        /// </summary>
        /// <returns></returns>
        protected string GetIpAddress()
        {
            string ipAddress = "";
            try
            {
                object tempObject = null;

                if (Context != null && Context.Request != null && Context.Request.Environment != null)
                    Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out tempObject);

                if (tempObject != null)
                {
                    ipAddress = (string)tempObject;

                    // Si localhost lo devuelve como V6, devolvemos en formato V4
                    if (ipAddress == "::1")
                        ipAddress = "127.0.0.1";
                }
            }
            catch (ObjectDisposedException)
            {
                log.Warning("GetIpAddress(). No se pudo obtener la IP del cliente, ya que la conexión ya fue liberada");
            }
            catch (Exception er)
            {
                //ILogger log = BSHP.LoggerManager.LogFactory.GetLogger(typeof(EccoHub));
                log.Error("GetIpAddress()", er);
            }

            return ipAddress;
        }

        #endregion

        #region Server to Client (métodos estáticos)

        public static bool Ping(string connectionId)
        {
            var sent = false;

            try
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<RequestHub>();

                var client = context.Clients.Client(connectionId);
                context.Clients.Client(connectionId).Ping();

                sent = true;
            }
            catch (Exception ex)
            {
                ILogger log = LogFactory.GetLogger(typeof(RequestHub));
                log.Error("RequestHub. Ping", ex);
            }
            return sent;
        }
        #endregion

        #region Client to Server

        /// <summary>
        /// El servidor recibe pong de un cliente
        /// </summary>
        public void Pong()
        {
            SignalRManager.GetInstance.Connections.PongReceived(Context.ConnectionId);
        }

        /// <summary>
        /// Método para comprobar que el servidor está funcionando y responde
        /// </summary>
        /// <returns></returns>
        public DateTime Test()
        {
            return DateTime.Now;
        }
        #endregion



        #region Información
        /// <summary>
        /// Devuelve la lista de usuarios conectados y configuraciones de los mismos
        /// </summary>
        public static IHubContext GetInternalConnections()
        {
            ILogger log = LogFactory.GetLogger(typeof(RequestHub));

            var context = GlobalHost.ConnectionManager.GetHubContext<RequestHub>();

            var Clientes = context.Clients;

            return context;
        }        
        #endregion

    }
}
