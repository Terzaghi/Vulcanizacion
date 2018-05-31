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


namespace RequestManager
{
    public class PendingRequests : IXmlSerializable
    {
        ILogger log = LogFactory.GetLogger(typeof(PendingRequests));

        private int _numeroMaximoSolicitudes = 1000;       // Maximo de solicitudes en memoria por defecto
        private double _ttlDefault = 604800;                             // TTL por defecto (en segundos).   Defecto 604800 (una semana)        

        private SortedDictionary<long, PendingRequestLogic> _dicRequest;
        private RequestMotor _refMotorSolicitudes = null;
        private PrensaCatalog.Prensas _catalogPrensas;


        public PendingRequests(ref PrensaCatalog.Prensas catalogPrensas)
        {
            this._catalogPrensas = catalogPrensas;
        }
        #region Timer comprobaciones y limpieza según TTL
        private System.Timers.Timer _tmr = null;

        private void InicializaHiloComprobacionesTTL()
        {
            log.Debug("Iniciado el hilo de limpieza de notificaciones");
            _tmr = new System.Timers.Timer();
            _tmr.Interval = 60000;              // Intervalo de comprobación de los TTL de las notificaciones
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

                
                List<long> lstElementosEliminar = new List<long>();

                PendingRequestLogic solicitud;

                lock (this._dicRequest)
                {
                    foreach (KeyValuePair<long, PendingRequestLogic> entry in this._dicRequest)
                    {
                        
                        solicitud = entry.Value;

                        bool eliminar = false;

                        if (solicitud != null)
                        {
                            double ttl = solicitud.GetTTL;
                            // Si la solicitud no tiene configurado un tiempo máximo, le ponemos el del sistema
                            if (ttl == 0 && this._ttlDefault > 0)
                            {
                                ttl = this._ttlDefault;
                            }

                            // Comprobamos si el ttl configurado por la solicitud sobrepasa al nuestro, para ponerlo en tal caso como máximo de la notificación
                            if (ttl > this._ttlDefault)
                                ttl = this._ttlDefault;

                            // Validación por tiempo de vida (si están a 0 son de tiempo ilimitado)
                            if (ttl > 0)
                            {

                                if (solicitud.GetDateGeneration != null)
                                {
                                    TimeSpan ts = DateTime.Now - solicitud.GetDateGeneration;

                                    
                                    if (ts.TotalMinutes > ttl)
                                    {
                                        log.Debug("El tiempo de vida de la solicitud ha finalizado. Marcada para su eliminación... (Id_Request: {0})", entry.Key);
                                        eliminar = true;
                                    }
                                }
                                else
                                {
                                    log.Debug("La solicitud no tiene configurada una hora de creación. Marcada para su eliminación... (Id_Request: {0})", entry.Key);
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
                            lstElementosEliminar.Add(entry.Key);
                        }
                    }
                }

                // Eliminamos los objetos
                if (lstElementosEliminar.Count > 0)
                {
                    log.Debug("Se limpiarán {0} solicitudes que han superado TTL de un total de {1}", lstElementosEliminar.Count, this._dicRequest.Count);
                    lock (this._dicRequest)
                    {
                        long id_request = -1;

                        foreach (var elemento in lstElementosEliminar)
                        {
                            id_request = elemento;

                            // Comprobamos primero, antes de eliminarla si está activa                            
                            bool estaAceptada = false;
                            estaAceptada = (_dicRequest[elemento].GetRequestState == Estado_Solicitud.Aceptada ? true : false);
                            //estaActiva = this._refMotorReglas.IsNotificationActive(id_notif);

                            if (!estaAceptada)
                            {
                                // Elimina de la cola de pendientes                               
                                this._dicRequest.Remove(elemento);
                            }
                            else
                            {
                                log.Debug("No se eliminará la solicitud de las colas de memoria ya que permanece activa (Id_Request: {0})", elemento);
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

        #region "Agregar Solicitud"
        public long Add(int Id_Prensa)
        {
            long Id_Request= -1;
            try
            {
                log.Debug("MemoryGeneratedRequest. Add(). ");
                //Add Request
                Solicitudes model = new Solicitudes();
                Model.BL.DTO.Solicitud solicitud = new Model.BL.DTO.Solicitud();
                solicitud.Id_Prensa = Id_Prensa;
                solicitud.Fecha_Generacion = DateTime.Now;
                Id_Request = model.Agregar(solicitud);

                PendingRequestLogic pendingRequestLogic = new PendingRequestLogic();
                pendingRequestLogic.SetConfiguration(Estado_Solicitud.Pendiente, null, null);

                _dicRequest.Add(Id_Request, pendingRequestLogic);

                //Add to History
                Model.BL.Historico_Solicitud modelHistoric = new Model.BL.Historico_Solicitud();
                Model.BL.DTO.Historico_Solicitud historico = new Model.BL.DTO.Historico_Solicitud();
                historico.Fecha = solicitud.Fecha_Generacion;
                historico.Id_Solicitud = Id_Request;
                historico.Estado = Estado_Solicitud.Pendiente;

                modelHistoric.Agregar(historico);
            }
            catch (Exception er)
            {
                log.Error("Add()", er);
            }
            return Id_Request;
        }
        #endregion
        public bool Remove(int Id_Prensa)
        {

            bool sw = false;
            try
            {
                var request = _dicRequest.SingleOrDefault(x => x.Value.GetIdPrensa == Id_Prensa);
                if (request.Equals(default(KeyValuePair<long, PendingRequestLogic>)))
                {
                    log.Debug("Solicitud encontrada, se va a eliminar (Id_Request: {0})", request.Value.GetIdRequest);
                    _dicRequest.Remove(request.Value.GetIdRequest);

                    sw = true;
                }
                else
                {
                    log.Debug("La solicitud de prensa intenta eliminar no está activa en memoria (Id_Prensa: {0})", Id_Prensa);
                }
            }
            catch (Exception er)
            {
                log.Error("Eliminar()", er);
            }

            return sw;
        }

        public bool isBarcodeValid(string barcode, int id_prensa)
        {
            bool sw = false;
            try
            {

            }catch(Exception ex)
            {
                log.Error("isBarcodeValid()", ex);
            }
            return sw;
        }
        public Tipo_Contramedidas getContramedidas(int id_prensa)
        {
            try
            {
                return Tipo_Contramedidas.Pinchar;

            }catch(Exception ex)
            {
                log.Error("getContramedidas", ex);
            }
            return Tipo_Contramedidas.Pinchar;
        }

        #region Cambiar de estado una solicitud

        public void MarkAs_Async(long Id_Request, Estado_Solicitud state, int? id_Usuario,  int? id_Device)
        {
            try
            {
                Task.Run(() =>
                {
                    MarkAs(Id_Request, state, id_Usuario, id_Device);
                });
            }
            catch (Exception er)
            {
                log.Error("MarkAs_Async()", er);
            }
        }

       public long MarkAs(long Id_Request, Estado_Solicitud state, int? id_Usuario,int? id_Device)
        {
            long idRequestCambioEstado = -1;

            try
            {
                // Configuramos el estado de las notificaciones en memoria
                log.Debug("MarkAs. Guardando el estado en memoria");
                PendingRequestLogic confRequest = null;

                if (_dicRequest.TryGetValue(Id_Request, out confRequest))
                {
                    // Obtenemos el estado actual
                    Estado_Solicitud? estadoSolicitud = confRequest.GetRequestState;

                    if (estadoSolicitud != null && state < estadoSolicitud)
                    {
                        //Add to History
                        Model.BL.Historico_Solicitud modelHistoric = new Model.BL.Historico_Solicitud();
                        Model.BL.DTO.Historico_Solicitud historico = new Model.BL.DTO.Historico_Solicitud();
                        historico.Fecha = DateTime.Now;
                        historico.Id_Solicitud = Id_Request;
                        historico.Estado = state;
                        idRequestCambioEstado=modelHistoric.Agregar(historico);
                        
                        //Change State
                        confRequest.SetConfiguration(state, id_Usuario, id_Device);

                    }
                }             
            }
            catch (Exception er)
            {
                log.Error("MarkAs()", er);
            }

            return idRequestCambioEstado;
        }



    
        #endregion

        public PendingRequestLogic GetNextRequest(int id_Usuario)
        {
            PendingRequestLogic request = new PendingRequestLogic();
            try
            {
                //Get Prensas for user
                List<int> prensasUsuario = _catalogPrensas.GetUserPrensas(id_Usuario);
                List<PrensaCatalog.DTO.Prensa> listCaracteristicas = new List<PrensaCatalog.DTO.Prensa>();
                if (prensasUsuario.Count > 0)
                {
                    foreach(int prensaId in prensasUsuario)
                    {
                        listCaracteristicas.Add(GetCaracteristicasPrensa(prensaId));
                    }
                    //Agrupamos y cogemos el grupo con mayor prioridad
                    var groupsPrioridad = listCaracteristicas.OrderByDescending(x => x.prioridad).GroupBy(x => x.prioridad).First();
                    //Convertimos en lista con IDs de prensa con mayor prioridad
                    var lstMaxPrioridad = groupsPrioridad.Select(x => x).ToList();
                    var lstIdsMaxPrioridad = lstMaxPrioridad.Select(x => x.prensa.Id).ToList();
                    //Buscamos las solicitudes asociadas a las prensas
                    var matches = _dicRequest.Where(kvp => lstIdsMaxPrioridad.Contains(kvp.Value.GetIdPrensa)).Select(x => x);
                    //Elegimos la solicitud más antigua
                    var match= from x in matches where x.Value.GetDateGeneration == matches.Min(v => v.Value.GetDateGeneration) select x.Value;
                    if (match.Count()>0)
                    {
                        request = match.ElementAt(0);
                    }
                }    
              
            }catch(Exception ex)
            {
                log.Error("GetNextRequest()", ex);
            }

            return request;
        }

        private PrensaCatalog.DTO.Prensa GetCaracteristicasPrensa(int idPrensa) {
            PrensaCatalog.DTO.Prensa prensa = null;
            if(_catalogPrensas._caracteristicasPrensa.ContainsKey(idPrensa)){
                prensa = _catalogPrensas._caracteristicasPrensa[idPrensa];
            }
            return prensa;
        }

        #region IXmlSerializable


        public XmlSchema GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            try
            {
                // Iniciamos la variable
                this._dicRequest.Clear();

                reader.Read();
                reader.ReadStartElement("dictionary");
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    string strJson = reader.ReadElementString("item");

                    RequestManager.PendingRequestLogic result = Newtonsoft.Json.JsonConvert.DeserializeObject<RequestManager.PendingRequestLogic>(strJson, new Newtonsoft.Json.JsonSerializerSettings
                    {
                        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
                    });

                    
                    reader.MoveToContent();

                    // Agregamos el valor
                    this._dicRequest.Add((long)result.GetIdRequest, result);
                }
                reader.ReadEndElement();
            }
            catch (Exception er)
            {
                log.Error("ReadXml()", er);
            }
        }

   

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            try
            {
                // Serializamos el diccionario de valores (propiedad privada)      
                writer.WriteStartElement("dictionary");
                foreach (KeyValuePair<long, PendingRequestLogic> entry in this._dicRequest)
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
            catch (Exception er)
            {
                log.Error("WriteXml()", er);
            }
        }


        #endregion
        #region Restaurar clase tras reinicio

        public SortedDictionary<long, PendingRequestLogic> GetAll()
        {
            return this._dicRequest;
        }

        /// <summary>
        /// Restaura un diccionario completo de notificaciones a memoria
        /// </summary>
        /// <param name="notifications"></param>
        /// <returns></returns>
        public int LoadValues(SortedDictionary<long, PendingRequestLogic> solicitudes)
        {
            try
            {
                log.Debug("Agregando a las estructuras de memoria las solicitudes pendientes almacenadas en temp");
                this._dicRequest = solicitudes;

                // Una vez cargadas limpia las que ya no valgan por TTL
                LimpiarCaducadasTTL();
            }
            catch (Exception er)
            {
                log.Error("LoadValues()", er);
            }

            return this._dicRequest.Count;
        }
        #endregion
    }
}
