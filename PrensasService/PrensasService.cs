using Common.Security;
using Communication.SignalR;
using Communication.SignalR_Tester;
using LoggerManager;
using RequestManager;
using System;
using System.Configuration;
using System.ServiceProcess;
using ValuesMemory;
using WCF_RequestMotorServer;

namespace PrensasService
{
    public partial class PrensasService : ServiceBase
    {
        ILogger log = LogFactory.GetLogger(typeof(PrensasService));

        #region Attributes

        private readonly string _version;           //Versión del servicio
        private readonly string _configurationInfo; //Configuración del servicio

        MemoryValues _datosEnMemoria;
        PrensaCatalog.Prensas _catalogoPrensas;              // Catálogo de señales configuradas
        RequestMotor _motorSolicitudes;  // Validación y lanzamiento de las solicitudes
        DataProvidersManagement.DataProvidersManagement _proveedores;
        RequestServerWCF _servidorWCF;

        #endregion

        #region Initialization

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
            try
            {
                log.Information("RequestManagerService OnStart. Versión: {0}", !string.IsNullOrEmpty(_version) ? _version : "null");
                log.Information("RequestManagerService OnStart. Configuración del servicio: {0}", !string.IsNullOrEmpty(_configurationInfo) ? _configurationInfo : "null");

                this.Inicialize();
                _proveedores.Start();
                //this.StartSignalR();

                //CargarConfiguracionReinicioValidacionesSignalR();
            }catch(Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("PrensasService-OnStart", ex.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                // Detenemos los proveedores de datos
                this._proveedores.Stop();

                // Al cerrar guardará el estado de las variables del motor de reglas            
                if (this._motorSolicitudes != null)
                {
                    this._motorSolicitudes.Dispose();
                }
            }catch(Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("PrensasService-OnStop", ex.Message);
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
                
                this._catalogoPrensas = new PrensaCatalog.Prensas();
                this._datosEnMemoria = new MemoryValues();

                this._proveedores = new DataProvidersManagement.DataProvidersManagement(new DataProvider.TManager.Provider(ref _datosEnMemoria));
                this._proveedores.DataChanged += _proveedores_DataChanged;


                // Se va a inicializar el motor de solicitudes, que cargará de los archivos temporales las solicitudes anteriores que hubiera (puede tardar)
                DateTime fch1 = DateTime.UtcNow;
                log.Debug("Motor de Solicitudes. Iniciando y cargando reglas anteriores... (puede tardar)");
                
                this._motorSolicitudes = new RequestMotor(ref this._datosEnMemoria, ref this._catalogoPrensas,ref this._proveedores);
              
                TimeSpan tsTiempoCarga = DateTime.UtcNow - fch1;
                log.Debug(string.Format("Tiempo de inicialización: {0} sg", tsTiempoCarga.Seconds));

                // Ponemos a la escucha el servidor de WCF TCP

                InicializaWCF();

                InicializaSignalR();
            }
            catch (Exception er)
            {
                System.Diagnostics.EventLog.WriteEntry("PrensasService.Initialize", er.Message);
                log.Error("Inicializa()", er);
               
            }
        }

        #endregion

        /// <summary>
        /// Con cada recepción de datos de los distintos proveedores se lanza el evento, y aqui valida si se cumple la condición
        /// y almacena los datos en las variables de sistema
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _proveedores_DataChanged(DataProvider.Interfaces.DataReceivedEventArgs e)
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
                System.Diagnostics.EventLog.WriteEntry("PrensasService-proveedoresDataChange", er.Message);
                log.Error("_proveedores_DataChanged()", er);
              
            }
        }
        
        #region SignalR
        
        private void InicializaSignalR()
        {
            try
            {
                SignalRManager.GetInstance.OnClientConnected += GetInstance_OnClientConnected;
                SignalRManager.GetInstance.OnClientDisconnected += GetInstance_OnClientDisconnected;
            }catch(Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("PrensasService-InicializaSignalR", ex.Message);
            }
          
        }

        private void StartSignalR()
        {
            try
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
            }catch(Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("PrensasService-StartSignalR", ex.Message);
            }
        }

        #region  Comprobar caidas SignalR   

        private bool _signalR_Connected = false;

        private System.Timers.Timer _tmrComprobarSignalR = null;

        private int _tiempoRestanteComprobarSignalR = 0;

        private int _segundosParaSignalR = 60;

        private void CargarConfiguracionReinicioValidacionesSignalR()
        {
            try
            {
                log.Debug("Comprobando configuración para las validaciones de signalR");

                if (ConfigurationManager.AppSettings["TiempoValidacionSignalR"] != null)
                {
                    if (int.TryParse(ConfigurationManager.AppSettings["TiempoValidacionSignalR"], out this._segundosParaSignalR))
                    {
                        if (_segundosParaSignalR > 0)
                        {
                            log.Debug("Están programadas las validaciones de SignalR cada {0} segundos", _segundosParaSignalR);
                            ValidarSignalR_Start();
                        }
                        else
                        {
                            log.Debug("No están programados las validaciones a signalR");
                        }
                    }
                }
            }
            catch (Exception er)
            {
                System.Diagnostics.EventLog.WriteEntry("PrensasService-CargarConfiguracionReinicioValidacionesSignalR", er.Message);
                log.Error("CargarConfiguracionReinicioAutomatico()", er);
              
            }
        }

        private void ValidarSignalR_Start()
        {
            try
            {
                if (_tmrComprobarSignalR == null)
                {
                    _tmrComprobarSignalR = new System.Timers.Timer();
                    _tmrComprobarSignalR.Interval = 1000;
                    _tmrComprobarSignalR.Elapsed += _tmrComprobarSignalR_Elapsed;
                    _tiempoRestanteComprobarSignalR = _segundosParaSignalR;

                    _tmrComprobarSignalR.Start();
                }
                else
                {
                    // Ya estaba iniciado, lo detenemos
                    _tmrComprobarSignalR.Stop();
                    _tmrComprobarSignalR.Dispose();
                    _tmrComprobarSignalR = null;

                    GC.Collect();
                }
            }
            catch (Exception er)
            {
                System.Diagnostics.EventLog.WriteEntry("PrensasService-ValidarSignalR_Start", er.Message);
                log.Error("ValidarSignalR_Start()", er);
              
            }
        }

        private void _tmrComprobarSignalR_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _tiempoRestanteComprobarSignalR--;

                //LabelReinicioEscribir(_tiempoRestanteComprobarSignalR);

                if (_tiempoRestanteComprobarSignalR <= 0)
                {
                    // Detenemos mientras validamos y luego reanudamos                    
                    _tmrComprobarSignalR.Stop();

                    // Lanzamos las validaciones
                    ValidarSignalR();

                    // Volvemos a activar las comprobaciones
                    _tiempoRestanteComprobarSignalR = _segundosParaSignalR;
                    _tmrComprobarSignalR.Start();
                }
            }
            catch (Exception er)
            {
                System.Diagnostics.EventLog.WriteEntry("PrensasService-_tmrComprobarSignalR_Elapsed", er.Message);
                log.Error("_tmrComprobarSignalR_Elapsed()", er);
                
            }
        }

        private void ValidarSignalR()
        {
            try
            {
                // Realiza una petición a signalR para que nos indique si está activo
                string puertoSignalR = ConfigurationManager.AppSettings["SignalR_Port"];
                string url = string.Format("http://localhost:{0}/", puertoSignalR);

                // Generamos un token de sistema
                Common.Security.TokenManager tokGen = new Common.Security.TokenManager();
                string strToken = tokGen.GenerateStringToken(0, 0, null);

                SignalR_Tester sR = new SignalR_Tester();

                sR.TestConnection(url, strToken, result =>
                {
                    log.Debug("ValidarSignalR(). Estado servidor SignalR: {0}", result);

                    this._signalR_Connected = result;

                    if (!result)
                    {
                        // No se pudo conectar con el servidor                        
                        ReiniciarAplicacion();
                    }
                });
            }
            catch (Exception er)
            {
                System.Diagnostics.EventLog.WriteEntry("PrensasService-ValidarSignalR", er.Message);
                log.Error("ValidarSignalR()", er);
               
            }
        }

        private void ReiniciarAplicacion()
        {
            try
            {
                log.Debug("Persistiendo el estado actual de la aplicación");
                this._motorSolicitudes.PersistirVariables();

                // Por si algo tarda en liberarse o liberar el puerto
                log.Debug("Liberando recursos");
                this._motorSolicitudes.Dispose();
                //this._webApi.Dispose();
                this._servidorWCF.Dispose();
                this._proveedores.Dispose();

                log.Information("REINICIANDO EL SERVICIO");
                //Application.Restart();


                RestartService();
                //System.Diagnostics.Process.Start(Application.ExecutablePath);
                //Application.Exit();
            }
            catch (Exception er)
            {
                System.Diagnostics.EventLog.WriteEntry("PrensasService-ReiniciarAplicacion", er.Message);
                log.Error("ReiniciarAplicacion()", er);
            }
        }

        private void RestartService()
        {
            try
            {
                var service = new ServiceController(ServiceName);

                //Se establece un timeout tanto para parar el servicio como para iniciarlo de 30 segundos
                TimeSpan timeout = TimeSpan.FromMilliseconds(30000);

                log.Debug("Parando el servicio");
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                log.Debug("Iniciando el servicio");
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);

                log.Debug("Servicio iniciado");
            }
            catch (Exception er)
            {
                System.Diagnostics.EventLog.WriteEntry("PrensasService-RestartService", er.Message);
                log.Error("RestartService()", er);
            }
        }

        #endregion

        #region Eventos

        // Desconexiones realizadas a signalr
        private void GetInstance_OnClientDisconnected(string connectionId, string ip, int Id_User)
        {
            
        }

        // Conexiones realizadas a signalr 
        private void GetInstance_OnClientConnected(string connectionId, string ip, int Id_User)
        {
           
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
                System.Diagnostics.EventLog.WriteEntry("PrensasService-InicializaWCF", er.Message);
                log.Error("InicializaWCF()", er);
            }

            return sw;
        }

        #endregion

    }
}
