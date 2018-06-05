using DataProvider.Interfaces;
using DataProvider.TManager;
using LoggerManager;
using Model.BL;
using Model.BL.DTO;
using PrensaCatalog;
using RequestManager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using ValuesMemory;
using System.Linq;
using WCF_RequestMotorServer;
using RequestManager.Conditions;
using Memory.Common;

namespace ApplicationTest
{
    public partial class Form1 : Form
    {
        ILogger log = LogFactory.GetLogger(typeof(Form1));

        #region Attributes
        private readonly string _version;           //Versión del servicio
        private readonly string _configurationInfo; //Configuración del servicio
        MemoryValues _datosEnMemoria;
        PrensaCatalog.Prensas _catalogoPrensas;              // Catálogo de señales configuradas
        RequestMotor _motorSolicitudes;  // Validación y lanzamiento de las solicitudes
        DataProvidersManagement.DataProvidersManagement _proveedores;
        RequestServerWCF _servidorWCF;

        #endregion
        public Form1()
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

        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                
                this.Inicialize();
                _proveedores.Start();
            }
            catch (Exception ex)
            {
                log.Error("Initialize", ex.Message);
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

                this._proveedores = new DataProvidersManagement.DataProvidersManagement(new Provider(ref _datosEnMemoria));
                this._proveedores.DataChanged += _proveedores_DataChanged;


                // Se va a inicializar el motor de solicitudes, que cargará de los archivos temporales las solicitudes anteriores que hubiera (puede tardar)
                DateTime fch1 = DateTime.UtcNow;
                log.Debug("Motor de Solicitudes. Iniciando y cargando reglas anteriores... (puede tardar)");

                this._motorSolicitudes = new RequestMotor(ref this._datosEnMemoria, ref this._catalogoPrensas, ref this._proveedores);

                TimeSpan tsTiempoCarga = DateTime.UtcNow - fch1;
                log.Debug(string.Format("Tiempo de inicialización: {0} sg", tsTiempoCarga.Seconds));

             
            }
            catch (Exception er)
            {
                log.Error("Inicializa()", er);
            }
        }
        private void _proveedores_DataChanged(DataReceivedEventArgs e)
        {
            try
            {

                // Una vez que ya han llegado todas las variables, y se han almacenado, lanzamos el evaluador solo para las señales que 
                // han cambiado. No se lanza junto con el anterior, ya que podría dar datos de variables que aún no estén almacenados
                // en las solicitudes
                _motorSolicitudes.EvaluateData(e.Value);

                //Añadimos traza de tag evaluado:
                 TraceTagEvaluated(e.Value);

                //Actualizamos estado de datagridview
                PrensasDatos model = new PrensasDatos(null);
                List<PrensaDato> lstPrensasTags = model.ListarNuevosRegistros(DateTime.Now);
                var lstFiltro = lstPrensasTags.GroupBy(x => x.PrensaId)
                                              .Select(x => x.OrderByDescending(y => y.Fecha).First());
                dataGridView1.DataSource = lstFiltro;
            }
            catch (Exception er)
            {
               log.Error("_proveedores_DataChanged()", er);
            }
        }

        private void TraceTagEvaluated(TagValue value)
        {
            
            Dictionary<TagType, TagValue> memoriesValuesForPrensa = new Dictionary<TagType, TagValue>();
            Conditions evalConditions = new RequestManager.Conditions.Conditions();
            //Recogemos el valor de cada uno de los tags relacionados con PrensaId
            foreach (var item in Enum.GetNames(typeof(TagType)))
            {
                TagType tagType = (TagType)Enum.Parse(typeof(TagType), item);
                var memoryValue = _datosEnMemoria.GetTagValue(value.Id_Prensa, tagType);
                memoriesValuesForPrensa.Add(tagType, memoryValue);

            }
            //Evaluamos condiciones relacionadas con el tag que ha cambiado
            listBox1.Items.Add("Tag Evaluado: " + value.Type);
            listBox1.Items.Add("Tag " + value.Type + " a cambiado de " + memoriesValuesForPrensa[value.Type] + " a " + value.Value);
            List<ICondition> conditionsEvalToApply = evalConditions.GetConditions(value.Type);
        
            foreach (ICondition cond in conditionsEvalToApply)
            {
                bool validation = cond.validateCondition(value, memoriesValuesForPrensa[value.Type]);
                if (validation == true)
                {
                    switch (cond.action)
                    {
                        case ActionForRequest.Delete:
                            listBox1.Items.Add("Solicitud borrada");
                            break;
                        case ActionForRequest.Generated:
                            listBox1.Items.Add("Solicitud Creada");
                            break;
                    }
                }
            }
        }

    }
}
