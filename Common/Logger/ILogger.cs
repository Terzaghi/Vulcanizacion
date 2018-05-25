using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerManager
{
    public interface ILogger
    {
        #region Comprobaciones de Seguimiento Log
        /// <summary>
        /// Indica si esta activa la gestión de Log en la aplicación
        /// </summary>
        bool isEnabled { get; }

        /// <summary>
        /// Indica si esta activo el nivel de log de nivel Trace
        /// </summary>
        bool isTraceEnabled { get; }

        /// <summary>
        /// Indica si esta activo el nivel de log de nivel Debug
        /// </summary>
        bool isDebugEnabled { get; }

        /// <summary>
        /// Indica si esta activo el nivel de log de nivel Information
        /// </summary>
        bool isInformationEnabled { get; }

        /// <summary>
        /// Indica si esta activo el nivel de log de nivel Warning
        /// </summary>
        bool isWarningEnabled { get; }

        /// <summary>
        /// Indica si esta activo el nivel de log de nivel Error
        /// </summary>
        bool isErrorEnabled { get; }

        /// <summary>
        /// Indica si esta activo el nivel de log de nivel Critical
        /// </summary>
        bool isCriticalEnabled { get; }
        #endregion

        #region Metodos de Log directos

        /// <summary>
        /// Inserta un mensaje de Log de nivel Trace
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        void Trace(string message);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Debug
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        void Debug(string message);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Information
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        void Information(string message);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Warning
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        void Warning(string message);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Error
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        void Error(string message);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Critical
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        void Critical(string message);
        #endregion

        #region Metodos de Log de Excepciones
        /// <summary>
        /// Inserta un mensaje de Log de nivel Trace
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        void Trace(string message, System.Exception exception);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Debug
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        void Debug(string message, System.Exception exception);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Information
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        void Information(string message, System.Exception exception);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Warning
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        void Warning(string message, System.Exception exception);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Error
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        void Error(string message, System.Exception exception);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Critical
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        void Critical(string message, System.Exception exception);

        #endregion

        #region Metodos de Log Configurables

        /// <summary>
        /// Inserta un mensaje de Log de nivel Trace
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="parameters">Parametros del mensaje</param>
        void Trace(string message, params object[] parameters);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Debug
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="parameters">Parametros del mensaje</param>
        void Debug(string message, params object[] parameters);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Information
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="parameters">Parametros del mensaje</param>
        void Information(string message, params object[] parameters);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Warning
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="parameters">Parametros del mensaje</param>
        void Warning(string message, params object[] parameters);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Error
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="parameters">Parametros del mensaje</param>
        void Error(string message, params object[] parameters);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Critical
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="parameters">Parametros del mensaje</param>
        void Critical(string message, params object[] parameters);
        #endregion

        #region Metodos de Log Configurables con Excepcion

        /// <summary>
        /// Inserta un mensaje de Log de nivel Trace
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        /// <param name="parameters">Parametros del mensaje</param>
        void Trace(string message, System.Exception exception, params object[] parameters);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Debug
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        /// <param name="parameters">Parametros del mensaje</param>
        void Debug(string message, System.Exception exception, params object[] parameters);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Information
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        /// <param name="parameters">Parametros del mensaje</param>
        void Information(string message, System.Exception exception, params object[] parameters);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Warning
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        /// <param name="parameters">Parametros del mensaje</param>
        void Warning(string message, System.Exception exception, params object[] parameters);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Error
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        /// <param name="parameters">Parametros del mensaje</param>
        void Error(string message, System.Exception exception, params object[] parameters);

        /// <summary>
        /// Inserta un mensaje de Log de nivel Critical
        /// </summary>
        /// <param name="message">Mensaje a insertar</param>
        /// <param name="exception">Excepción recogida</param>
        /// <param name="parameters">Parametros del mensaje</param>
        void Critical(string message, System.Exception exception, params object[] parameters);

        #endregion
    }
}
