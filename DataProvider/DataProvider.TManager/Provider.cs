
using DataProvider.Interfaces;
using DataProvider.TManager.Utils;
using LoggerManager;
using Memory.Common;
using Memory.Common.Utils;
using Model.BL.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using ValuesMemory;

namespace DataProvider.TManager
{
    public class Provider : IDataProvider,IDisposable
    {
        ILogger log = LogFactory.GetLogger(typeof(Provider));

        #region Atributos

        private const string ConnectionStringPrensas = "TManagerDB";

        private MemoryValues _valoresMemoria = null;

        #region bucle asíncrono de comprobación

        private bool _alive = true; //Bucle activo

        private double _lapse = 2000; // lapso de tiempo de comprobación de cambios en base de datos

        private delegate void AsyncMethodCaller();

        private AsyncMethodCaller async_caller;

        #endregion

        #endregion

        #region Evento 

        /// <summary>
        /// Evento que se lanza con la llegada de nuevos datos 
        /// </summary>
        public event DataChangeEventHandler DataChanged;

        private void OnDataReceived(DataReceivedEventArgs e)
        {
            DataChanged?.Invoke(e);
        }

        #endregion

        #region Inicialización

        public Provider(ref MemoryValues valoresMemoria)
        {
            this._valoresMemoria = valoresMemoria;

            Inicializa();
        }
        
        private void Inicializa()
        {
            try
            {
                async_caller = new AsyncMethodCaller(Async_PrensasDatos);

                if (ConfigurationManager.AppSettings["database_check_interval"] != null)
                {
                    double.TryParse(ConfigurationManager.AppSettings["database_check_interval"], out _lapse);

                    log.Debug("Se configura un lapso de {0} milisegundos entre comprobaciones", _lapse);
                }
            }
            catch (Exception ex)
            {
                log.Error("Inicializa()", ex);
            }
        }

        #endregion

        #region Interfaz IDataProvider

        public bool Start()
        {
            bool sw = false;

            try
            {
                if (!_alive) _alive = true;

                IAsyncResult result_prensas_datos = async_caller.BeginInvoke(null, null);

                sw = true;
            }
            catch (Exception ex)
            {
                log.Error("Start. ", ex);
            }

            return sw;
        }
        public bool Stop()
        {
            bool sw = false;

            try
            {
                if (_alive) _alive = false;
                sw = true;
            }
            catch (Exception ex)
            {
                log.Error("Start. ", ex);
            }

            return sw;
        }
        #endregion

        #region Bucle de comprobación
        
        private void Async_PrensasDatos()
        {
            bool createNew;

            log.Debug("Se crea el bucle asíncrono de comprobación de prensas");

            EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, Guid.NewGuid().ToString(), out createNew);

            if (!createNew)
            {
                eventWaitHandle.Set(); //Inform other process to stop
            }

            do
            {
                var lastCheck = CheckPrensasDatos(Persistence.GetInstance.LastCheckDate);

                if (lastCheck.HasValue)
                {
                    Persistence.GetInstance.SetCheckDate(lastCheck.Value);
                }

                eventWaitHandle.WaitOne(TimeSpan.FromMilliseconds(_lapse));

            } while (_alive);
        }

        private DateTime? CheckPrensasDatos(DateTime lastCheckDate)
        {
            DateTime? date = null;

            try
            {
                Model.BL.PrensasDatos model = new Model.BL.PrensasDatos(ConnectionStringPrensas);

                List<PrensaDato> lstDatos = null;

                if (lastCheckDate > DateTime.MinValue) //Si es igual a MinValue no compruebo la fecha
                    lstDatos = model.ListarNuevosRegistros(lastCheckDate);
                else
                    log.Warning("CheckPrensasDatos. La fecha a partir de la cual comprobar los nuevos registros no ha sido establecida");

                if (lstDatos != null && lstDatos.Count > 0)
                {
                    log.Debug("CheckPrensasDatos. Se han obtenido {0} nuevos registros. Comprobando si se han producido cambios en los valores", lstDatos.Count);

                    date = lstDatos.Max(a => a.Fecha);

                    foreach (var registro in lstDatos)
                    {
                        CompruebaRegistro(registro);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("CheckPrensasDatos()", ex);
            }

            return date;
        }

        private void CompruebaRegistro(PrensaDato registro)
        {
            try
            {
                if (registro == null || registro.PrensaId <= 0)
                    return;

                if (_valoresMemoria == null)
                    _valoresMemoria = new MemoryValues();

                var valoresDB = Converter.ConvertToTagValueDictionary(registro);

                if (valoresDB != null && valoresDB.Count > 0)
                {
                    foreach (TagValue valorDB in valoresDB.Values)
                    {
                        if (!_valoresMemoria.ContainsKey(valorDB.ToKey()))
                        {
                            log.Debug("Nuevo tag añadido al diccionario de valores. Prensa: [{0}]. Tipo: [{1}]. Valor: [{2}]. Fecha: [{3}]",
                                valorDB.Id_Prensa, 
                                valorDB.Type.ToString(), 
                                !string.IsNullOrEmpty(valorDB.Value) ? valorDB.Value : "null", 
                                valorDB.Date.ToString("dd/MM/yyyy HH:mm:ss"));

                            _valoresMemoria.AddValue(valorDB.Id_Prensa, valorDB.Type, valorDB.Value, valorDB.Date);

                            //TODO: Nuevo tag añadido al diccionario de valores, ¿lanzar evento de cambio de valor?
                        }
                        else
                        {
                            var tagValue = _valoresMemoria.GetTagValue(valorDB.Id_Prensa, valorDB.Type);

                            if (tagValue == null)
                            {
                                log.Debug("Nuevo tag añadido al diccionario de valores. Prensa: [{0}]. Tipo: [{1}]. Valor: [{2}]. Fecha: [{3}]",
                                    valorDB.Id_Prensa,
                                    valorDB.Type.ToString(),
                                    !string.IsNullOrEmpty(valorDB.Value) ? valorDB.Value : "null",
                                    valorDB.Date.ToString("dd/MM/yyyy HH:mm:ss"));

                                _valoresMemoria.AddValue(valorDB.Id_Prensa, valorDB.Type, valorDB.Value, valorDB.Date);

                                //TODO: Nuevo tag añadido al diccionario de valores, ¿lanzar evento de cambio de valor?
                            }
                            else if (tagValue.Value.Equals(valorDB.Value))
                            {
                                //El valor sigue siendo el mismo, actualizo la fecha

                                tagValue.Date = valorDB.Date;
                            }
                            else
                            {
                                log.Debug("Nuevo valor para el tag [{1}] de la Prensa: [{0}]. Valor: [{2}]. Fecha: [{3}]",
                                    valorDB.Id_Prensa,
                                    valorDB.Type.ToString(),
                                    !string.IsNullOrEmpty(valorDB.Value) ? valorDB.Value : "null",
                                    valorDB.Date.ToString("dd/MM/yyyy HH:mm:ss"));

                                //Nuevo valor
                                tagValue.Date = valorDB.Date;
                                tagValue.Value = valorDB.Value;

                                OnDataReceived(new DataReceivedEventArgs
                                {
                                    Value = tagValue
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("CompruebaRegistro()", ex);
            }
        }

        #endregion


        #region Interfaz IDisposable

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
                    // Llamamos al Dispose de todos los RECURSOS MANEJADOS.
                    this.Stop();
                    //if (_timer != null)
                    //    _timer.Dispose();
                }

                // Aqui finalizamos correctamente los RECURSOS NO MANEJADOS
                // ...

            }
            this.disposed = true;
        }

        /// <summary>
        /// Destructor de la instancia
        /// </summary>
        ~Provider()
        {
            this.Dispose(false);
        }
        #endregion

    }
}
