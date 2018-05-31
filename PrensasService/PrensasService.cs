using Communication.SignalR;
using DataProvider.Interfaces;
using LoggerManager;
using Model.BL.DTO;
using Model.BL.DTO.Enums;
using RequestManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ValuesMemory;
using WCF_RequestMotorServer;
using static RequestManager.RequestMotor;

namespace PrensasService
{
    public partial class PrensasService : ServiceBase
    {
        ILogger log = LogFactory.GetLogger(typeof(PrensasService));

        private readonly string _version;           //Versión del servicio
        private readonly string _configurationInfo; //Configuración del servicio

        MemoryValues _datosEnMemoria;
        PrensaCatalog.PrensaCatalog _catalogoPrensas;              // Catálogo de señales configuradas
        RequestMotor _motorSolicitudes;  // Validación y lanzamiento de las solicitudes
        DataProvidersManagement.DataProvidersManagement _proveedorDatos;
        RequestServerWCF _servidorWCF;

        public PrensasService()
        {
            InitializeComponent();
            if (ConfigurationManager.AppSettings["Service_Version"] != null)
            {
                _version = ConfigurationManager.AppSettings["Service_Version"].Trim();
            }

            _configurationInfo = string.Empty;

            if (ConfigurationManager.AppSettings["SignalR_Port"] != null)
            {
                string puertoSignalR = ConfigurationManager.AppSettings["SignalR_Port"];

                _configurationInfo += " SignalR: " + puertoSignalR;
            }

            if (ConfigurationManager.AppSettings["WCF_ListenPort"] != null)
            {
                _configurationInfo += " WCF: " + ConfigurationManager.AppSettings["WCF_ListenPort"];
            }
        }

        protected override void OnStart(string[] args)
        {
            log.Information("RequestManagerService OnStart. Versión: {0}", !string.IsNullOrEmpty(_version) ? _version : "null");
            log.Information("RequestManagerService OnStart. Configuración del servicio: {0}", !string.IsNullOrEmpty(_configurationInfo) ? _configurationInfo : "null");

            this.Inicialize();
            this.StartSignalR();
            
        }

        protected override void OnStop()
        {
            // Al cerrar guardará el estado de las variables del motor de reglas            
            if (this._motorSolicitudes != null)
            {
                this._motorSolicitudes.Dispose();
            }
        }

        private void Inicialize()
        {
            try
            {
                log.Information("#############################################################################");
                log.Information("##   INICIANDO PROCESO REQUEST MANAGER. VULCANIZADO                        ##");
                log.Information("#############################################################################");
                log.Information("");
                log.Information("  Fecha de inicio: {0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                log.Information("");

                log.Information("Inicializando sistema");

                //this._conexiones = new ConnectionsManager.Connections();
                this._catalogoPrensas = new PrensaCatalog.PrensaCatalog();

                this._datosEnMemoria = new MemoryValues();
    
                
                // Se va a inicializar el motor de solicitudes, que cargará de los archivos temporales las solicitudes anteriores que hubiera (puede tardar)
                DateTime fch1 = DateTime.UtcNow;
                log.Debug("Motor de Solicitudes. Iniciando y cargando reglas anteriores... (puede tardar)");

                this._proveedorDatos = new DataProvidersManagement.DataProvidersManagement(new DataProvider.TManager.Provider(ref _datosEnMemoria));

                { }
                this._proveedorDatos.DataChanged += _proveedorDatos_DataChanged;

                this._motorSolicitudes = new RequestMotor(ref this._datosEnMemoria, ref this._catalogoPrensas,ref this._proveedorDatos);
           
              
                TimeSpan tsTiempoCarga = DateTime.UtcNow - fch1;
                log.Debug(string.Format("Tiempo de inicialización: {0} sg", tsTiempoCarga.Seconds));

                // Ponemos a la escucha el servidor de WCF TCP

                InicializaWCF();

                InicializaSignalR();
            }
            catch (Exception er)
            {
                log.Error("Inicializa()", er);
            }
        }
        //private void _motorSolicitudes_RequestExpired(object sender, RequestExpiredEventArgs e)
        //{
        //    try
        //    {
        //        string str = string.Format(" **** Solicitud expirada (Id_Request: {0}, Id_RequestGenerated: {1})", e.Id_Request, e.Id_RequestGenerated);
        //        log.Debug(str);
        //    }
        //    catch (Exception er)
        //    {
        //        log.Error("_motorReglas_RequestExpired()", er);
        //    }
        //}

        /// <summary>
        /// Método que se lanza cuando tras evaluar una regla se cumple o se deja de cumplir la condición
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void _motorSolicitudes_RequestStateChanged(object sender, RequestStateChangedEventArgs e)
        //{
        //    try
        //    {
        //        string str = string.Format("CAMBIO DE ESTADO (Id_Request: {0}, Estado: {1})", e.Id_Request, e.estado);
        //        log.Debug(str);
        //    }
        //    catch (Exception er)
        //    {
        //        log.Error("_motorSolicitudes_RequestStateChanged()", er);
        //    }
        //}

        /// <summary>
        /// Con cada recepción de datos de los distintos proveedores se lanza el evento, y aqui valida si se cumple la condición
        /// y almacena los datos en las variables de sistema
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _proveedorDatos_DataChanged(DataProvider.Interfaces.DataReceivedEventArgs e)
        {
            try
            {
                     
                 // Una vez que ya han llegado todas las variables, y se han almacenado, lanzamos el evaluador solo para las señales que 
                // han cambiado. No se lanza junto con el anterior, ya que podría dar datos de variables que aún no estén almacenados
                // en las solicitudes
                _motorSolicitudes.EvaluateData(e.Value);
            }
            catch (Exception er)
            {
                log.Error("_proveedores_DataChanged()", er);
            }
        }


        #region SignalR
        private bool _signalR_Connected = false;

        private void StartSignalR()
        {
            if (SignalRManager.GetInstance.Start())
            {
                log.Information("SignalR server started");
                this._signalR_Connected = true;
            }
            else
            {
                log.Warning("SignalR server not started");
                this._signalR_Connected = false;
            }
        }

        private void InicializaSignalR()
        {
                   

            SignalRManager.GetInstance.OnClientConnected += GetInstance_OnClientConnected;
            SignalRManager.GetInstance.OnClientDisconnected += GetInstance_OnClientDisconnected;
            SignalRManager.GetInstance.OnRequestAccepted+= SignalRManager_OnRequestAccepted;
        }

        // Desconexiones realizadas a signalr
        private void GetInstance_OnClientDisconnected(string connectionId, string ip, int Id_User)
        {
            
        }

        // Conexiones realizadas a signalr 
        private void GetInstance_OnClientConnected(string connectionId, string ip, int Id_User)
        {
           
        }




        #region Events

        private void SignalRManager_OnRequestAccepted(string connectionId, int requestId)
        {
            log.Debug(string.Format("RequestAccepted: {0}, {1}", connectionId, requestId));
        }

        #endregion

        #endregion
        #region WCF

       

        public bool InicializaWCF()
        {
            bool sw = false;

            try
            {
                log.Information("InicializaWCF(). Levantando servicio WCF TCP");

                this._servidorWCF = new RequestServerWCF(ref this._motorSolicitudes, SignalRManager.GetInstance);
                sw = this._servidorWCF.Start();

                log.Information("Servidor de WCF: {0}", sw);
            }
            catch (Exception er)
            {
                log.Error("InicializaWCF()", er);
            }

            return sw;
        }

        #endregion

    }
}
