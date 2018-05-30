using Common.Security;
using LoggerManager;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;

namespace Communication.SignalR.Clases
{
    public class ActiveConnections
    {
        ILogger log = LogFactory.GetLogger(typeof(ActiveConnections));

        #region Attributes

        // Almacena el connectionId, seguido de los datos de la conexión
        private Dictionary<string, Connection> _dicConexionesActivas { get; set; }
      

        #endregion

        #region Constructor

        public ActiveConnections()
        {
            this._dicConexionesActivas = new Dictionary<string, Connection>();
          
        }

        #endregion

        public void ClientConnected(string connectionId, Connection connection, IHubContext context, Token token)
        {
            try
            {
                if (!string.IsNullOrEmpty(connectionId))
                {
                    lock (_dicConexionesActivas)
                    {
                        if (_dicConexionesActivas.ContainsKey(connectionId))
                        {
                            log.Trace(string.Format("ClientConnected: Se actualiza la conexión {0}. IP: {1}",
                                      connectionId,
                                      connection != null && !string.IsNullOrEmpty(connection.IP) ? connection.IP : "null"));

                            UpdateConnection(connectionId, connection);
                        }
                        else
                        {
                            log.Trace(string.Format("ClientConnected: Nueva conexión subscrita {0}. IP: {1}",
                                      connectionId,
                                      connection != null && !string.IsNullOrEmpty(connection.IP) ? connection.IP : "null"));

                            // Guardamos tb la fecha en la que se agrega
                            connection.FechaConexion = DateTime.Now;                            

                            _dicConexionesActivas.Add(connectionId, connection);

                            // Agregamos también la conexión por IP
                        }
                    }
                   
                    // Lanzamos el evento
                    SignalRManager.GetInstance.Hub_ClientConnected(
                        connectionId,
                        (connection != null) ? connection.IP : "",                        
                        ((token != null) ? token.Id_Usuario : -1)
                    );

                }
            }
            catch (Exception er)
            {
                log.Error("ClientConnected", er);
            }
            
        }

        public void ClientDisconnected(string connectionId, IHubContext context)
        {
            try
            {
                if (!string.IsNullOrEmpty(connectionId))
                {
                    Token token = null;
                    string ip = null;                 

                    lock (_dicConexionesActivas)
                    {
                        if (_dicConexionesActivas.ContainsKey(connectionId))
                        {
                            if (log.isDebugEnabled)
                            {
                                Connection conex;
                                if (_dicConexionesActivas.TryGetValue(connectionId, out conex))
                                {
                                    if (conex.Token != null)
                                    {                                        
                                        token = conex.Token;                                        
                                        log.Debug("UnsubscribeClient(). Se eliminará la conexión para el Id_Usuario: {0}", conex.Token.Id_Usuario);
                                    }
                                    else
                                        log.Debug("UnsubscribeClient. Sin datos de la conexión de usuario. ConnectionId: {0}", connectionId);

                                    ip = conex.IP;
                                }
                                else
                                {
                                    log.Debug("UnsubscribeClient(). Sin datos de la conexión: {0}", connectionId);
                                }
                            }
                            log.Trace(string.Format("UnsubscribeClient: Se elimina connectionId [{0}]", connectionId));

                            // Lanzamos el evento de desconexión del usuario
                            SignalRManager.GetInstance.Hub_ClientDisconnected(
                                connectionId, 
                                ip,
                                (token != null) ? token.Id_Usuario : -1
                            );


                            _dicConexionesActivas.Remove(connectionId);
                        }
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("ClientDisconnected()", er);
            }
        }

        /// <summary>
        /// Busca el nombre de la conexión para un usuario que se haya identificado
        /// </summary>
        /// <param name="Id_Usuario"></param>
        /// <returns></returns>
        public List<string> GetConnectionsIdByUser(int Id_Usuario)
        {
            List<string> result = new List<string>();

            try
            {
                if (_dicConexionesActivas != null)
                {
                    foreach (var connection in _dicConexionesActivas)
                    {
                        if (connection.Value != null && connection.Value.Token != null)
                        {
                            Token tok = connection.Value.Token;

                            if (tok.Id_Usuario == Id_Usuario)
                            {                                
                                result.Add(connection.Key);
                                //break;
                            }
                        }
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("GetConnectionsIdByUser", er);
            }
            return result;
        }

        /// <summary>
        /// Devuelve la Ip del dispositivo a partir de la id de la conexión
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public string GetIpByConnectionId(string connectionId)
        {
            string ip = null;

            try
            {
                if (!string.IsNullOrEmpty(connectionId) && _dicConexionesActivas != null && _dicConexionesActivas.ContainsKey(connectionId))
                {
                    Connection conexion = null;

                    if (_dicConexionesActivas.TryGetValue(connectionId, out conexion) && conexion != null)
                    {
                        ip = conexion.IP;
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("GetIpByConnectionId", er);
            }

            return ip;
        }

        /// <summary>
        /// Devuelve una lista de conexiónes según la IP de la máquina
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public List<string> GetConnectionsByIP(string ip)
        {
            List<string> lstConnectionsId = new List<string>();

            try
            {
                if (_dicConexionesActivas != null)
                {
                    foreach (var connection in _dicConexionesActivas)
                    {
                        if (connection.Value != null && connection.Value.IP != null)
                        {
                            if (connection.Value.IP == ip)
                            {
                                lstConnectionsId.Add(connection.Key);
                            }
                        }
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("GetConnectionsByIP", er);
            }
            return lstConnectionsId;
        }       

        public Token GetToken(string connectionId)
        {
            Token token = null;

            try
            {
                if (_dicConexionesActivas != null && _dicConexionesActivas.ContainsKey(connectionId))
                {
                    Connection connection = null;
                    _dicConexionesActivas.TryGetValue(connectionId, out connection);

                    if (connection != null && connection.Token != null)
                    {
                        token = connection.Token;

                        if (token.EsValido)
                        {
                            // Validamos tb si el token es válido
                            if (DateTime.Now > new DateTime(token.TickHora))
                            {
                                token = null;
                                log.Debug("La validez del token ha caducado (Usuario: {0})", token.Id_Usuario);
                            }
                        }
                        else
                        {
                            token = null;
                            log.Debug("Token no válido (Usuario: {0})", token.Id_Usuario);
                        }
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("GetToken()", er);
            }

            return token;
        }

        public void PongReceived(string connectionId)
        {
            try
            {

                //if (!string.IsNullOrEmpty(connectionId) && pings.Contains(connectionId))
                if (!string.IsNullOrEmpty(connectionId))
                {
                    log.Trace(string.Format("Hub_PongReceived: Pong recibido de la conexión [{0}]", connectionId));

                    //pings.Remove(connectionId);
                }

                // Si existe, guardamos la fecha en la que nos respondió
                if (_dicConexionesActivas.ContainsKey(connectionId))
                {
                    Connection connection;

                    lock (_dicConexionesActivas)
                    {
                        if (_dicConexionesActivas.TryGetValue(connectionId, out connection))
                        {
                            log.Debug("Pong. Actualizando información de respuesta");

                            // Guardamosl a fecha en la que se actualizó
                            connection.FechaPong = DateTime.Now;

                            _dicConexionesActivas[connectionId] = connection;
                        }
                        else
                            log.Debug("Pong. Ya no existe la conexión (ConnectionId: {0})", connectionId);
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("PongReceived()", er);
            }
        }

        #region Private Interface

        private void UpdateConnection(string connectionId, Connection connection)
        {
            try
            {
                if (_dicConexionesActivas.ContainsKey(connectionId))
                {
                    lock (_dicConexionesActivas)
                    {
                        // Guardamosl a fecha en la que se actualizó
                        connection.FechaActualizacion = DateTime.Now;

                        _dicConexionesActivas[connectionId] = connection;
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("UpdateConnection(ConnectionID: {0})", er, connectionId);
            }
        }

        #endregion

        public List<Tuple<string, Connection>> GetConnections()
        {
            List<Tuple<string, Connection>> result = null;
            try
            {
                result = new List<Tuple<string, Connection>>();

                foreach (KeyValuePair<string, Connection> conex in this._dicConexionesActivas)
                {
                    result.Add(new Tuple<string, Connection>(conex.Key, conex.Value));
                }
            }
            catch (Exception er)
            {
                log.Error("GetConnections()", er);
            }

            return result;
        }

        public void CheckPings(IHubContext context)
        {
            try
            {
                log.Trace(string.Format("CheckPings() Clientes conectados: {0}", this._dicConexionesActivas != null ? this._dicConexionesActivas.Count : 0));

                var clientsLoggedOff = new List<string>();

                var tiempoComprobacion = 60000;

                lock (this._dicConexionesActivas)
                {
                    foreach (var client in this._dicConexionesActivas)
                    {
                        TimeSpan tiempoDesdeUltimoPong = DateTime.Now - client.Value.FechaPong;

                        if (tiempoDesdeUltimoPong.TotalMilliseconds > tiempoComprobacion)
                        {
                            // Hace mucho que no nos ha llegado un mensaje suyo (si no se ha inicializado dirá que es desde el año uno, por lo que serán muchos días), 
                            // en ese caso le solicitaremos tb un ping
                            if (tiempoDesdeUltimoPong.TotalDays < 10 && tiempoDesdeUltimoPong.TotalMilliseconds > tiempoComprobacion * 2)
                            {
                                // Hace 2 veces el tiempo indicado que no responde, lo marcamos para eliminarlo
                                log.Trace(string.Format("CheckPings() No se recibe respuesta de la conexión {0}. IP: {1}", client.Key, client.Value.IP));
                                clientsLoggedOff.Add(client.Key);
                            }
                            else
                            {
                                // Le mandamos un mensaje para que nos responda
                                log.Trace(string.Format("CheckPings() Se envía Ping a la conexión {0}. IP: {1}", client.Key, client.Value.IP));
                                RequestHub.Ping(client.Key);
                            }
                        }
                        else
                        {
                            // El cliente ha respondido hace no demasiado, está ok
                            // Le mandamos un mensaje para que nos responda
                            log.Trace(string.Format("CheckPings() Se reenvía Ping a la conexión {0}. IP: {1}", client.Key, client.Value.IP));
                            RequestHub.Ping(client.Key);
                        }
                    }
                }

                if (clientsLoggedOff.Count > 0)
                {
                    log.Trace("Eliminando clientes desconectados del diccionario");

                    foreach (var item in clientsLoggedOff)
                    {                                               
                        ClientDisconnected(item, context);
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("CheckPings()", er);
            }
        }
    }

    public class Connection
    {
        public string IP { get; set; }
        public Token Token { get; set; }

        public DateTime FechaConexion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaPong { get; set; }
    }
}
