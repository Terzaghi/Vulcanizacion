
using LoggerManager;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Common.XML_Configuration
{
    /// <summary>
    /// Gestiona la configuración de las librerías a utilizar en la aplicación
    /// </summary>
    public class LibraryConfigurationXML
    {
        ILogger log =LogFactory.GetLogger(typeof(LibraryConfigurationXML));

        private string _ruta = string.Empty;

        /// <summary>
        /// El constructor requiere el nombre del archivo XML a cargar (por defecto DataProvidersConfiguration.xml)
        /// </summary>
        /// <param name="nombreXmlConfiguracion"></param>
        public LibraryConfigurationXML(string nombreXmlConfiguracion)
        {
            string rutaFisica = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string rutaCompleta = string.Format(@"{0}\{1}", rutaFisica, nombreXmlConfiguracion);

            this._ruta = rutaCompleta;
        }

        /// <summary>
        /// Almacena un objeto con la configuración del sistema en la ruta pasada al constructor
        /// </summary>
        /// <param name="datos"></param>
        /// <returns></returns>
        public bool Guardar<T>(object datos)
        {
            bool sw = false;

            try
            {
                if (File.Exists(this._ruta))
                {
                    // Guarda la configuración en un XML
                    XmlSerializer x = new XmlSerializer(typeof(T));

                    using (TextWriter writer = new StreamWriter(this._ruta))
                    {
                        x.Serialize(writer, datos);
                    }

                    sw = true;
                }
                else
                {
                    log.Warning("Guardar(). Compruebe que existe el archivo de configuración: {0}", this._ruta);
                }
            }
            catch (Exception er)
            {
                log.Error("Guardar()", er);
            }
            return sw;
        }

        /// <summary>
        /// Carga un objeto con la configuración del sistema en la ruta pasada al constructor
        /// </summary>
        /// <returns></returns>
        public object Leer<T>()
        {
            object datos = null;

            try
            {
                // Carga la configuración de un XML
                if (File.Exists(this._ruta))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));

                    using (FileStream fs = new FileStream(this._ruta, FileMode.Open))
                    {
                        datos = (T)serializer.Deserialize(fs);
                    }
                }
                else
                {
                    log.Warning("Leer(). Compruebe que existe el archivo de configuración: {0}", this._ruta);
                }
            }
            catch (Exception er)
            {
                datos = null;
                log.Error("Leer()", er);
            }

            return datos;
        }
    }
}
