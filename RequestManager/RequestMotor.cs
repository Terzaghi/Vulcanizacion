
using DataProvider.Interfaces;
using LoggerManager;
using Memory.Common;
using Model.BL.DTO.Enums;
using RequestManager.DTO;
using RuleManager.Clases;
using RuleManager.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ValuesMemory;

namespace RequestManager
{
    public class RequestMotor : IDisposable
    {
        ILogger log = LogFactory.GetLogger(typeof(RequestMotor));

        private Dictionary<int, Request> _dicRequest { get; set; }
        private MemoryValues _valoresMemoria;
        private PendingRequests _solicitudesGeneradas; // Solicitudes generadas y en memoria
        private IDataProvider _proveedorDatos;

        private const int TIEMPO_COMPROBACIONES = 60000;  // Tiempo predeterminado 60000 (tiempo mínimo)


        private DictionaryActiveRequests _estadoSolicitudesGeneradas;
        public RequestMotor(ref MemoryValues valoresMemoria, ref IDataProvider proveedorDatos)
        {
            this._valoresMemoria = valoresMemoria;
            this._solicitudesGeneradas = new PendingRequests(this);
            this._proveedorDatos = proveedorDatos;

            _estadoSolicitudesGeneradas = new DictionaryActiveRequests();
            CargarSolicitudes();
            // Algunas solicitudes llevan condiciones con comprobaciones de tiempo, inicializamos un hilo que lo comprueba
            InicializaTimerSolicitudes();
        }

        #region Cargar Solicitudes        
        /// <summary>
        /// Carga todas las reglas solicitudes en el sistema
        /// </summary>
        private void CargarSolicitudes()
        {
            try
            {
                log.Debug("Cargando solicitudes del sistema");

                LoadRequests solicitudes = new LoadRequests();
                List<Request> lstSolicitudes = solicitudes.Cargar();

                // Creamos un diccionario con las solicitudes configuradas en el sistema
                this._dicRequest = new Dictionary<int, Request>();
                

                foreach (var solicitud in lstSolicitudes)
                {
                    AgregarSolicitud(solicitud.Id_Request);
                }

                log.Debug("Se han cargado {0} solicitudes", ((lstSolicitudes != null) ? lstSolicitudes.Count.ToString() : "NULL"));
            }
            catch (Exception er)
            {
                log.Error("CargarSolicitudes()", er);
            }
        }

        /// <summary>
        /// Carga (agrega) una solicitud por Id
        /// </summary>
        /// <param name="Id_Request"></param>
        private bool AgregarSolicitud(int Id_Request)
        {
            bool sw = false;

            try
            {
                log.Debug("Agregar solicitud: {0}", Id_Request);

                LoadRequests solicitudes = new LoadRequests();
                List<Request> lstSolicitudes = solicitudes.CargarSolicitud(Id_Request);

                if (lstSolicitudes != null && lstSolicitudes.Count > 0)
                {
                    // Solo hay una solicitud, devuelve una lista para compartir la estructura del objeto y por si se quiere pasar posteriormente un listado
                    foreach (var solicitud in lstSolicitudes)
                    {
                        sw = AgregarSolicitud(solicitud.Id_Request);
                        log.Debug("Se ha agregado la solicitud (Id_Request: {0}), al request manager", Id_Request);
                    }
                }
                else
                {
                    log.Warning("No se ha agregado ninguna solicitud (Id_Request: {0})", Id_Request);
                }
            }
            catch (Exception er)
            {
                log.Error("AgregarSolicitudes()", er);
            }

            return sw;
        }

   
            
        private bool EliminarSolicitud(int Id_Request)
        {
            bool sw = false;

            try
            {
                if (this._dicRequest.ContainsKey(Id_Request))
                {
                    log.Debug("Solicitud encontrada, se va a proceder a detener (Id_Request: {0})", Id_Request);

                    // Buscamos en la lista de tags vinculados a los que pertenece y los quitamos
                    Request solicitud;
                    lock (this._dicRequest)
                    {
                        if (this._dicRequest.TryGetValue(Id_Request, out solicitud))
                        {
                            this._dicRequest.Remove(Id_Request);
                        }
                    }

                    sw = true;
                }
                else
                {
                    log.Debug("La solicitud que intenta eliminar no está activa en memoria (Id_Request: {0})", Id_Request);
                }
            }
            catch (Exception er)
            {
                log.Error("EliminarSolicitud()", er);
            }

            return sw;
        }

       
        #endregion
        #region Solicitudes con comprobaciones periodicas (contienen condiciones de tiempo)
        private System.Timers.Timer _timerSolicitudes;

        private void InicializaTimerSolicitudes()
        {
            try
            {
                int intervalo = TIEMPO_COMPROBACIONES;

                this._timerSolicitudes = new System.Timers.Timer();
                this._timerSolicitudes.Interval = intervalo;
                this._timerSolicitudes.Elapsed += _timerSolicitudes_Elapsed;

                // Lanzamos el evento nada más inicializar para comprobarlo
                _timerSolicitudes_Elapsed(null, null);

                // Iniciamos ya las comprobaciones periódicas
                this._timerSolicitudes.Start();
            }
            catch (Exception er)
            {
                log.Error("InicializaTimerReglas()", er);
            }
        }

        private void _timerSolicitudes_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
             
            }
            catch (Exception er)
            {
                log.Error("_timerReglas_Elapsed()", er);
            }
        }
        #endregion

        #region Métodos de consulta
        public List<Request> ListRequest()
        {
            List<Request> lstResult = new List<Request>();

            try
            {
                foreach (KeyValuePair<int, Request> entry in this._dicRequest)
                {
                    lstResult.Add(entry.Value);
                }
            }
            catch (Exception er)
            {
                log.Error("ListRules()", er);
            }

            return lstResult;
        }
        

        /// <summary>
        /// Devuelve el listado de solicitudes pendientes de visualizar según la configuración del usuario
        /// </summary>
        /// <param name="Id_User"></param>
        /// <returns></returns>        
        public List<RequestGenerated> ListPendingRequest(int? Id_User, int? Id_Device, int elements)
        {
            return this._solicitudesGeneradas.GetRequest(Id_User, Id_Device, elements);
        }


        /// <summary>
        /// Devuelve el conjunto de reglas activas a partir de un listado de ellas
        /// </summary>
        public List<Request> ListActiveRequest(List<int> Ids_Request)
        {
            List<Request> solicitudes = null;

            try
            {
                if (this._dicRequest != null && Ids_Request != null)
                {
                    //Obtenemos las solicitudes cumplidas filtradas por Ids_Request
                    var solicitudesFiltradas = _dicRequest.Where(a => Ids_Request.Any(b => b.Equals(a.Value.Id_Request)) );

                    solicitudes = new List<Request>();
                    foreach (KeyValuePair<int, Request> dicRequest in solicitudesFiltradas)
                    {
                        solicitudes.Add(dicRequest.Value);
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("ListActiveRules.", er);
            }
            return solicitudes;
        }

        /// <summary>
        /// Devuelve el listado de solicitudes pendientes de visualizar según la configuración de usuario, 
        /// agregando también las características relativas al estado de la regla (activa o no)
        /// </summary>
        /// <param name="Id_User"></param>
        /// <param name="Id_Device"></param>
        
        /// <returns></returns>
        public List<RequestWithStates> ListPendingRequestsWithState(int? Id_User, int? Id_Device,int elements)
        {
            var result = new List<RequestWithStates>();

            try
            {
                // Cargamos de la cola de memoria de reglas pendientes de entrega            
                var lstSolicitudes = this._solicitudesGeneradas.GetNotifications(Id_User, Id_Device, elements);

                // Marcamos el estado según las reglas activas
                foreach (var solicitud in lstSolicitudes)
                {
                    // Buscamos el estado de la solicitud
                    Estado_Solicitud estado = Estado_Solicitud.Pendiente;
                    if (solicitud != null)
                        estado = this._solicitudesGeneradas.GetState((long)solicitud.Id_Request_Generated);

                    // Creamos el objeto con sus propiedades
                    result.Add(new RequestWithStates()
                    {
                        Request = solicitud,
                        State = estado
                    });
                }
            }
            catch (Exception er)
            {
                log.Error("ListPendingRequestsWithState()", er);
            }

            return result;
        }

        public bool IsRequestGeneratedActive(long Id_RequestGenerated)
        {
            return this._estadoSolicitudesGeneradas.SolicitudGenerada_IsActive(Id_RequestGenerated);
        }
       
        #endregion

        #region Cambio de estado de una notificación generada (recepción cliente, visualización, etc)
        public long MarkActionStateAs(long Id_RequestGenerated, Estado_Solicitud state, int? id_Usuario, int? id_Device)
        {
            return this._solicitudesGeneradas.MarkAs(Id_RequestGenerated, state, id_Usuario, id_Device);
        }

        public void MarkActionStateAs_Async(long Id_NotificationGenerated, Estado_Solicitud state, int? id_Usuario, int? id_Device)
        {
            this._solicitudesGeneradas.MarkAs_Async(Id_NotificationGenerated, state, id_Usuario, id_Device);
        }

        public void MarkAllAs_Async(long[] Ids_NotificationGenerated, Estado_Solicitud state, int? id_Usuario,  int? id_Device)
        {
            this._solicitudesGeneradas.MarkAllAs_Async(Ids_NotificationGenerated, state, id_Usuario, id_Device);
        }
        #endregion

        #region Guardar el estado de las variables en memoria durante los reinicios
        /// <summary>
        /// Almacena las variables internas con los valores de señales y notificaciones
        /// </summary>
        public void PersistirVariables()
        {
            try
            {
                log.Debug("PersistirVariables(). Almacenando los valores de las variables relativas al RequestMotor");

                // Memoria
                log.Debug("Serializando valores de las señales");

                // Configuramos los saltos de línea
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = true;
                //settings.IndentChars = "\t";

                string rutaAplicacion = AppDomain.CurrentDomain.BaseDirectory;

                var serializer = new System.Xml.Serialization.XmlSerializer(this._valoresMemoria.GetType());
                using (var writer = System.Xml.XmlWriter.Create(
                    string.Format(@"{0}temp\memory.xml", rutaAplicacion),
                    settings))
                {
                    serializer.Serialize(writer, this._valoresMemoria);
                }
                // Notificaciones generadas
                log.Debug("Serializando diccionario de solicitudes");
                var serializer2 = new System.Xml.Serialization.XmlSerializer(this._solicitudesGeneradas.GetType());
                using (var writer = System.Xml.XmlWriter.Create(
                    string.Format(@"{0}temp\colaSolicitudes.xml", rutaAplicacion),
                    settings))
                {
                    serializer2.Serialize(writer, this._solicitudesGeneradas);
                }

                // Reglas activas
                log.Debug("Serializando estado de las reglas");
                var serializer3 = new System.Xml.Serialization.XmlSerializer(this._estadoSolicitudesGeneradas.GetType());
                using (var writer = System.Xml.XmlWriter.Create(
                    string.Format(@"{0}temp\solicitudesActivas.xml", rutaAplicacion),
                    settings))
                {
                    serializer3.Serialize(writer, this._estadoSolicitudesGeneradas);
                }
            }
            catch (Exception er)
            {
                log.Error("PersistirVariables()", er);
            }
        }

        /// <summary>
        /// Carga el último estado de las variables al cerrar la aplicación
        /// </summary>
        public void RecuperarVariables()
        {
            try
            {
                // Valores en memoria
                RecuperaVariablesMemoria();

                // Notificaciones generadas
                RecuperaVariablesSolicitudes();

                // Estado de las solicituedes (solicitudes generadas activas)
                RecuperaVariablesEstadoSolicitudes();

            }
            catch (Exception er)
            {
                log.Error("RecuperarVariables()", er);
            }
        }

        private void RecuperaVariablesMemoria()
        {
            try
            {
                string rutaAplicacion = AppDomain.CurrentDomain.BaseDirectory;
                string ruta = string.Format(@"{0}temp\memory.xml", rutaAplicacion);

                if (System.IO.File.Exists(ruta))
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MemoryValues));
                    using (var reader = XmlReader.Create(ruta))
                    {
                        MemoryValues memory = (MemoryValues)serializer.Deserialize(reader);
                        this._valoresMemoria.LoadValues(memory.GetAll());
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("RecuperaVariablesMemoria()", er);
            }
        }

        private void RecuperaVariablesSolicitudes()
        {
            try
            {
                string rutaAplicacion = AppDomain.CurrentDomain.BaseDirectory;
                string ruta = string.Format(@"{0}temp\colaSolicitudes.xml", rutaAplicacion);

                if (System.IO.File.Exists(ruta))
                {
                    var serializer2 = new System.Xml.Serialization.XmlSerializer(typeof(PendingRequests));
                    using (var reader = System.Xml.XmlReader.Create(ruta))
                    {

                        // Carga las notificaciones pendientes en el cierre anterior
                        PendingRequests solicitudesPendientes = (PendingRequests)serializer2.Deserialize(reader);

                        // Agrega al objeto las notificaciones pendientes (filtrando las finalizadas por TTL)
                        this._solicitudesGeneradas.LoadValues(solicitudesPendientes.GetAll());
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("RecuuperaVariablesNotificaciones()", er);
            }
        }

        private void RecuperaVariablesEstadoSolicitudes()
        {
            try
            {
                string rutaAplicacion = AppDomain.CurrentDomain.BaseDirectory;
                string ruta = string.Format(@"{0}temp\solicitudesActivas.xml", rutaAplicacion);

                if (System.IO.File.Exists(ruta))
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(DictionaryActiveRequests));
                    using (var reader = System.Xml.XmlReader.Create(ruta))
                    {
                        // Carga las notificaciones pendientes en el cierre anterior
                        DictionaryActiveRequests solicitudesPendientes = (DictionaryActiveRequests)serializer.Deserialize(reader);

                        // Agrega al objeto los estados de notificaciones activas
                        this._estadoSolicitudesGeneradas.LoadValues(solicitudesPendientes.GetAll());
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("RecuperaVariablesEstadoReglas()", er);
            }
        }

        #endregion


        #region Evento de caducidad de la solicitud
        public event RequestExpiredEventHandler RequestExpired;

        public delegate void RequestExpiredEventHandler(object sender, RequestExpiredEventArgs e);

        public class RequestExpiredEventArgs : EventArgs
        {
           
            public long? Id_Request { get; set; }
            public long? Id_RequestGenerated { get; set; }
        }

        public void OnRequestExpired(object sender, RequestExpiredEventArgs e)
        {
            RequestExpiredEventHandler handler = RequestExpired;
            if (handler != null)
                handler(sender, e);
        }
        #endregion

        #region Evaluación cuando llegan nuevos datos        
        /// <summary>
        /// Método al que se llama cada vez que llegan valores al dataprovider para evaluar las reglas aqui cargadas
        /// </summary>
        /// <param name="values"></param>
        public void EvaluateData(Memory.Common.TagValue value)
        {
            try
            {
                Dictionary<TagType, TagValue> memoriesValuesForPrensa = new Dictionary<TagType, TagValue>();
                foreach (var item in Enum.GetNames(typeof(TagType)))
                {
                  
                 //probamos git
                }


            }
            catch (Exception er)
            {
                log.Error("EvaluateData()", er);
            }
        }

      
        #endregion



        #region Evento de cumplimiento de la regla

        public event RequestStateChangedEventHandler RequestStateChanged;

        public delegate void RequestStateChangedEventHandler(object sender, RequestStateChangedEventArgs e);

        public class RequestStateChangedEventArgs : EventArgs
        {
            public int Id_Request { get; set; }
            public Estado_Solicitud estado { get; set; }
        }

        private void OnRequestStateChanged(object sender, RequestStateChangedEventArgs e)
        {
            RequestStateChangedEventHandler handler = RequestStateChanged;
            if (handler != null)
                handler(sender, e);
        }

        #endregion

        #region Gestion del motor de solicitudes
        /// <summary>
        /// Indica si el motor de solicitudes está activo 
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            bool sw = false;

            try
            {
                // Realente lo que se detiene es la adquisición de datos de los proveedores
                // al no haber cambios no hay reglas
                //if (this._proveedorDatos != null)
                //    sw = this._proveedorDatos.IsActive();
            }
            catch (Exception er)
            {
                log.Error("IsActive()", er);
            }

            return sw;
        }
        #endregion

        #region Tags
        /// <summary>
        /// Devuelve el valor de un tag buscandolo en los proveedores de datos
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public object ReadValue(string tag)
        {
            object result = null;

            try
            {
                //// Internamente se puede trabajar con arrays de tags,
                //// externamente no se utilizan estas peticiones, y de momento no es necesario
                //// su uso, en caso necesario, exponer el método para mejorar el rendimiento
                //// reduciendo las peticiones                
                //string[] tags = new string[] { tag };
                //var r = this._proveedorDatos.ReadValue(tags);

                //if (r != null && r.Values.Count > 0)
                //{
                //    result = r.Values[0];
                //}
            }
            catch (Exception er)
            {
                log.Error("ReadValue()", er);
            }
            return result;
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
                    // Guardamos el estado de las variables al cerrar                    
                    this.PersistirVariables();

                    // Llamamos al Dispose de todos los RECURSOS MANEJADOS.
                    //this._proveedoresDatos.Dispose();
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
        ~RequestMotor()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
