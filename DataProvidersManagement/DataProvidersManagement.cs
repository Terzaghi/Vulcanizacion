using DataProvider.Interfaces;
using LoggerManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvidersManagement
{
    public class DataProvidersManagement:IDisposable
    {
        ILogger log = LogFactory.GetLogger(typeof(DataProvidersManagement));
 
        private IDataProvider[] _proveedoresDatos;
        private bool _activo;

        public DataProvidersManagement(params IDataProvider[] lstProveedores)
        {
            this._proveedoresDatos = lstProveedores;
        }

        /// <summary>
        /// Activa la adquisición de datos de todos los proveedores
        /// </summary>
        public void Start()
        {
            try
            {
                foreach (var prov in this._proveedoresDatos)
                {
                    log.Information("Iniciando el proveedor: {0}", prov.GetType().FullName);
                    prov.Start();
                }
                this._activo = true;
            }
            catch (Exception er)
            {
                log.Error("Start()", er);
            }
        }
        /// <summary>
        /// Detiene la adquisición de datos de todos los proveedores
        /// </summary>
        public void Stop()
        {
            try
            {
                foreach (var prov in this._proveedoresDatos)
                {
                    log.Information("Deteniendo el proveedor: {0}", prov.GetType().FullName);
                    prov.Stop();
                }
                this._activo = false;
            }
            catch (Exception er)
            {
                log.Error("Stop()", er);
            }
        }

        /// <summary>
        /// Indica si los proveedores están activos o no
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return this._activo;
        }

        private void Proveedor_DataChanged(object sender, DataReceivedEventArgs e)
        {
            // TODO: Si la frecuencia de datos es muy alta, podríamos configurar aqui una frecuencia mínima para no saturar el sistema

            // Relanzamos el evento con los datos que nos acaban de llegar
            OnDataReceived(sender, e);
        }
        
        /// <summary>
        /// Evento que se lanza con la llegada de nuevos valores de las señales (tags)
        /// </summary>
        public event DataChangeEventHandler DataChanged;
        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            DataChangeEventHandler dataReceived = DataChanged;
            if (dataReceived != null)
                dataReceived(e);
        }


        #region Implementación interfaz IDisposeable
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
                    // Llamamos al Dispose de todos los RECURSOS MANEJADOS.
                    // Livera todos los proveedores
                    LiberarProveedores();

                }

                // Aqui finalizamos correctamente los RECURSOS NO MANEJADOS
                // ...

            }
            this.disposed = true;
        }

        /// <summary>
        /// Destructor de la instancia
        /// </summary>
        ~DataProvidersManagement()
        {
            this.Dispose(false);
        }

        private void LiberarProveedores()
        {
            try
            {
                // Livera todos los proveedores
                if (this._proveedoresDatos != null)
                {
                    foreach (var proveedor in this._proveedoresDatos)
                    {
                        proveedor.Dispose();
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("LiberarProveedores()", er);
            }
        }
        #endregion

    }
}
