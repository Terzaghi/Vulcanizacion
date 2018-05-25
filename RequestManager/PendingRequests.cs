using LoggerManager;
using Model.BL;
using Model.BL.DTO.Enums;
using RequestManager.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using RequestManager.DTO;
using static RequestManager.RequestMotor;

namespace RequestManager
{
    public class PendingRequests : IXmlSerializable
    {
        ILogger log = LogFactory.GetLogger(typeof(PendingRequests));

        private int _numeroMaximoColaSolicitudesPendientes = 1000;       // Maximo de solicitudes en memoria por defecto
        private double _ttlDefault = 604800;                             // TTL por defecto (en segundos).   Defecto 604800 (una semana)        

        private SortedDictionary<long, PendingRequestLogic> _dicSolicitudesGeneradas;

        private RequestMotor _refMotorSolicitudes = null;

        #region Constructor y carga de configuración

        private PendingRequests()
        {
            // Para el serializador/deserializador, sino es necesario inicializar con el parámetro del motor de reglas

            log.Debug("PendingNotifications(). Inicializando PendingNotifications");
            // Iniciamos el diccionario de notificaciones, que lo configuraremos como diccionario ordenado, con orden descendente
            this._dicSolicitudesGeneradas = new SortedDictionary<long, PendingRequestLogic>(new ReverseComparer<long>(Comparer<long>.Default));
        }

        public PendingRequests(RequestMotor motorSolicitudes): this()
        {
            try
            {
                log.Debug("PendingNotifications(ruleMotor). Cargando configuración del objeto");

                // Leemos la configuración del sistema            
                int maxPendientes;
                if (ConfigurationManager.AppSettings["MaximoColaSolicitudesPendientes"] != null)
                {
                    if (int.TryParse(ConfigurationManager.AppSettings["MaximoColaSolicitudesPendientes"], out maxPendientes))
                    {
                        this._numeroMaximoColaSolicitudesPendientes = maxPendientes;
                    }
                }
                log.Debug("El tamaño máximo de la cola de solicitudes pendientes se establece en: {0}", this._numeroMaximoColaSolicitudesPendientes);
                // Configuración TTL de sistema (si no tienen configurado un tiempo de vida)
                double ttlMax;
                if (ConfigurationManager.AppSettings["TTL_Solicitud"] != null)
                {
                    if (double.TryParse(ConfigurationManager.AppSettings["TTL_Solicitud"].Replace(".", ","), out ttlMax))
                    {
                        this._ttlDefault = ttlMax; // En minutos
                        log.Debug("TTL configurado por el sistema (TTL: {0} minutos (app.config={1}))", this._ttlDefault, ttlMax);
                    }
                }
                log.Debug("TTL máximo por defecto: {0}", this._ttlDefault);

                // Puntero al motor de reglas
                this._refMotorSolicitudes = motorSolicitudes;

                // Inicializamos
                InicializaHiloComprobacionesTTL();
            }
            catch (Exception er)
            {
                log.Error("HistoricalNotifications()", er);
            }
        }
        #endregion

        #region Timer comprobaciones y limpieza según TTL
        private System.Timers.Timer _tmr = null;

        private void InicializaHiloComprobacionesTTL()
        {
            log.Debug("Iniciado el hilo de limpieza de notificaciones");
            _tmr = new System.Timers.Timer();
            _tmr.Interval = 60000;              // Intervalo de comprobación de los TTL de las solicitudes
            _tmr.Elapsed += _tmr_Elapsed;
            _tmr.Start();
        }

        private void _tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            LimpiarCaducadasTTL();
        }

        /// <summary>
        /// Elimina las notificaciones que su tiempo de vida haya caducado
        /// </summary>
        private void LimpiarCaducadasTTL()
        {
            try
            {
                log.Debug("LimpiarCaducadasTTL(). Comprobando solicitudes a eliminar");

                //List<int> lstElementosEliminar = new List<int>();
                List<Tuple<long, int>> lstElementosEliminar = new List<Tuple<long, int>>();

                RequestGenerated solicitud;

                lock (this._dicSolicitudesGeneradas)
                {
                    foreach (KeyValuePair<long, PendingRequestLogic> entry in this._dicSolicitudesGeneradas)
                    {
                        //notificacion = entry.Value.NotificationGenerated;
                        solicitud = entry.Value.GetRequestGenerated();

                        bool eliminar = false;

                        if (solicitud != null)
                        {
                            double ttl = solicitud.TTL;
                            // Si la notificación no tiene configurado un tiempo máximo, le ponemos el del sistema
                            if (ttl == 0 && this._ttlDefault > 0)
                            {
                                ttl = this._ttlDefault;
                            }

                            // Comprobamos si el ttl configurado por la notificación sobrepasa al nuestro, para ponerlo en tal caso como máximo de la notificación
                            if (ttl > this._ttlDefault)
                                ttl = this._ttlDefault;

                            // Validación por tiempo de vida (si están a 0 son de tiempo ilimitado)
                            if (ttl > 0)
                            {

                                if (solicitud.DateGeneration != null)
                                {
                                    TimeSpan ts = DateTime.Now - solicitud.DateGeneration;

                                    //if (ts.TotalSeconds > ttl)
                                    if (ts.TotalMinutes > ttl)
                                    {
                                        log.Debug("El tiempo de vida de la solicitud ha finalizado. Marcada para su eliminación... (Id_RequestGenerated: {0})", entry.Key);
                                        eliminar = true;
                                    }
                                }
                                else
                                {
                                    log.Debug("La solicitud no tiene configurada una hora de creación. Marcada para su eliminación... (Id_RequestGenerated: {0})", entry.Key);
                                    eliminar = true;
                                }
                            }

                        }
                        else
                        {
                            eliminar = true;
                        }

                        if (eliminar)
                        {
                            lstElementosEliminar.Add(new Tuple<long, int>(
                                entry.Key,
                                ((solicitud != null) ? solicitud.Id_Request : 0)
                            ));
                        }
                    }
                }

                // Eliminamos los objetos
                if (lstElementosEliminar.Count > 0)
                {
                    log.Debug("Se limpiarán {0} solicitudes que han superado TTL de un total de {1}", lstElementosEliminar.Count, this._dicSolicitudesGeneradas.Count);
                    lock (this._dicSolicitudesGeneradas)
                    {
                        long id_solicitudGen;
                        int id_solicitud;

                        foreach (var elemento in lstElementosEliminar)
                        {
                            id_solicitudGen = elemento.Item1;
                            id_solicitud = elemento.Item2;

                            // Comprobamos primero, antes de eliminarla si está activa                            
                            bool estaActiva = false;
                            if (this._refMotorSolicitudes != null)
                                estaActiva = this._refMotorSolicitudes.IsRequestGeneratedActive(id_solicitudGen);
                          

                            if (!estaActiva)
                            {
                                // Elimina de la cola de pendientes                               
                                EliminarSolicitudActiva(id_solicitudGen, id_solicitud);
                            }
                            else
                            {
                                log.Debug("No se eliminará la solicitud de las colas de memoria ya que permanece activa (Id_RequestGenerated: {0})", id_solicitudGen);
                            }
                        }
                    }
                }


            }
            catch (Exception er)
            {
                log.Error("LimpiarCaducadasTTL()", er);
            }
        }
        #endregion
        #region Agregar Solicitudes
        /// <summary>
        /// Agrega en la lista de tareas pendientes una nueva solicitud
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public long Add(RequestGenerated requestGenerated)
        {
              long Id_RequestGenerated = -1;

                try
                {
                    log.Debug("MemoryGeneratedNotifications. Add(). ");
                    // Guardamos el histórico del envío y generamos un Id_RequestGenerada 
                    
                    DateTime fchGeneracion = DateTime.Now;
                    Historico_Solicitud modHistoricoSolicitudes = new Historico_Solicitud();


                    // Genera el Id_NotificationGenerated antes del envío de la acción
                    if (requestGenerated.Id_Request != 0)
                    {
                        Id_RequestGenerated = modHistoricoSolicitudes.Agregar(new Model.BL.DTO.Historico_Solicitud()
                        {
                            Id_Solicitud = requestGenerated.Id_Request,
                            Fecha = fchGeneracion
                        });
                    }
                    else
                    {
                        log.Debug("MemoryGeneratedNotifications(). Notificación sin identificador (Id_Notification). La acción no llevaba vinculada una notificación");
                    }


                    // Si el objeto viene completo lo almacenamos en memoria
                    if (requestGenerated != null)
                    {
                        // El objeto notificationGenerated lo crea el send del action en su base, actualizamos la fecha
                        requestGenerated.Id_Request_Generated = Id_RequestGenerated;
                        requestGenerated.DateGeneration = fchGeneracion;                        // byRef

                        if (Id_RequestGenerated > 0)
                        {
                            // Almacena en BD los valores de los tags cuando se produjo la notificación
                           // GuardarTagParameters((long)Id_NotificationGenerated, notification);
                                                      
                                lock (_dicSolicitudesGeneradas)
                                {
                                // La notificación no existirá en memoria, pero lo validamos para agregarla
                                if (_dicSolicitudesGeneradas.ContainsKey((long)requestGenerated.Id_Request_Generated))
                                    {
                                        log.Warning("Atención, la solicitud generada ya está en memoria");
                                        _dicSolicitudesGeneradas.Remove((long)requestGenerated.Id_Request_Generated);
                                    }

                                    // Almacenamos la solicitud en memoria                            
                                    _dicSolicitudesGeneradas.Add((long)requestGenerated.Id_Request_Generated, new PendingRequestLogic(requestGenerated, this._refMotorSolicitudes));

                                    // Comprobamos si al agregar una nueva solicitud, nos excedemos de la configuración del máximo
                                    // de notoficaciones pendientes en memoria
                                    if (this._numeroMaximoColaSolicitudesPendientes > 0 && this._dicSolicitudesGeneradas.Count >= this._numeroMaximoColaSolicitudesPendientes)
                                    {
                                        log.Debug("Eliminando solicitud antigua para no exceder el tamaño de la configuración en memoria (Max: {0})", this._numeroMaximoColaSolicitudesPendientes);


                                        // Obtenemos la solicitud más antigua para eliminarla
                                        var solicitudAntigua = this._dicSolicitudesGeneradas.Last();
                                        

                                        // El elemento que se eliminará
                                        long antigua_idSolicitudGen = solicitudAntigua.Key;
                                        // Parámetro extra para enviar en el evento que lanza
                                        long antigua_idSolicitud;
                                        var nOld = solicitudAntigua.Value.GetRequestGenerated();
                                        if (nOld != null)
                                        antigua_idSolicitud = nOld.Id_Request_Generated;
                                        else
                                        {
                                            log.Debug("No tenía una solicitud vinculada");
                                            antigua_idSolicitud = -1;
                                        }

                                        // La quitamos de la lista a tratar
                                        EliminarSolicitudActiva(antigua_idSolicitudGen, antigua_idSolicitud);
                                    }
                                }
                                log.Debug("Almacenada en memoria nueva notificación generada");
                            
                        }
                        else
                        {
                            log.Warning("La notificación recibida no contiene un identificador de notificación generada");
                        }
                    }
                    else
                    {
                        // Pudiera haber envios que no tuvieran definida una notificación vinculada
                        // como por ejemplo un envío de un comando de operación
                        log.Debug("Se recibió una notificación nula (viene así cuando por ejemplo es un compando de opreación)");
                    }
                
            }
            catch (Exception er)
            {
                log.Error("Add()", er);
            }
            return 0;
          
        }
        #endregion
        #region Obtención de datos
        /// <summary>
        /// Filtra los datos para el usuario/grupo/dispositivo indicado y devuelve los elementos pertinentes
        /// </summary>
        /// <param name="Id_User"></param>
        /// <param name="Id_Device"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public List<RequestGenerated> GetRequest(int? Id_User, int? Id_Device, int elements)
        {
            List<RequestGenerated> result = new List<RequestGenerated>();

            try
            {
                log.Debug("GetRequests(). Solicitado listado de solicitudes pendientes (Id_User: {0})",
                    ((Id_User != null) ? Id_User.ToString() : "")
                );

                // Puntero al diccionario de solicitudes, ya ordenado descendentemente para tratar y devolver las más nuevas primero (para limitar el número de ellas a devolver)
                var datosOrdenados = this._dicSolicitudesGeneradas;

                // Filtramos y devolvemos los objetos presentes de solictudes pendientes
                PendingRequestLogic valor;
                foreach (KeyValuePair<long, PendingRequestLogic> entry in datosOrdenados)
                {
                    valor = entry.Value;

                    // Agrega las solicitudes que son para nosotros
                    if (valor.IsValidDestinatary(Id_User, Id_Device))
                    {
                        // Ocultamos las que nosotros ya tengamos marcadas como reconocidas
                        var estado = valor.GetState(Id_User, Id_Device);

                        // Si ya la han visto no se la mostramos, a no ser que requiera ack y nadie la haya reconocido
                        bool requiereAck = false,
                             estaActiva = false;
                        RequestGenerated requestGenerated = valor.GetRequestGenerated();

                        if (valor != null && requestGenerated != null)
                        {
                            if (requestGenerated.AckRequiered)
                            {
                                requiereAck = true;

                                var estadoTodosUsuarios = valor.GetState();

                                //Compruebo si el usuario o el dispositivo debe reconocer la notificacion. En caso afirmativo obtengo el estado más bajo de ambos
                                List<Estado_Solicitud> ownStates = new List<Estado_Solicitud>();

                                if (requestGenerated.AckRequiered_AllUsers)
                                {
                                    var estadoUsuario = valor.GetUserState(Id_User);

                                    if (estadoUsuario.HasValue)
                                        ownStates.Add(estadoUsuario.Value);
                                }

                                if (requestGenerated.AckRequiered_AllDevices)
                                {
                                    var estadoDispositivo = valor.GetDeviceState(Id_Device);

                                    if (estadoDispositivo.HasValue)
                                        ownStates.Add(estadoDispositivo.Value);
                                }

                                if (requestGenerated.AckRequiered_AllUsers || requestGenerated.AckRequiered_AllDevices)
                                {
                                    if (ownStates.Count > 0 && ownStates.All(a => a == Estado_Solicitud.Aceptada))
                                        requiereAck = false;
                                }
                                else if (estadoTodosUsuarios == Estado_Solicitud.Aceptada)
                                    requiereAck = false; // Si requiere ack, pero ya la han reconocido otros, no hace falta nuestro ack
                            }


                            // Comprobamos si está activa la regla en el motor de reglas
                            if (this._refMotorSolicitudes!= null)
                                estaActiva = this._refMotorSolicitudes.IsRequestGeneratedActive((long)requestGenerated.Id_Request_Generated);

                        }

                        // Se puede solicitar que sea visualizada al menos con display requiered, así como mostrar las que requieren ack sin o están reconocidas aún
                        if ((estado == null || (estado <= Estado_Solicitud.Aceptada && requestGenerated.DisplayRequiered)) || (requiereAck) || (estaActiva))
                        {
                            result.Add(requestGenerated);

                            // Filtramos para devolver un máximo de X elementos pasado como parámetro
                            if (result.Count >= elements)
                                break;
                        }
                    }
                }

                log.Debug("Total en memoria: {0}, Filtradas para usuario: {1} (max: {2})", this._dicSolicitudesGeneradas.Count, result.Count, elements);
            }
            catch (Exception er)
            {
                log.Error("GetNotifications()", er);
            }

            return result;
        }

        /// <summary>
        /// Devuelve el estado más alto de la notificación entre todos los enviados
        /// </summary>
        /// <param name="Id_NotificationGenerated"></param>
        /// <returns></returns>
        public Estado_Solicitud GetState(long Id_RequestGenerated)
        {
            Estado_Solicitud? estado = null;

            try
            {
                PendingRequestLogic solicitudLog;

                if (this._dicSolicitudesGeneradas.TryGetValue(Id_RequestGenerated, out solicitudLog))
                {
                    estado = solicitudLog.GetState();
                }
            }
            catch (Exception er)
            {
                log.Error("GetState()", er);
            }

            if (estado == null) estado = Estado_Solicitud.Pendiente;

            return (Estado_Solicitud)estado;
        }
        /// <summary>
        /// Filtra los datos para el usuario/grupo/dispositivo indicado y devuelve los elementos pertinentes
        /// </summary>
        /// <param name="Id_User"></param>
        /// <param name="Id_Device"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public List<RequestGenerated> GetNotifications(int? Id_User, int? Id_Device, int elements)
        {
            List<RequestGenerated> result = new List<RequestGenerated>();

            try
            {
                log.Debug("GetRequests(). Solicitado listado de requests pendientes (Id_User: {0})",
                    ((Id_User != null) ? Id_User.ToString() : ""));

                // Puntero al diccionario de solicitudes, ya ordenado descendentemente para tratar y devolver las más nuevas primero (para limitar el número de ellas a devolver)
                var datosOrdenados = this._dicSolicitudesGeneradas;

                // Filtramos y devolvemos los objetos presentes de solicitudes pendientes
                PendingRequestLogic valor;
                foreach (KeyValuePair<long, PendingRequestLogic> entry in datosOrdenados) //this._dicNotifGeneradas.Reverse())
                {
                    valor = entry.Value;

                    // Agrega las notificaciones que son para nosotros
                    if (valor.IsValidDestinatary(Id_User, Id_Device))
                    {
                        // Ocultamos las que nosotros ya tengamos marcadas como reconocidas
                        var estado = valor.GetState(Id_User, Id_Device);

                        // Si ya la han visto no se la mostramos, a no ser que requiera ack y nadie la haya reconocido
                        bool requiereAck = false,
                             estaActiva = false;
                        RequestGenerated requestGenerated = valor.GetRequestGenerated();

                        if (valor != null && requestGenerated != null)
                        {
                            if (requestGenerated.AckRequiered)
                            {
                                requiereAck = true;

                                var estadoTodosUsuarios = valor.GetState();

                                //Compruebo si el usuario o el dispositivo debe reconocer la notificacion. En caso afirmativo obtengo el estado más bajo de ambos
                                List<Estado_Solicitud> ownStates = new List<Estado_Solicitud>();

                                if (requestGenerated.AckRequiered_AllUsers)
                                {
                                    var estadoUsuario = valor.GetUserState(Id_User);

                                    if (estadoUsuario.HasValue)
                                        ownStates.Add(estadoUsuario.Value);
                                }

                                if (requestGenerated.AckRequiered_AllDevices)
                                {
                                    var estadoDispositivo = valor.GetDeviceState(Id_Device);

                                    if (estadoDispositivo.HasValue)
                                        ownStates.Add(estadoDispositivo.Value);
                                }

                                if (requestGenerated.AckRequiered_AllUsers || requestGenerated.AckRequiered_AllDevices)
                                {
                                    if (ownStates.Count > 0 && ownStates.All(a => a == Estado_Solicitud.Aceptada))
                                        requiereAck = false;
                                }
                                else if (estadoTodosUsuarios == Estado_Solicitud.Aceptada)
                                    requiereAck = false; // Si requiere ack, pero ya la han reconocido otros, no hace falta nuestro ack
                            }


                            // Comprobamos si está activa la regla en el motor de solicitudes
                            if (this._refMotorSolicitudes != null)
                                estaActiva = this._refMotorSolicitudes.IsRequestGeneratedActive((long)requestGenerated.Id_Request_Generated);

                        }

                        // Se puede solicitar que sea visualizada al menos con display requiered, así como mostrar las que requieren ack sin o están reconocidas aún
                        if ((estado == null || (estado <= Estado_Solicitud.Mostrada && requestGenerated.DisplayRequiered)) || (requiereAck) || (estaActiva))
                        {
                            result.Add(requestGenerated);

                            // Filtramos para devolver un máximo de X elementos pasado como parámetro
                            if (result.Count >= elements)
                                break;
                        }
                    }
                }

                log.Debug("Total en memoria: {0}, Filtradas para usuario: {1} (max: {2})", this._dicSolicitudesGeneradas.Count, result.Count, elements);
            }
            catch (Exception er)
            {
                log.Error("GetNotifications()", er);
            }

            return result;
        }

        #endregion
        #region Cambiar de estado una Solicitud

        public void MarkAs_Async(long Id_RequestGenerated, Estado_Solicitud state, int? id_Usuario, int? id_Device)
        {
            try
            {
                
            }
            catch (Exception er)
            {
                log.Error("MarkAs_Async()", er);
            }
        }

        public void MarkAllAs_Async(long[] Ids_RequestGenerated, Estado_Solicitud state, int? id_Usuario, int? id_Device)
        {
            try
            {
                
            }
            catch (Exception er)
            {
                log.Error("MarkAllAs_Async()", er);
            }
        }


        /// <summary>
        /// Establece el estado para una solicitud pendiente para la conexión de un usuario, eliminandola de la cola de pendientes cuando sea necesario.
        /// Almacena también en base de datos el cambio de estado del mismo
        /// </summary>
        /// <param name="Id_NotificationGenerated"></param>
        /// <param name="state"></param>
        /// <param name="id_Usuario"></param>
        /// <param name="id_Device"></param>
        /// <returns></returns>
        public long MarkAs(long Id_RequestGenerated, Estado_Solicitud state, int? id_Usuario, int? id_Device)
        {
            long idModeloCambioEstado = -1;

            try
            {
                             
            }
            catch (Exception er)
            {
                log.Error("MarkAs()", er);
            }

            return idModeloCambioEstado;
        }




        #endregion

        #region Solicitudes, comprobación si están activas

        private void EliminarSolicitudActiva(long Id_RequestGenerated, long Id_Request)
        {
            try
            {
                // Lanzamos, las acciones que avisen que ha expirado            
                PendingRequestLogic requestLogic;
                _dicSolicitudesGeneradas.TryGetValue(Id_RequestGenerated, out requestLogic);

                // Se solicita que las expiradas, pero que ya hayan sido reconocidas, no se almacene la traza
                bool swGuardarTraza = true;
                var estadoMayor = requestLogic.GetState();
                if (estadoMayor != null && estadoMayor >= Estado_Solicitud.Aceptada)
                    swGuardarTraza = false;

                if (swGuardarTraza)
                {
                    // Guardamos en BD y la marcamos como expirada                
                    MarkAs(Id_RequestGenerated, Estado_Solicitud.Expirada, null, null);
                }
                else
                {
                    log.Trace("La notificación se ha eliminado de memoria, pero ya había sido reconocida. No se guardará traza como expirada (Id_NotificationGenerated: {0})", Id_RequestGenerated);
                }

                // Eliminamos de la cola de pendientes
                log.Debug("Eliminando notificación de memoria (Id_NotificationGenerated: {0})", Id_RequestGenerated);
                this._dicSolicitudesGeneradas.Remove(Id_RequestGenerated);

                // Lanzamos un evento informando de que la solicitud ha expirado
                this._refMotorSolicitudes.OnRequestExpired(requestLogic, new RequestExpiredEventArgs() { Id_Request = Id_Request, Id_RequestGenerated = Id_RequestGenerated });
            }
            catch (Exception er)
            {
                log.Error("EliminarRequestActiva()", er);
            }
        }
        #endregion

        #region Restaurar clase tras reinicio

        public SortedDictionary<long, PendingRequestLogic> GetAll()
        {
            return this._dicSolicitudesGeneradas;
        }

        /// <summary>
        /// Restaura un diccionario completo de notificaciones a memoria
        /// </summary>
        /// <param name="notifications"></param>
        /// <returns></returns>
        public int LoadValues(SortedDictionary<long, PendingRequestLogic> requests)
        {
            try
            {
                log.Debug("Agregando a las estructuras de memoria las notificaciones pendientes almacenadas en temp");
                this._dicSolicitudesGeneradas = requests;

                // Una vez cargadas limpia las que ya no valgan por TTL
                LimpiarCaducadasTTL();
            }
            catch (Exception er)
            {
                log.Error("LoadValues()", er);
            }

            return this._dicSolicitudesGeneradas.Count;
        }
        #endregion

        #region IXmlSerializable
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            try
            {
                // Iniciamos la variable
                this._dicSolicitudesGeneradas.Clear();

                reader.Read();
                reader.ReadStartElement("dictionary");
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    string strJson = reader.ReadElementString("item");

                    //PendingNotificationLogic result = Newtonsoft.Json.JsonConvert.DeserializeObject<PendingNotificationLogic>(strJson, new Newtonsoft.Json.JsonSerializerSettings
                    //{
                    //    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
                    //});

                    //reader.ReadEndElement();
                    reader.MoveToContent();

                    // Agregamos el valor
                    //this._dicRequestGeneradas.Add((long)result.GetNotificationGenerated().Id_Notification_Generated, result);
                }
                reader.ReadEndElement();
            }
            catch (Exception er)
            {
                log.Error("ReadXml()", er);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("dictionary");
            foreach (KeyValuePair<long, PendingRequestLogic> entry in this._dicSolicitudesGeneradas)
            {
                writer.WriteStartElement("item");

                string result = Newtonsoft.Json.JsonConvert.SerializeObject(entry.Value, new Newtonsoft.Json.JsonSerializerSettings
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
                });

                writer.WriteString(result);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        #endregion
    }
}
