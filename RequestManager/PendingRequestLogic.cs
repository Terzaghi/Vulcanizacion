using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using LoggerManager;
using Model.BL.DTO.Enums;
using RequestManager.DTO;

namespace RequestManager
{
    /// <summary>
    /// Gestiona y valida que si la solicitud ha de eliminarse de la memoria de notificaciones pendientes
    /// </summary>
    [DataContract]
    public class PendingRequestLogic
    {
        ILogger log = LogFactory.GetLogger(typeof(PendingRequestLogic));

        RequestGenerated request;
        // Listados de usuarios/dispositivos a los que se les envía
        [DataMember(Name = "_estadoUsuarios")]
        private Dictionary<int, Estado_Solicitud> _estadoUsuarios = new Dictionary<int, Estado_Solicitud>();
        [DataMember(Name = "_estadoDispositivos")]
        private Dictionary<int, Estado_Solicitud> _estadoDispositivos = new Dictionary<int, Estado_Solicitud>();

        // Puntero al motor de solicitudes
        RequestMotor _ref_requestManager = null;

        // Configuración de la acción para usuarios dispositivos
        [DataMember(Name = "Ids_Users")]
        private List<int> Ids_Users { get; set; }
     
        [DataMember(Name = "Ids_Devices")]
        private List<int> Ids_Devices { get; set; }

        public PendingRequestLogic(RequestGenerated request,RequestMotor motorSolicitudes)
        {
            try
            {
                // Vinculando al puntero del motor de solicitudes
                this._ref_requestManager = motorSolicitudes;

                // El deserializador entrará sin parámetro para asignar automáticamente los parámetros
                if (request != null)
                {

                    // Almacenamos una copia del objeto (en lugar de un puntero a la solicitud que sería única, la clonamos)
                    this.request = (RequestGenerated) request.Clone();

                    // Carga las propiedades de la solicitud
                    CargarListadoDestintatarios(request);
                }
            }
            catch (Exception er)
            {
                log.Error("PendingNotificationLogic()", er);
            }
        }

        #region Carga los destinatarios de la acción
        private void CargarListadoDestintatarios(RequestGenerated request)
        {
            try
            {
                // Cargando configuración de usuarios vinculados a esta notificación

                if (request.Ids_Users != null)
                {
                    // Guardamos la configuración original
                    this.Ids_Users = request.Ids_Users;


                    // Usuarios
                    log.Debug("Cargando usuarios vinculados a la solicitud");
                    foreach (var idUsr in request.Ids_Users)
                    {
                        this._estadoUsuarios.Add(idUsr, Estado_Solicitud.Pendiente);
                    }
                }
                if (request.Ids_Devices != null)
                {
                    this.Ids_Devices = request.Ids_Devices;
                    // Dispositivos
                    foreach (var idDev in request.Ids_Devices)
                    {
                        this._estadoDispositivos.Add(idDev, Estado_Solicitud.Pendiente);
                    }
                }
                 
            }
            catch (Exception er)
            {
                log.Error("CargarListadoDestintatarios()", er);
            }
        }
        #endregion
        #region Devolución de datos/objetos 
      

        public RequestGenerated GetRequestGenerated()
        {
            RequestGenerated result = null;

            try
            {
                result = (RequestGenerated)this.request.Clone();
            }
            catch (Exception er)
            {
                log.Error("GetNotificationGenerated()", er);
            }

            return result;
        }
        #endregion

        #region Consulta del estado de las notificaciones

        /// <summary>
        /// Devuelve el estado para el dispositivo actual
        /// </summary>
        public Estado_Solicitud? GetDeviceState(int? Id_Device)
        {
            Estado_Solicitud? result = null;

            try
            {
                if (Id_Device.HasValue && _estadoDispositivos.ContainsKey(Id_Device.Value))
                {
                    result = _estadoDispositivos[Id_Device.Value];
                }
            }
            catch (Exception ex)
            {
                log.Error("GetDeviceState()", ex);
            }

            return result;
        }

        /// <summary>
        /// Devuelve el estado para el usuario actual
        /// </summary>
        public Estado_Solicitud? GetUserState(int? Id_User)
        {
            Estado_Solicitud? result = null;

            try
            {
                if (Id_User.HasValue && _estadoUsuarios.ContainsKey(Id_User.Value))
                {
                    result = _estadoUsuarios[Id_User.Value];
                }
            }
            catch (Exception ex)
            {
                log.Error("GetUserState()", ex);
            }

            return result;
        }


        /// <summary>
        /// Devuelve el estado más alto de la solicitd entre todos los enviados
        /// </summary>
        /// <returns></returns>
        public Estado_Solicitud? GetState()
        {
            Estado_Solicitud? result = null;

            try
            {
                List<Estado_Solicitud> lstCombinada = new List<Estado_Solicitud>();
                CombinarEstados(this._estadoUsuarios, ref lstCombinada);
                CombinarEstados(this._estadoDispositivos, ref lstCombinada);

                foreach (var estado in lstCombinada)
                {
                    if (result == null)
                        result = estado;

                    // Los estados vienen ya ordenados de menor a mayor,
                    // marcamos con el más importante (por ej. reconocido)
                    if (estado > result)
                        result = estado;
                }
            }
            catch (Exception er)
            {
                log.Error("GetState()", er);
            }

            return result;
        }


        /// <summary>
        /// Comprueba el estado para el usuario actual o dispositivo
        /// </summary>
        /// <param name="Id_User"></param>
        /// <param name="Id_Device"></param>
        /// <returns></returns>
        public Estado_Solicitud? GetState(int? Id_User, int? Id_Device)
        {
            Estado_Solicitud? result = null;

            try
            {
                // Si se entra con una misma IP lo marcaría como sistema también, y tiene estado al entrar, definimos que si el usuario es 0 no lo guarde tb (ya que es sistema)
                Estado_Solicitud estadoUsuario, estadoDispositivo;

                if (Id_Device != null)
                {
                    if (this._estadoDispositivos.TryGetValue((int)Id_Device, out estadoDispositivo))
                        result = estadoDispositivo;
                }

                if (Id_User != null && Id_User != 0)
                {
                    if (this._estadoUsuarios.TryGetValue((int)Id_User, out estadoUsuario))
                        result = estadoUsuario;
                }
            }
            catch (Exception er)
            {
                log.Error("GetState()", er);
            }

            return result;
        }
        #endregion

        #region Validaciones negocio
        public void SetConfiguration(int? Id_User, int? Id_Device, Estado_Solicitud state)
        {
            try
            {
                // Establecemos el estado del usuario (si viene un usuario o el usuario no es el sistema, que entonces vendrá por IP)
                if (Id_User != null && Id_User != 0)
                {
                    // Marcamos el estado, pasando como parámetro la lista de usuarios
                    MarcarEstado(ref this._estadoUsuarios, Id_User, state);
                }

                // Establecemos el estado del dispositivo
                if (Id_Device != null)
                {
                    // Marcamos el estado, pasando como parámetro la lista de dispositivos
                    MarcarEstado(ref this._estadoDispositivos, Id_Device, state);
                }
            }
            catch (Exception er)
            {
                log.Error("SetConfiguration()", er);
            }
        }

        private void MarcarEstado(ref Dictionary<int, Estado_Solicitud> diccionarioEstados, int? id, Estado_Solicitud state)
        {
            Estado_Solicitud punteroEstado;

            lock (diccionarioEstados)
            {
                bool actualizar = true;

                if (diccionarioEstados.TryGetValue((int)id, out punteroEstado))
                {
                    // Comprobamos si el estado a asignar es superior al anterior
                    if (state > punteroEstado)
                    {
                        // Si ya existe lo eliminamos antes de actualizarlo
                        diccionarioEstados.Remove((int)id);
                    }
                    else
                        actualizar = false;
                }

                if (actualizar)
                {
                    log.Debug("Asignando estado (Id: {0}, State: {1})", id, state);
                    diccionarioEstados.Add((int)id, state);
                }

                // Revisamos los estados de las pendingnotification
                //ActualizarEstadoParaPostactions(state);
            }
        }

        /// <summary>
        /// Comprueba si el elemento está en estado de eliminarlo porque ya está visto o reconocido
        /// </summary>
        /// <returns></returns>
        public bool IsEnabled()
        {
            bool swActiva = true;

            try
            {
                // Combinamos ambos elementos en una única lista que validar
                List<Estado_Solicitud> lstCombinada = new List<Estado_Solicitud>();
                lock (this._estadoUsuarios)
                {
                    CombinarEstados(this._estadoUsuarios, ref lstCombinada);
                }
                lock (this._estadoDispositivos)
                {
                    CombinarEstados(this._estadoDispositivos, ref lstCombinada);
                }

                // Comprobamos si todos al menos la han visto
                bool swVistaTodos = false;//VistaPorTodos(lstCombinada);

                // Si la solicitud está configurada como que requiere ack, no la podemos cerrar directamente                
                RequestGenerated solicitudGenerated = this.GetRequestGenerated();

                if (solicitudGenerated != null && solicitudGenerated.AckRequiered)
                {
                    // Validamos que además de haberla visto todos, alguien la haya reconocido
                    bool swReconocida = lstCombinada.Contains(Estado_Solicitud.Aceptada);

                    if (swVistaTodos && swReconocida)
                        swActiva = false;
                }
                else
                {
                    // Si todos la han visto la podemos ya cerrar                        
                    if (swVistaTodos)
                        swActiva = false;
                }

            }
            catch (Exception er)
            {
                log.Error("IsEnabled()", er);
            }

            return swActiva;
        }
             

        private void CombinarEstados(Dictionary<int, Estado_Solicitud> diccionario, ref List<Estado_Solicitud> lstResultante)
        {
            foreach (KeyValuePair<int, Estado_Solicitud> entry in diccionario)
            {
                lstResultante.Add(entry.Value);
            }

        }
        #endregion

        #region Valida si la solicitud era para nuestro usuario
        /// <summary>
        /// Comprueba si la acción está destinada a este destinatario
        /// </summary>
        /// <param name="Id_User"></param>
        /// <param name="Id_Group"></param>
        /// <returns></returns>
        public bool IsValidDestinatary(int? Id_User, int? Id_Device)
        {
            bool sw = false;

            try
            {
                // Si no se ha configurado usuario, grupo ni dispositivo va a broadcast
                if ((this.Ids_Users == null || this.Ids_Users.Count == 0) &&
                   (this.Ids_Devices == null || this.Ids_Devices.Count == 0))
                    sw = true;
                else
                {
                    // Comprobamos si hay filtro por usuario (el 0 es sistema, se validará por dispositivo)
                    if (Id_User != null && Id_User != 0)
                    {
                        // Se solicita un filtro por usuario
                        if (this.Ids_Users != null)
                        {
                            if (this.Ids_Users.Contains((int)Id_User))
                            {
                                sw = true;
                            }
                        }
                    }
                    // Comprobamos si se ha enviado a nuestro dispositivo/IP
                    if (Id_Device != null)
                    {
                        if (this.Ids_Devices.Contains((int)Id_Device))
                        {
                            sw = true;
                        }
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("IsValidDestinatary()", er);
            }


            return sw;
        }
        #endregion

    }
}
