using LoggerManager;
using RequestManager.DTO;
using RequestManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RequestManager
{
    [DataContract]
    public class DictionaryActiveRequests
    {        
        ILogger log = LogFactory.GetLogger(typeof(DictionaryActiveRequests));

        [DataMember(Name = "SolicitudesGeneradasSolicitudes")]
        public SerializableDictionary<long, RequestGenerated> _solicitudesGeneradasEstados;  
        [DataMember(Name = "SolicitudesSolicitudesGeneradas")]
        public SerializableDictionary<int, RequestGenerated> _solicitudesGeneradasSolicitudes;
        public DictionaryActiveRequests()
        {
            this._solicitudesGeneradasEstados = new SerializableDictionary<long, RequestGenerated>();
            this._solicitudesGeneradasSolicitudes = new SerializableDictionary<int, RequestGenerated>();
        }

        #region Gestión
        public void SolicitudGenerada_Desactivar(int Id_Request)
        {
            try
            {
                RequestGenerated solicitudGenerada;

                // Comprobamos si está presente la solicitud 
                lock (this._solicitudesGeneradasSolicitudes)
                {
                    if (this._solicitudesGeneradasSolicitudes.ContainsKey(Id_Request))
                    {
                        if (this._solicitudesGeneradasSolicitudes.TryGetValue(Id_Request, out solicitudGenerada))
                        {
                            // Pasamos a quitarla de las activas                            
                            this._solicitudesGeneradasEstados.Remove(solicitudGenerada.Id_Request_Generated);
                        }
                        // Y también quitamos la regla que la lanzó
                        this._solicitudesGeneradasSolicitudes.Remove(Id_Request);
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("SolicitudGenerada_Desactivar()", er);
            }
        }

        public void SolicitudGenerada_Activar(int Id_Request, RequestGenerated RequestGenerated)
        {
            try
            {
                if (this._solicitudesGeneradasSolicitudes.ContainsKey(Id_Request))
                {
                    // La solicitud ya estaba activa, la quitamos
                    SolicitudGenerada_Desactivar(Id_Request);
                }

                if (Id_Request > 0)
                
                {
                    // Agregamos la solicitud generada a la estructura de activas
                    lock (this._solicitudesGeneradasSolicitudes)
                    {
                        this._solicitudesGeneradasSolicitudes.Add(Id_Request, RequestGenerated);
                        this._solicitudesGeneradasEstados.Add(RequestGenerated.Id_Request_Generated, RequestGenerated);
                    }
                }
                else
                {
                    
                }
            }
            catch (Exception er)
            {
                log.Error("NotificacionGenerada_Activar", er);
            }
        }
    
           
        #endregion

        #region Consulta de datos
        public bool SolicitudGenerada_IsActive(long Id_RequestGenerada)
        {
            bool sw = false;

            if (this._solicitudesGeneradasEstados.ContainsKey(Id_RequestGenerada))
            {
                sw = true;
            }

            return sw;
        }
        #endregion

        #region Restaurar clase tras reinicio
        
        public Tuple<SerializableDictionary<long, RequestGenerated>, SerializableDictionary<int, RequestGenerated>> GetAll()
        {
            Tuple<SerializableDictionary<long, RequestGenerated>, SerializableDictionary<int, RequestGenerated>> result = new Tuple<SerializableDictionary<long, RequestGenerated>, SerializableDictionary<int, RequestGenerated>>(this._solicitudesGeneradasEstados, this._solicitudesGeneradasSolicitudes);
            return result;
        }
        
        /// <summary>
        /// Restaura los diccionarios de notificaciones y reglas en memoria
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int LoadValues(Tuple<SerializableDictionary<long, RequestGenerated>, SerializableDictionary<int, RequestGenerated>> obj)
        {
            try
            {
                log.Debug("Agregando a las estructuras de memoria las notificaciones activas");
                this._solicitudesGeneradasEstados = obj.Item1;
                this._solicitudesGeneradasSolicitudes = obj.Item2;

            }
            catch (Exception er)
            {
                log.Error("LoadValues()", er);
            }


            return this._solicitudesGeneradasEstados.Count;
        }
        #endregion

    }
}
