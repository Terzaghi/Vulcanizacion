using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using System.Runtime.Caching;

namespace LoggerManager
{
    public class ELLogger: ILogger
    {
        private string nombreClave;
        private string clase;
        private enum nivelesLog { All, Trace, Debug, Information, Warning, Error, Critical, Off };
        private nivelesLog nivelLog = nivelesLog.All;
        private bool enabled = true;
        
        #region Constructores
        public ELLogger(string name)
        {
            nombreClave = name;
            clase = "[" + name + "]";
            this.Configurar();
        }

        public ELLogger(Type type)
        {
            nombreClave = type.FullName;
            clase = "[" + type.Name + "]";
            this.Configurar();
        }

        private void Configurar()
        {            
            //Sino tenemos configuración dejamos que de un error y se eleve.
            try
            {
                #region Método normal (ConfigurationSource va liberando mal recursos ocupando memoria)
                /*
                IConfigurationSource configurationSource = ConfigurationSourceFactory.Create();

                LoggingSettings configuracion;

                configuracion = Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.LoggingSettings.GetLoggingSettings(configurationSource);                

                //Se comprueba contra nulo por que en la previsualización de controles en tiempo de diseño el 
                //archivo de configuración es el del Visual Studio, y no incluye configuración de Logging
                if (configuracion == null || !configuracion.TracingEnabled)
                {
                    enabled = false;
                    nivelLog = nivelesLog.Off;
                }
                else
                {
                    enabled = true;
                    TraceSourceData categ;
                    //Buscamos si el nombre de la clase tiene una categoria especial
                    categ = configuracion.TraceSources.Get(nombreClave);
                    if (categ == null)
                    {
                        //Sino tiene una categoria especial cogemos la categoria por defecto
                        categ = configuracion.TraceSources.Get(configuracion.DefaultCategory);
                        nombreClave = configuracion.DefaultCategory;
                    }

                    if (categ != null)
                    {
                        switch (categ.DefaultLevel)
                        {
                            case System.Diagnostics.SourceLevels.Off:
                                nivelLog = nivelesLog.Off;
                                break;
                            case System.Diagnostics.SourceLevels.Critical:
                                nivelLog = nivelesLog.Critical;
                                break;
                            case System.Diagnostics.SourceLevels.Error:
                                nivelLog = nivelesLog.Error;
                                break;
                            case System.Diagnostics.SourceLevels.Warning:
                                nivelLog = nivelesLog.Warning;
                                break;
                            case System.Diagnostics.SourceLevels.Information:
                                nivelLog = nivelesLog.Information;
                                break;
                            case System.Diagnostics.SourceLevels.Verbose:
                                nivelLog = nivelesLog.Debug;
                                break;
                            case System.Diagnostics.SourceLevels.ActivityTracing:
                                nivelLog = nivelesLog.Trace;
                                break;
                            case System.Diagnostics.SourceLevels.All:
                                nivelLog = nivelesLog.All;
                                break;
                        }
                    }
                    else
                    {
                        //Sino existen ninguna categoria desactivamos el Log
                        nivelLog = nivelesLog.Off;
                    }
                }
                */
                #endregion


                #region Carga de configuración usando cachés 
                // Creamos el objeto para la gestión de la caché
                ObjectCache cache = MemoryCache.Default;
                var politicaDuracion = new CacheItemPolicy();


                IConfigurationSource configurationSource;
                string nombreCache = "configurationSource";
                if (cache.Contains(nombreCache))
                    configurationSource = (IConfigurationSource)cache.Get(nombreCache);
                else
                {
                    configurationSource = ConfigurationSourceFactory.Create();
                    cache.Add(nombreCache, configurationSource, politicaDuracion);
                }


                
                LoggingSettings configuracion;

                nombreCache = "configuracion";
                if (cache.Contains(nombreCache))
                    configuracion = (LoggingSettings)cache.Get(nombreCache);
                else
                {
                    configuracion = Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.LoggingSettings.GetLoggingSettings(configurationSource);
                    cache.Add(nombreCache, configuracion, politicaDuracion);
                }



                //Se comprueba contra nulo por que en la previsualización de controles en tiempo de diseño el 
                //archivo de configuración es el del Visual Studio, y no incluye configuración de Logging
                if (configuracion == null || !configuracion.TracingEnabled)
                {
                    enabled = false;
                    nivelLog = nivelesLog.Off;
                }
                else
                {
                    enabled = true;
                    TraceSourceData categ;
                    //Buscamos si el nombre de la clase tiene una categoria especial
                    categ = configuracion.TraceSources.Get(nombreClave);
                    if (categ == null)
                    {
                        //Sino tiene una categoria especial cogemos la categoria por defecto
                        categ = configuracion.TraceSources.Get(configuracion.DefaultCategory);
                        nombreClave = configuracion.DefaultCategory;
                    }

                    if (categ != null)
                    {
                        switch (categ.DefaultLevel)
                        {
                            case System.Diagnostics.SourceLevels.Off:
                                nivelLog = nivelesLog.Off;
                                break;
                            case System.Diagnostics.SourceLevels.Critical:
                                nivelLog = nivelesLog.Critical;
                                break;
                            case System.Diagnostics.SourceLevels.Error:
                                nivelLog = nivelesLog.Error;
                                break;
                            case System.Diagnostics.SourceLevels.Warning:
                                nivelLog = nivelesLog.Warning;
                                break;
                            case System.Diagnostics.SourceLevels.Information:
                                nivelLog = nivelesLog.Information;
                                break;
                            case System.Diagnostics.SourceLevels.Verbose:
                                nivelLog = nivelesLog.Debug;
                                break;
                            case System.Diagnostics.SourceLevels.ActivityTracing:
                                nivelLog = nivelesLog.Trace;
                                break;
                            case System.Diagnostics.SourceLevels.All:
                                nivelLog = nivelesLog.All;
                                break;
                        }
                    }
                    else
                    {
                        //Sino existen ninguna categoria desactivamos el Log
                        nivelLog = nivelesLog.Off;
                    }
                }
                #endregion

                #region Sin carga de configuración, valores fijos en la configuración
                /*
                enabled = true;
                nombreClave = "nombre";
                nivelLog = nivelesLog.All;
                */
                #endregion
            }
            catch (System.Exception fallo)
            {
              throw new System.Exception("Fallo en la lectura de la configuración de Log.", fallo);
            }
        }

        #endregion

        #region Comprobaciones de Seguimiento Log

        /// <summary>
        /// Indica si esta activa la gestión de Log en la aplicación
        /// </summary>
        public bool isEnabled
        {
            get { return enabled; }
        }

        /// <summary>
        /// Indica si esta activo el nivel de log de nivel Trace
        /// </summary>
        public bool isTraceEnabled
        {
            get { return (nivelLog <= nivelesLog.Trace); }
        }

        /// <summary>
        /// Indica si esta activo el nivel de log de nivel Debug
        /// </summary>
        public bool isDebugEnabled
        {
            get { return (nivelLog <= nivelesLog.Debug); }
        }

        /// <summary>
        /// Indica si esta activo el nivel de log de nivel Information
        /// </summary>
        public bool isInformationEnabled
        {
            get { return (nivelLog <= nivelesLog.Information); }
        }

        /// <summary>
        /// Indica si esta activo el nivel de log de nivel Warning
        /// </summary>
        public bool isWarningEnabled
        {
            get { return (nivelLog <= nivelesLog.Warning); }
        }

        /// <summary>
        /// Indica si esta activo el nivel de log de nivel Error
        /// </summary>
        public bool isErrorEnabled
        {
            get { return (nivelLog <= nivelesLog.Error); }
        }

        /// <summary>
        /// Indica si esta activo el nivel de log de nivel Critical
        /// </summary>
        public bool isCriticalEnabled
        {
            get { return (nivelLog <= nivelesLog.Critical); }
        }

        #endregion

        #region Metodos de Log directos

        /// <summary>
        /// Inserta un mensaje de Log de nivel Trace
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        public void Trace(string message)
        {
            if(isTraceEnabled)
                Logger.Write(string.Format("{0} {1}", clase, message), nombreClave, 6, 100, System.Diagnostics.TraceEventType.Resume);
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Debug
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        public void Debug(string message)
        {
            if(isDebugEnabled)
                Logger.Write(string.Format("{0} {1}", clase, message), nombreClave, 5, 100, System.Diagnostics.TraceEventType.Verbose);
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Information
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        public void Information(string message)
        {
            if(isInformationEnabled)
                Logger.Write(string.Format("{0} {1}", clase, message), nombreClave, 4, 100, System.Diagnostics.TraceEventType.Information);
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Warning
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        public void Warning(string message)
        {
            if(isWarningEnabled)
                Logger.Write(string.Format("{0} {1}", clase, message), nombreClave, 3, 100, System.Diagnostics.TraceEventType.Warning);
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Error
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        public void Error(string message)
        {
            if(isErrorEnabled)
                Logger.Write(string.Format("{0} {1}", clase, message), nombreClave, 2, 100, System.Diagnostics.TraceEventType.Error);
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Critical
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        public void Critical(string message)
        {
            if(isCriticalEnabled)
                Logger.Write(string.Format("{0} {1}", clase, message), nombreClave, 1, 100, System.Diagnostics.TraceEventType.Critical);
        }
        #endregion

        #region Metodos de Log de Excepciones
        /// <summary>
        /// Inserta un mensaje de Log de nivel Trace
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        public void Trace(string message, System.Exception exception)
        {
            if(isTraceEnabled)
                Logger.Write(string.Format("{0} {1}", clase, message) + "; Inner Exception: \r\n" + exception.GetType().ToString() + "\r\n" + exception.Source + "\r\n" + exception.Message + "\r\n" + exception.InnerException + "\r\n" + exception.StackTrace, nombreClave, 6, 100, System.Diagnostics.TraceEventType.Resume);
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Debug
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        public void Debug(string message, System.Exception exception)
        {
            if(isDebugEnabled)
                Logger.Write(string.Format("{0} {1}", clase, message) + "; Inner Exception: \r\n" + exception.GetType().ToString() + "\r\n" + exception.Source + "\r\n" + exception.Message + "\r\n" + exception.InnerException + "\r\n" + exception.StackTrace, nombreClave, 5, 100, System.Diagnostics.TraceEventType.Verbose);
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Information
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        public void Information(string message, System.Exception exception)
        {
            if(isInformationEnabled)
                Logger.Write(string.Format("{0} {1}", clase, message) + "; Inner Exception: \r\n" + exception.GetType().ToString() + "\r\n" + exception.Source + "\r\n" + exception.Message + "\r\n" + exception.InnerException + "\r\n" + exception.StackTrace, nombreClave, 4, 100, System.Diagnostics.TraceEventType.Information);
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Warning
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        public void Warning(string message, System.Exception exception)
        {
            if(isWarningEnabled)
                Logger.Write(string.Format("{0} {1}", clase, message) + "; Inner Exception: \r\n" + exception.GetType().ToString() + "\r\n" + exception.Source + "\r\n" + exception.Message + "\r\n" + exception.InnerException + "\r\n" + exception.StackTrace, nombreClave, 3, 100, System.Diagnostics.TraceEventType.Warning);
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Error
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        public void Error(string message, System.Exception exception)
        {
            if(isErrorEnabled)
                Logger.Write(string.Format("{0} {1}", clase, message) + "; Inner Exception: \r\n" + exception.GetType().ToString() + "\r\n" + exception.Source + "\r\n" + exception.Message + "\r\n" + exception.InnerException + "\r\n" + exception.StackTrace, nombreClave, 2, 100, System.Diagnostics.TraceEventType.Error);
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Critical
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        public void Critical(string message, System.Exception exception)
        {
            if(isCriticalEnabled)
                Logger.Write(string.Format("{0} {1}", clase, message) + "; Inner Exception: \r\n" + exception.GetType().ToString() + "\r\n" + exception.Source + "\r\n" + exception.Message + "\r\n" + exception.InnerException + "\r\n" + exception.StackTrace, nombreClave, 2, 100, System.Diagnostics.TraceEventType.Critical);
        }

        #endregion

        #region Metodos de Log Configurables

        /// <summary>
        /// Inserta un mensaje de Log de nivel Trace
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="parameters">Parametros del mensaje</param>
        public void Trace(string message, params object[] parameters)
        {
            try
            {
                if(isTraceEnabled)
                    Logger.Write(String.Format(string.Format("{0} {1}", clase, message), parameters), nombreClave, 6, 100, System.Diagnostics.TraceEventType.Resume);
            }
            catch (System.Exception Error)
            {
                this.Error("Error en escritura de Log Trace", Error);
            }
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Debug
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="parameters">Parametros del mensaje</param>
        public void Debug(string message, params object[] parameters)
        {
            try
            {
                if(isDebugEnabled)
                    Logger.Write(String.Format(string.Format("{0} {1}", clase, message), parameters), nombreClave, 5, 100, System.Diagnostics.TraceEventType.Verbose);
            }
            catch (System.Exception Error)
            {
                this.Error("Error en escritura de Log Debug", Error);
            }
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Information
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="parameters">Parametros del mensaje</param>
        public void Information(string message, params object[] parameters)
        {
            try
            {
                if(isInformationEnabled)
                    Logger.Write(String.Format(string.Format("{0} {1}", clase, message), parameters), nombreClave, 4, 100, System.Diagnostics.TraceEventType.Information);
            }
            catch (System.Exception Error)
            {
                this.Error("Error en escritura de Log Information", Error);
            }
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Warning
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="parameters">Parametros del mensaje</param>
        public void Warning(string message, params object[] parameters)
        {
            try
            {
                if(isWarningEnabled)
                    Logger.Write(String.Format(string.Format("{0} {1}", clase, message), parameters), nombreClave, 3, 100, System.Diagnostics.TraceEventType.Warning);
            }
            catch (System.Exception Error)
            {
                this.Error("Error en escritura de Log Warning", Error);
            }
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Error
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="parameters">Parametros del mensaje</param>
        public void Error(string message, params object[] parameters)
        {
            try
            {
                if(isErrorEnabled)
                    Logger.Write(String.Format(string.Format("{0} {1}", clase, message), parameters), nombreClave, 2, 100, System.Diagnostics.TraceEventType.Error);
            }
            catch (System.Exception Error)
            {
                this.Error("Error en escritura de Log Error", Error);
            }
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Critical
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="parameters">Parametros del mensaje</param>
        public void Critical(string message, params object[] parameters)
        {
            try
            {
                if(isCriticalEnabled)
                    Logger.Write(String.Format(string.Format("{0} {1}", clase, message), parameters), nombreClave, 1, 100, System.Diagnostics.TraceEventType.Critical);
            }
            catch (System.Exception Error)
            {
                this.Error("Error en escritura de Log Critical", Error);
            }
        }
        #endregion

        #region Metodos de Log Configurables con Excepcion

        /// <summary>
        /// Inserta un mensaje de Log de nivel Trace
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        /// <param name="parameters">Parametros del mensaje</param>
        public void Trace(string message, System.Exception exception, params object[] parameters)
        {
            try
            {
                if(isTraceEnabled)
                    Logger.Write(String.Format(string.Format("{0} {1}", clase, message), parameters) + "; Inner Exception: \r\n" + exception.GetType().ToString() + "\r\n" + exception.Source + "\r\n" + exception.Message + "\r\n" + exception.InnerException + "\r\n" + exception.StackTrace, nombreClave, 6, 100, System.Diagnostics.TraceEventType.Resume);
            }
            catch (System.Exception Error)
            {
                this.Error("Error en escritura de Log Trace", Error);
            }
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Debug
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        /// <param name="parameters">Parametros del mensaje</param>
        public void Debug(string message, System.Exception exception, params object[] parameters)
        {
            try
            {
                if(isDebugEnabled)
                    Logger.Write(String.Format(string.Format("{0} {1}", clase, message), parameters) + "; Inner Exception: \r\n" + exception.GetType().ToString() + "\r\n" + exception.Source + "\r\n" + exception.Message + "\r\n" + exception.InnerException + "\r\n" + exception.StackTrace, nombreClave, 5, 100, System.Diagnostics.TraceEventType.Verbose);
            }
            catch (System.Exception Error)
            {
                this.Error("Error en escritura de Log Debug", Error);
            }
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Information
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        /// <param name="parameters">Parametros del mensaje</param>
        public void Information(string message, System.Exception exception, params object[] parameters)
        {
            try
            {
                if(isInformationEnabled)
                    Logger.Write(String.Format(string.Format("{0} {1}", clase, message), parameters) + "; Inner Exception: \r\n" + exception.GetType().ToString() + "\r\n" + exception.Source + "\r\n" + exception.Message + "\r\n" + exception.InnerException + "\r\n" + exception.StackTrace, nombreClave, 4, 100, System.Diagnostics.TraceEventType.Information);
            }
            catch (System.Exception Error)
            {
                this.Error("Error en escritura de Log Information", Error);
            }
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Warning
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        /// <param name="parameters">Parametros del mensaje</param>
        public void Warning(string message, System.Exception exception, params object[] parameters)
        {
            try
            {
                if(isWarningEnabled)
                    Logger.Write(String.Format(string.Format("{0} {1}", clase, message), parameters) + "; Inner Exception: \r\n" + exception.GetType().ToString() + "\r\n" + exception.Source + "\r\n" + exception.Message + "\r\n" + exception.InnerException + "\r\n" + exception.StackTrace, nombreClave, 3, 100, System.Diagnostics.TraceEventType.Warning);
            }
            catch (System.Exception Error)
            {
                this.Error("Error en escritura de Log Warning", Error);
            }
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Error
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        /// <param name="parameters">Parametros del mensaje</param>
        public void Error(string message, System.Exception exception, params object[] parameters)
        {
            try
            {
                if(isErrorEnabled)
                    Logger.Write(String.Format(string.Format("{0} {1}", clase, message), parameters) + "; Inner Exception: \r\n" + exception.GetType().ToString() + "\r\n" + exception.Source + "\r\n" + exception.Message + "\r\n" + exception.InnerException + "\r\n" + exception.StackTrace, nombreClave, 2, 100, System.Diagnostics.TraceEventType.Error);
            }
            catch (System.Exception Error)
            {
                this.Error("Error en escritura de Log Error", Error);
            }
        }

        /// <summary>
        /// Inserta un mensaje de Log de nivel Critical
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        /// <param name="parameters">Parametros del mensaje</param>
        public void Critical(string message, System.Exception exception, params object[] parameters)
        {
            try
            {
                if(isCriticalEnabled)
                    Logger.Write(String.Format(string.Format("{0} {1}", clase, message), parameters) + "; Inner Exception: \r\n" + exception.GetType().ToString() + "\r\n" + exception.Source + "\r\n" + exception.Message + "\r\n" + exception.InnerException + "\r\n" + exception.StackTrace, nombreClave, 1, 100, System.Diagnostics.TraceEventType.Critical);
            }
            catch (System.Exception Error)
            {
                this.Error("Error en escritura de Log Critical", Error);
            }
        }
        #endregion
    }
}
