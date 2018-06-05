using Common.Cache;
using LoggerManager;
using Model.BL.DTO;
using Model.BL.Utils;
using Model.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.BL
{
    public class Prensas
    {
        ILogger log = LogFactory.GetLogger(typeof(Prensas));

        private const string ConnectionStringPrensasConfig = "PrensasConfigDB";

        private string _connectionString;
        
        public Prensas()//string connectionString = null)
        {
            this._connectionString = ConnectToPrensasConfig();
        }

        private string ConnectToPrensasConfig()
        {
            if (ConfigurationManager.ConnectionStrings[ConnectionStringPrensasConfig].ConnectionString != null)
            {
                return ConfigurationManager.ConnectionStrings[ConnectionStringPrensasConfig].ConnectionString;
            }
            else
                return null;
        }

        public Prensa Detalles(int id)
        {
            Prensa result = null;

            try
            {
                string nombreCache = string.Format("prensa{0}", id);

                if (!CacheData.Exist(nombreCache))
                {
                    PrensaDAL model = new PrensaDAL(_connectionString);

                    var prensa = model.Detalles(id);

                   result = Converter.ConvertToBL(prensa);

                    CacheData.Add(nombreCache, result);
                }
                else
                {
                    result = (Prensa)CacheData.Get(nombreCache);
                }
            }
            catch (Exception ex)
            {
                log.Error("Detalles()", ex);
            }

            return result;
        }

        public List<Prensa> Listar()
        {
            List<Prensa> result = null;

            try
            {
                PrensaDAL model = new PrensaDAL(_connectionString);
                var prensas = model.Listar();

                if (prensas != null)
                {
                    result = Converter.ConvertToBL(prensas);
                }
            }
            catch (Exception ex)
            {
                log.Error("Listar()", ex);
            }

            return result;
        }
        public bool Modificar(Prensa prensa)
        {
            bool updated = false;

            try
            {
                DAL.DTO.Prensa prensaDal = Converter.ConvertToDAL(prensa);
                

                PrensaDAL mod = new PrensaDAL(_connectionString);

                updated = mod.Modificar(prensaDal);


                if (updated)
                {
                    // Si había una caché para prensa la borramos                  
                    string nombreCache = string.Format("prensa{0}", prensa.Id);
                    CacheData.Remove(nombreCache);
                }
            }
            catch (Exception er)
            {
                log.Error("Agregar()", er);
            }

            return updated;
        }

        public bool Eliminar(int Id_Prensa)
        {
            bool sw = false;

            try
            {
                PrensaDAL model = new PrensaDAL(_connectionString);
                sw = model.Eliminar(Id_Prensa);

                if (sw)
                {
                    // Si había una caché para prensa la borramos                  
                    string nombreCache = string.Format("prensa{0}", Id_Prensa);
                    CacheData.Remove(nombreCache);
                }
            }
            catch (Exception er)
            {
                log.Error("Eliminar(Id_Prensa: {0})", er, Id_Prensa);
            }
            return sw;
        }
    }
}
