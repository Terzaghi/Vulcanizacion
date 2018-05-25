using Common.XML_Configuration;
using LoggerManager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.TManager.Utils
{
    public class Persistence //Necesito que ésta clase sea pública para que pueda ser utilizada por el proyecto Common.XML_Configuration
    {
        #region Atributos

        ILogger log = LogFactory.GetLogger(typeof(Persistence));

        private static Persistence _instance;

        private DateTime _lastCheckDate;

        private string _checkDatePath;

        #endregion

        #region Propiedades

        public static Persistence GetInstance
        {
            get { return _instance ?? (_instance = new Persistence()); }
        }

        public DateTime LastCheckDate
        {
            get
            {
                return _lastCheckDate;
            }
        }

        #endregion
        
        private Persistence()
        {
            if (InicializaPersistencia())
                _lastCheckDate = Cargar_FechaComprobacion();
            else
                _lastCheckDate = DateTime.MinValue;
        }

        #region Interfaz Publica

        public void SetCheckDate(DateTime fecha)
        {
            if (fecha > _lastCheckDate)
            {
                _lastCheckDate = fecha;

                Guardar_FechaComprobacion(new XML_CheckDate
                {
                    LastCheckDate = fecha.ToString("dd/MM/yyyy HH:mm:ss")
                });
            }
        }

        #endregion

        #region Interfaz Privada

        private bool InicializaPersistencia()
        {
            var sw = false;

            try
            {
                _checkDatePath = @"XML\XML_ProviderLastCheckDate.xml";

                if (ConfigurationManager.AppSettings["provider_lastcheckdate_path"] != null)
                {
                    _checkDatePath = ConfigurationManager.AppSettings["provider_lastcheckdate_path"].Trim();

                    log.Debug("Se configura la ruta del xml de última fecha de comprobación: {0}", _checkDatePath);
                }
                else
                {
                    log.Warning("No se encuentra la ruta del xml de la última fecha de comprobación del proveedor de T-Manager, se toma la configuración por defecto ({0})", _checkDatePath);
                }

                sw = true;
            }
            catch (Exception ex)
            {
                log.Error("InicializaPersistencia. ", ex);
            }

            return sw;
        }

        private DateTime Cargar_FechaComprobacion()
        {
            DateTime result = DateTime.MinValue;

            try
            {
                log.Debug("CargarPersistencia_FechaComprobacion. Solicitando configuración");

                LibraryConfigurationXML config = new LibraryConfigurationXML(_checkDatePath);
                var configData = (XML_CheckDate)config.Leer<XML_CheckDate>();

                if (configData != null && !string.IsNullOrEmpty(configData.LastCheckDate))
                {
                    log.Debug("Configuración obtenida, fecha obtenida: {0}", configData.LastCheckDate);

                    if (!DateTime.TryParseExact(configData.LastCheckDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                        log.Warning("Cargar_FechaComprobacion. Se ha producido un error al convertir la fecha de comprobación");
                }
                else
                    log.Warning("No se ha obtenido la configuración de la última fecha comprobada");
            }
            catch (Exception ex)
            {
                log.Error("CargarPersistencia_FechaComprobacion. ", ex);
            }

            return result;
        }

        private void Guardar_FechaComprobacion(XML_CheckDate fecha)
        {
            try
            {
                LibraryConfigurationXML xml = new LibraryConfigurationXML(_checkDatePath);
                xml.Guardar<XML_CheckDate>(fecha);
            }
            catch (Exception ex)
            {
                log.Error("Guardar_FechaComprobacion. ", ex);
            }
        }

        #endregion

        public class XML_CheckDate
        {
            public string LastCheckDate { get; set; }
        }
    }
}
