using LoggerManager;
using Memory.Common;
using Model.BL.DTO.Enums;
using RequestManager.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ValuesMemory;

namespace RequestManager
{
    public class RequestMotor : IDisposable
    {
        ILogger log = LogFactory.GetLogger(typeof(RequestMotor));

        private MemoryValues _datosEnMemoria;
        private PrensaCatalog.Prensas _catalogPrensa;
        private DataProvidersManagement.DataProvidersManagement _proveedores;

        private PendingRequests _solicitudesGeneradas;
        public RequestMotor(ref MemoryValues datosEnMemoria, ref PrensaCatalog.Prensas prensaCatalog, ref DataProvidersManagement.DataProvidersManagement proveedores)
        {
            _datosEnMemoria = datosEnMemoria;
            _catalogPrensa = prensaCatalog;
            _proveedores = proveedores;
        }
        #region "Pooling Proveedores"
        public void InicializaTimerTags()
        {

        }


        #endregion

        /// <summary>
        /// Método al que se llama cada vez que llegan valores al dataprovider para evaluar las reglas aqui cargadas
        /// </summary>
        /// <param name="values"></param>
        public void EvaluateData(Memory.Common.TagValue value)
        {
            try
            {
                Dictionary<TagType, TagValue> memoriesValuesForPrensa = new Dictionary<TagType, TagValue>();
                RequestManager.Conditions.Conditions evalConditions = new RequestManager.Conditions.Conditions();
                //Recogemos el valor de cada uno de los tags relacionados con PrensaId
                foreach (var item in Enum.GetNames(typeof(TagType)))
                {
                    TagType tagType = (TagType)Enum.Parse(typeof(TagType), item);
                    var memoryValue = _datosEnMemoria.GetTagValue(value.Id_Prensa, tagType);
                    memoriesValuesForPrensa.Add(tagType, memoryValue);

                }
                //Evaluamos condiciones relacionadas con el tag que ha cambiado
               
                List<ICondition> conditionsEvalToApply = evalConditions.GetConditions(value.Type);
                KeyValuePair<long,PendingRequestLogic> solicitud = this._solicitudesGeneradas.GetAll().Where(x => x.Value.GetIdPrensa == value.Id_Prensa).SingleOrDefault();
                foreach (ICondition cond in conditionsEvalToApply)
                {
                    bool validation = cond.validateCondition(value, memoriesValuesForPrensa[value.Type]);
                    if (validation == true)
                    {
                        switch (cond.action)
                        {
                            case ActionForRequest.Delete:
                                this._solicitudesGeneradas.Remove(value.Id_Prensa);
                                break;
                            case ActionForRequest.Generated:
                                this._solicitudesGeneradas.Add(value.Id_Prensa);
                                break;
                        }
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("EvaluateData()", er);
            }
        }
     

        public PendingRequestLogic GetNextRequest(int id_User)
        {
            return this._solicitudesGeneradas.GetNextRequest(id_User);
        }
  
        public void MarkAs_Async(long Id_Request, Estado_Solicitud state, int? id_Usuario, int? id_Device)
        {
            this._solicitudesGeneradas.MarkAs_Async(Id_Request, state, id_Usuario,id_Device);
        }
     
        public void AddPrensa(int id_Prensa)
        {
            this._solicitudesGeneradas.Add(id_Prensa);
        }
        public void ModifyPrensa()
        {
            
        }
        public void RemovePrensa(int id_prensa)
        {
            this._solicitudesGeneradas.Remove(id_prensa);
        }
        public bool isBarcodeValid(string Barcode, int id_prensa)
        {
            return this._solicitudesGeneradas.isBarcodeValid(Barcode, id_prensa);
        }
        public Tipo_Contramedidas getContramedidas(int id_prensa)
        {
            return this._solicitudesGeneradas.getContramedidas(id_prensa);
        }
        
        #region Guardar el estado de las variables en memoria durante los reinicios
        /// <summary>
        /// Almacena las variables internas con los valores de señales y notificaciones
        /// </summary>
        public void PersistirVariables()
        {
            try
            {
                log.Debug("PersistirVariables(). Almacenando los valores de las variables relativas al RuleMotor");

                // Memoria
                log.Debug("Serializando valores de las señales");

                // Configuramos los saltos de línea
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = true;
                //settings.IndentChars = "\t";

                string rutaAplicacion = AppDomain.CurrentDomain.BaseDirectory;

                var serializer = new System.Xml.Serialization.XmlSerializer(this._datosEnMemoria.GetType());
                using (var writer = System.Xml.XmlWriter.Create(
                    string.Format(@"{0}temp\memory.xml", rutaAplicacion),
                    settings))
                {
                    serializer.Serialize(writer, this._datosEnMemoria);
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

            }
            catch (Exception er)
            {
                log.Error("RecuperarVariables()", er);
            }
        }

           private void RecuperaVariablesSolicitudes()
        {
            try
            {
                string rutaAplicacion = AppDomain.CurrentDomain.BaseDirectory;
                string ruta = string.Format(@"{0}temp\colaNotificaciones.xml", rutaAplicacion);

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
                log.Error("RecuuperaVariablesSolicitudes()", er);
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
                    using (var reader = System.Xml.XmlReader.Create(ruta))
                    {
                       MemoryValues memory = (MemoryValues)serializer.Deserialize(reader);
                        this._datosEnMemoria.LoadValues(memory.GetAll());
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("RecuperaVariablesMemoria()", er);
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
                    // Guardamos el estado de las variables al cerrar                    
                    this.PersistirVariables();

                    // Llamamos al Dispose de todos los RECURSOS MANEJADOS.
                    //this._proveedores.Dispose();
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
