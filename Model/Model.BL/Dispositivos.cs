using Common.Cache;
using Common.Security;
using LoggerManager;
using Model.BL.DTO;
using Model.BL.Utils;
using Model.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.BL
{
    public class Dispositivos
    {
        ILogger log = LogFactory.GetLogger(typeof(Dispositivos));

        private string _connectionString;

        public Dispositivos(string connectionString = null)
        {
            this._connectionString = connectionString;
        }

        #region Securización

        public Token SecurityToken { get; set; }

        public Dispositivos(Token token=null, string connectionString = null)
        {
            this.SecurityToken = token;
            this._connectionString = connectionString;
        }

        #endregion

        public Dispositivo Detalles(int id)
        {
            Dispositivo result = null;

            try
            {
                string nombreCache = string.Format("device{0}", id);

                if (!CacheData.Exist(nombreCache))
                {
                    DispositivoDAL model = new DispositivoDAL(_connectionString);

                    var device = model.Detalles(id);

                    result = Converter.ConvertToBL(device);

                    CacheData.Add(nombreCache, result);
                }
                else
                {
                    result = (Dispositivo)CacheData.Get(nombreCache);
                }
            }
            catch (Exception ex)
            {
                log.Error("Detalles()", ex);
            }

            return result;
        }

        public Dispositivo GetDetailsByIP(string ip)
        {
            Dispositivo result = null;

            try
            {
                if (!string.IsNullOrEmpty(ip))
                {
                    string nombreCache = string.Format("deviceIP{0}", ip);

                    if (!CacheData.Exist(nombreCache))
                    {
                        // No existe en cache, lo solicitamos a BD
                        DispositivoDAL model = new DispositivoDAL(_connectionString);

                        var device = model.GetDetailsByIp(ip);

                        result = Converter.ConvertToBL(device);
                    }
                    else
                    {
                        // Este dato ya se solicitó anteriormente, devolvemos de caché
                        result = (Dispositivo)CacheData.Get(nombreCache);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetDetailsByIP()", ex);
            }

            return result;
        }

        public List<Dispositivo> ListarTodos()
        {
            List<Dispositivo> result = null;

            try
            {
                DispositivoDAL model = new DispositivoDAL(_connectionString);
                var dispositivos = model.Listar();

                if (dispositivos != null)
                {
                    result = Converter.ConvertToBL(dispositivos);
                }
            }
            catch (Exception ex)
            {
                log.Error("ListarTodos()", ex);
            }

            return result;
        }

        public List<Dispositivo> Listar()
        {
            List<Dispositivo> result = null;

            try
            {
                DispositivoDAL model = new DispositivoDAL(_connectionString);

                IList<DAL.DTO.Dispositivo> devices=null;

                // Dependiendo de los permisos de usuario, devuelve lo que el usuario puede ver
                if (this.SecurityToken != null)
                {
                    // devices = model.Listar_FiltradoPorUsuario(this.SecurityToken.Id_Usuario);
                }
                else
                {
                    devices = model.Listar();
                }

                if (devices != null)
                {
                    result = Converter.ConvertToBL(devices);
                }
            }
            catch (Exception ex)
            {
                log.Error("Listar()", ex);
            }

            return result;
        }

        public List<string> GetDevicesIPs(List<int> devicesIds)
        {
            List<string> result = null;

            try
            {
                result = new List<string>();

                // Cargamos también las configuraciones de IPs vinculadas para estos dispositivos
                if (devicesIds != null)
                {
                    foreach (var idDevice in devicesIds)
                    {
                        var dev = Detalles(idDevice);

                        if (dev != null)
                        {
                            result.Add(dev.IP);
                        }
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("GetDevicesIPs()", er);
            }
            return result;
        }
    }
}
