using Common.Cache;
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
    public class Solicitudes
    {
        ILogger log = LogFactory.GetLogger(typeof(Solicitudes));

        private string _connectionString;

        public Solicitudes(string connectionString = null)
        {
            this._connectionString = connectionString;
        }
        public int Agregar(Solicitud solicitud)
        {
            int id = -1;

            try
            {
                log.Debug("Agregar(). Se va a agregar una nueva solicitud");

                SolicitudDAL model = new SolicitudDAL(_connectionString);
             
                DAL.DTO.Solicitud solicitudDal = Converter.ConvertToDAL(solicitud);
             
               id = model.Agregar(solicitudDal);
              
            }
            catch (Exception er)
            {
                log.Error("Agregar()", er);
            }

            return id;
        }
        public Solicitud Detalles(int id)
        {
            Solicitud result = null;

            try
            {
                string nombreCache = string.Format("solicitud{0}", id);

                if (!CacheData.Exist(nombreCache))
                {
                    SolicitudDAL model = new SolicitudDAL(_connectionString);

                    var solicitud = model.Detalles(id);

                    result = Converter.ConvertToBL(solicitud);

                    CacheData.Add(nombreCache, result);
                }
                else
                {
                    result = (Solicitud)CacheData.Get(nombreCache);
                }
            }
            catch (Exception ex)
            {
                log.Error("Detalles()", ex);
            }

            return result;
        }

        public List<Solicitud> Listar()
        {
            List<Solicitud> result = null;

            try
            {
                SolicitudDAL model = new SolicitudDAL(_connectionString);
                var solicitudes = model.Listar();

                if (solicitudes != null)
                {
                    result = Converter.ConvertToBL(solicitudes);
                }
            }
            catch (Exception ex)
            {
                log.Error("Listar()", ex);
            }

            return result;
        }
        public bool Modificar(Solicitud solicitud)
        {
            bool updated = false;

            try
            {
                DAL.DTO.Solicitud solicitudDal = Converter.ConvertToDAL(solicitud);
               

                SolicitudDAL mod = new SolicitudDAL(_connectionString);

                updated = mod.Modificar(solicitudDal);


                if (updated)
                {
                    // Si había una caché para prensa la borramos                  
                    string nombreCache = string.Format("solicitud{0}", solicitud.Id);
                    CacheData.Remove(nombreCache);
                }
            }
            catch (Exception er)
            {
                log.Error("Agregar()", er);
            }

            return updated;
        }

        public bool Eliminar(int Id_Solicitud)
        {
            bool sw = false;

            try
            {
                PrensaDAL model = new PrensaDAL(_connectionString);
                sw = model.Eliminar(Id_Solicitud);

                if (sw)
                {
                    // Si había una caché para prensa la borramos                  
                    string nombreCache = string.Format("solicitud{0}", Id_Solicitud);
                    CacheData.Remove(nombreCache);
                }
            }
            catch (Exception er)
            {
                log.Error("Eliminar(Id_Solicitud: {0})", er, Id_Solicitud);
            }
            return sw;
        }
    }
}
