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
    public class Roles
    {
        ILogger log = LogFactory.GetLogger(typeof(Roles));

        private string _connectionString;
        public Roles(string connectionString = null)
        {
            this._connectionString = connectionString;
        }
        public Rol Detalles(int id)
        {
            Rol result = null;

            try
            {
                string nombreCache = string.Format("Rol{0}", id);

                if (!CacheData.Exist(nombreCache))
                {
                    RolDAL model = new RolDAL(_connectionString);

                    var rol = model.Detalles(id);

                    result = Converter.ConvertToBL(rol);

                    CacheData.Add(nombreCache, result);
                }
                else
                {
                    result = (Rol)CacheData.Get(nombreCache);
                }
            }
            catch (Exception ex)
            {
                log.Error("Detalles()", ex);
            }

            return result;
        }

        public List<Rol> Listar()
        {
            List<Rol> result = null;

            try
            {
                RolDAL model = new RolDAL(_connectionString);
                var roles = model.Listar();

                if (roles != null)
                {
                    result = Converter.ConvertToBL(roles);
                }
            }
            catch (Exception ex)
            {
                log.Error("Listar()", ex);
            }

            return result;
        }

        public bool Modificar(Rol rol)
        {
            bool updated = false;

            try
            {
                DAL.DTO.Rol rolDal = Converter.ConvertToDAL(rol);

                RolDAL mod = new RolDAL(_connectionString);

                updated = mod.Modificar(rolDal);


                if (updated)
                {
                    // Si había una caché para prensa la borramos                  
                    string nombreCache = string.Format("rol{0}", rol.rol);
                    CacheData.Remove(nombreCache);
                }
            }
            catch (Exception er)
            {
                log.Error("Agregar()", er);
            }

            return updated;
        }

        public bool Eliminar(int Id_Rol)
        {
            bool sw = false;

            try
            {
                RolDAL model = new RolDAL(_connectionString);
                sw = model.Eliminar(Id_Rol);

                if (sw)
                {
                    // Si había una caché para prensa la borramos                  
                    string nombreCache = string.Format("rol{0}", Id_Rol);
                    CacheData.Remove(nombreCache);
                }
            }
            catch (Exception er)
            {
                log.Error("Eliminar(Id_Role: {0})", er, Id_Rol);
            }
            return sw;
        }

        public bool Vincular(int Id_User, int Id_Rol)
        {
            bool sw = false;

            try
            {
                DAL.RolDAL mod = new DAL.RolDAL(_connectionString);
                sw = mod.Vincular(Id_User, Id_Rol);
            }
            catch (Exception er)
            {
                log.Error("Vincular()", er);
            }

            return sw;
        }

        public bool Desvincular(int Id_User, int Id_Rol)
        {
            bool sw = false;

            try
            {
                DAL.RolDAL mod = new DAL.RolDAL(_connectionString);
                sw = mod.Desvincular(Id_User, Id_Rol);
            }
            catch (Exception er)
            {
                log.Error("Desvincular()", er);
            }

            return sw;
        }
    }
}
