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
    public class Permisos
    {
        ILogger log = LogFactory.GetLogger(typeof(Permisos));

        private string _connectionString;
        public Permisos(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Permiso Detalles(int id)
        {
            Permiso result = null;

            try
            {
                string nombreCache = string.Format("Permiso{0}", id);

                if (!CacheData.Exist(nombreCache))
                {
                    PermisoDAL model = new PermisoDAL(_connectionString);

                    var permiso = model.Detalles(id);

                    result = Converter.ConvertToBL(permiso);

                    CacheData.Add(nombreCache, result);
                }
                else
                {
                    result = (Permiso)CacheData.Get(nombreCache);
                }
            }
            catch (Exception ex)
            {
                log.Error("Detalles()", ex);
            }

            return result;
        }

        public List<Permiso> Listar()
        {
            List<Permiso> result = null;

            try
            {
                PermisoDAL model = new PermisoDAL(_connectionString);
                var permisos = model.Listar();

                if (permisos != null)
                {
                    result = Converter.ConvertToBL(permisos);
                }
            }
            catch (Exception ex)
            {
                log.Error("Listar()", ex);
            }

            return result;
        }

        public bool Modificar(Permiso permiso)
        {
            bool updated = false;

            try
            {
                DAL.DTO.Permiso permisoDal = Converter.ConvertToDAL(permiso);             

                PermisoDAL mod = new PermisoDAL(_connectionString);

                updated = mod.Modificar(permisoDal);


                if (updated)
                {
                    // Si había una caché para prensa la borramos                  
                    string nombreCache = string.Format("permiso{0}", permiso.tipoPermiso);
                    CacheData.Remove(nombreCache);
                }
            }
            catch (Exception er)
            {
                log.Error("Agregar()", er);
            }

            return updated;
        }

        public bool Eliminar(int Id_Permiso)
        {
            bool sw = false;

            try
            {
                PermisoDAL model = new PermisoDAL(_connectionString);
                sw = model.Eliminar(Id_Permiso);

                if (sw)
                {
                    // Si había una caché para prensa la borramos                  
                    string nombreCache = string.Format("permiso{0}", Id_Permiso);
                    CacheData.Remove(nombreCache);
                }
            }
            catch (Exception er)
            {
                log.Error("Eliminar(Id_Permiso: {0})", er, Id_Permiso);
            }
            return sw;
        }
    }
}
