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
                log.Error("EccoHub. OnConnected()", ex);
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
                string pwdConex = ""; // Se podría securizar cifrando el token por IP de la conexión para mejorar la seguridad

                Token tokenUsr = new Token(token, pwdConex);

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
        /// <summary>
        /// Envia una nueva notificación a usuario, clients o brodcast si no se especifica ninguno
        /// </summary>
        /// <param name="IdsUsuarios"></param>
        /// <param name="jsonRequest"></param>
        /// <param name="fecha"></param>
        public static bool SendRequest(List<int> IdsUsuarios, List<string> DevicesIPs, string jsonRequest, DateTime fecha, bool activa)
        {
            ILogger log = LogFactory.GetLogger(typeof(RequestHub));
            bool sw = false;

            try
            {
                log.Debug("YCM. Acceso al método SendNotification. Dispositivos: [{1}]. Usuarios: [{2}]",
                    DevicesIPs != null ? string.Join(",", DevicesIPs) : "null",
                    IdsUsuarios != null ? string.Join(",", IdsUsuarios) : "null");

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<RequestHub>();

                #region Usuarios
                if (IdsUsuarios != null && IdsUsuarios.Count > 0)
                {
                    log.Debug("SendRequest. Enviando solicitud a los usuarios...");

                    // Envío unos usuarios por identificador
                    foreach (int idUsuario in IdsUsuarios)
                    {
                        log.Debug("SendRequest. Enviando solicitud para el usuario: {0}", idUsuario);
                                                
                        List<string> lstConexiones = SignalRManager.GetInstance.Connections.GetConnectionsIdByUser((int)idUsuario);

                        if (lstConexiones != null && lstConexiones.Count > 0)
                        {
                            foreach (string connectionId in lstConexiones)
                            {
                                if (connectionId != null && connectionId.Length > 0)
                                {
                                    hubContext.Clients.Client(connectionId).SendRequest(jsonRequest, fecha, activa);
                                    sw = true;

                                    log.Debug("SendRequest. Enviada solicitud para el usuario: {0} (Id: {1})", connectionId, idUsuario);
                                }
                                else
                                {                                    
                                    log.Warning("SendRequest. Sin datos de la conexión (Id: {0})", idUsuario);
                                }
                            }
                        }
                        else
                        {
                            // El usuario no está en la lista de usuarios conectados 
                            log.Debug("SendRequest. No se puede mandar el mensaje al usuario ya que no está autenticado (Id: {0})", idUsuario);
                        }
                    }
                    //sw = true;
                }
                #endregion

                #region IPs
                // Envío a IPs
                if (DevicesIPs != null && DevicesIPs.Count > 0)
                {                    
                    log.Debug("SendRequest. Enviando solicitud a IPs...");

                    foreach (string ip in DevicesIPs)
                    {
                        log.Debug("IP: {0}. Enviando solicitud.", ip);

                        List<string> lstConexiones = SignalRManager.GetInstance.Connections.GetConnectionsByIP(ip);

                        if (lstConexiones != null && lstConexiones.Count > 0)
                        {
                            foreach (string connectionId in lstConexiones)
                            {
                                if (connectionId != null && connectionId.Length > 0)
                                {
                                    hubContext.Clients.Client(connectionId).SendNotification(jsonRequest, fecha, activa);
                                    sw = true;

                                    log.Debug("SendRequest. Enviada solicitud para el usuario: {0} (IP: {1})", connectionId, ip);
                                }
                                else
                                {
                                    log.Warning("SendRequest. Sin datos de la conexión (IP: {0})", ip);
                                }
                            }
                        }
                        else
                        {
                            // La IP no está en la lista de usuarios conectados 
                            log.Debug("SendRequest. Ningún usuario está conectado desde esa IP (IP: {0})", ip);
                        }
                    }
                }
                #endregion
            }
            catch (Exception er)
            {
                log.Error("SendRequest()", er);
            }

            return sw;
        }
               
        //public static bool SendChangeState(List<int> IdsUsuarios, List<int> IdsGrupos, List<string> DevicesIPs, NotificationToSend Notification, StateToSend State, ActionSender sender)
        //{
        //    ILogger log = LogFactory.GetLogger(typeof(RequestHub));

        //    bool sw = false;

        //    try
        //    {
        //        var hubContext = GlobalHost.ConnectionManager.GetHubContext<RequestHub>();

        //        #region Usuarios
        //        if (IdsUsuarios != null && IdsUsuarios.Count > 0)
        //        {
        //            log.Debug("SendChangeState. Enviando cambio de estado a los usuarios...");

        //            // Envío unos usuarios por identificador
        //            foreach (int idUsuario in IdsUsuarios)
        //            {
        //                log.Debug("SendChangeState. Enviando cambio de estado para el usuario: {0}", idUsuario);
                        
        //                List<string> lstConexiones = SignalRManager.GetInstance.Connections.GetConnectionsIdByUser((int)idUsuario);

        //                if (lstConexiones != null && lstConexiones.Count > 0)
        //                {
        //                    foreach (string connectionId in lstConexiones)
        //                    {
        //                        if (connectionId != null && connectionId.Length > 0)
        //                        {
        //                            hubContext.Clients.Client(connectionId).SendChangeState(Notification, State, sender);

        //                            log.Debug("SendChangeState. Enviada cambio de estado para el usuario: {0} (Id: {1})", connectionId, idUsuario);
        //                        }
        //                        else
        //                        {
        //                            log.Warning("SendChangeState. Sin datos de la conexión (Id: {0})", idUsuario);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    // El usuario no está en la lista de usuarios conectados 
        //                    log.Debug("SendChangeState. No se puede mandar el cambio de estado al usuario ya que no está autenticado (Id: {0})", idUsuario);
        //                }
        //            }
        //            sw = true;
        //        }
        //        #endregion

        //        #region Grupos
        //        if (IdsGrupos != null && IdsGrupos.Count > 0)
        //        {
        //            // Envio a un grupo entero de clientes
        //            log.Debug("SendChangeState. Enviando cambio de estado a los grupos...");

        //            foreach (int idGrupo in IdsGrupos)
        //            {
        //                log.Debug("Grupo: {0}. Enviando cambio de estado.", idGrupo);

        //                string nombreGrupo = idGrupo.ToString();

        //                hubContext.Clients.Group(nombreGrupo).SendChangeState(Notification, State, sender);
        //            }
        //            sw = true;
        //        }
        //        #endregion

        //        #region IPs                
        //        if (DevicesIPs != null && DevicesIPs.Count > 0)
        //        {
        //            log.Debug("SendChangeState. Enviando cambio de estado por IPs...");

        //            // Envío unos usuarios por identificador
        //            foreach (string ip in DevicesIPs)
        //            {
        //                log.Debug("SendChangeState. Enviando cambio de estado a la IP: {0}", ip);
                        
        //                List<string> lstConexiones = SignalRManager.GetInstance.Connections.GetConnectionsByIP(ip);

        //                if (lstConexiones != null && lstConexiones.Count > 0)
        //                {
        //                    foreach (string connectionId in lstConexiones)
        //                    {
        //                        if (connectionId != null && connectionId.Length > 0)
        //                        {
        //                            hubContext.Clients.Client(connectionId).SendChangeState(Notification, State, sender);

        //                            log.Debug("SendChangeState. Enviada cambio de estado a la IP: {0} (Id: {1})", connectionId, ip);
        //                        }
        //                        else
        //                        {
        //                            log.Warning("SendChangeState. Sin datos de la conexión (IP: {0})", ip);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    // El usuario no está en la lista de usuarios conectados 
        //                    log.Debug("SendChangeState. No se puede mandar el cambio de estado a la IP {0}, ya qu eno hay ningún usuario autenticado en esa máquina", ip);
        //                }
        //            }
        //            sw = true;
        //        }
        //        #endregion

        //        #region Broadcast (no hay destinatario configurado)
        //        // Si no hay usuarios ni clientes, manda a broadcast
        //        if ((IdsUsuarios == null || IdsUsuarios.Count == 0) && (IdsGrupos == null || IdsGrupos.Count == 0) && (DevicesIPs == null || DevicesIPs.Count == 0))
        //        {
        //            // Si no se especifica usuario ni clientes
        //            // Envío genérico a todos (brodcast)
        //            log.Debug("Enviando cambio de estado al brodcast");

        //            hubContext.Clients.All.SendChangeState(Notification, State, sender);

        //            sw = true;
        //        }
        //        #endregion
        //    }
        //    catch (Exception er)
        //    {
        //        log.Error("SendChangeState()", er);
        //    }

        //    return sw;
        //}

        public static bool SendChat(int? groupId, string deviceIp, string jsonChat)
        {
            // Enviaremos a los clientes los campos Id_Chat_Generated, groupId, deviceIp, senderName, fecha, message
            // Con groupId e deviceId sabemos a qué conversación agregar el mensaje. sender_name será el nombre de la conversación.
            ILogger log = LogFactory.GetLogger(typeof(RequestHub));
            bool sw = false;

            try
            {
                log.Debug("YCM. Acceso al método SendChat. Grupo: [{0}]. Dispositivo: [{1}]",
                    groupId.HasValue ? groupId.Value.ToString() : "null",
                    !string.IsNullOrEmpty(deviceIp) ? deviceIp : "null");

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<RequestHub>();

                #region Grupos
                if (groupId.HasValue)
                {
                    // Envio a un grupo entero de clientes
                    log.Debug("SendChat. Enviando chat al grupo...", groupId.Value);

                    string nombreGrupo = groupId.ToString();

                    hubContext.Clients.Group(nombreGrupo).SendChat(jsonChat);

                    sw = true;
                    
                }
                #endregion

                #region IP
                // Envío a IP
                if (!string.IsNullOrEmpty(deviceIp))
                {
                    log.Debug("IP: {0}. Enviando chat.", deviceIp);

                    List<string> lstConexiones = SignalRManager.GetInstance.Connections.GetConnectionsByIP(deviceIp);

                    if (lstConexiones != null && lstConexiones.Count > 0)
                    {
                        foreach (string connectionId in lstConexiones)
                        {
                            if (connectionId != null && connectionId.Length > 0)
                            {
                                hubContext.Clients.Client(connectionId).SendChat(jsonChat);
                                sw = true;

                                log.Debug("SendChat. Enviada notificación para el usuario: {0} (IP: {1})", connectionId, deviceIp);
                            }
                            else
                            {
                                log.Warning("SendChat. Sin datos de la conexión (IP: {0})", deviceIp);
                            }
                        }
                    }
                    else
                    {
                        // La IP no está en la lista de usuarios conectados 
                        log.Debug("SendChat. Ningún usuario está conectado desde esa IP (IP: {0})", deviceIp);
                    }
                }
                #endregion

                #region Broadcast (no hay destinatario configurado)
                // Si no hay usuarios ni clientes, manda a broadcast
                if (!groupId.HasValue&& string.IsNullOrEmpty(deviceIp))
                {
                    // Si no se especifica usuario ni clientes
                    // Envío genérico a todos (brodcast)
                    log.Debug("Enviando chat al brodcast");

                    hubContext.Clients.All.SendChat(jsonChat);

                    sw = true;
                }
                #endregion
            }
            catch (Exception er)
            {
                log.Error("SendChat()", er);
            }

            return sw;
        }

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
        /// Permite mandar un mensaje de chat desde un dispositivo a otro
        /// </summary>
        /// <param name="groupId">Grupo al que enviar el mensaje</param>
        /// <param name="deviceId">Dispositivo al que enviar el mensaje</param>
        /// <param name="message">Mensaje a enviar</param>
        public void PushChat(string conversationId, string groupId, string deviceId, string message)
        {
            try
            {
                log.Debug("RequestHub.PushChat({0}, {1}, {2}, {3})",
                                                             !string.IsNullOrEmpty(conversationId) ? conversationId : "null", 
                                                             !string.IsNullOrEmpty(groupId) ? groupId : "null",
                                                             !string.IsNullOrEmpty(deviceId) ? deviceId : "null",
                                                             !string.IsNullOrEmpty(message) ? message : "null");

                int? group = null, device = null;
                int g, d;

                long? conversation = null;
                long c;

                if (!string.IsNullOrEmpty(groupId) && int.TryParse(groupId, out g))
                    group = g;

                if (!string.IsNullOrEmpty(deviceId) && int.TryParse(deviceId, out d))
                    device = d;

                if (!string.IsNullOrEmpty(conversationId) && long.TryParse(conversationId, out c))
                    conversation = c;

            
            }
            catch (Exception er)
            {
                log.Error("RequestHub. PushChat()", er);
            }
        }

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


            //var aaaa = context.Clients.All();

            var Groups = context.Groups;

            var Clientes = context.Clients;

            return context;
        }        
        #endregion

    }
}
