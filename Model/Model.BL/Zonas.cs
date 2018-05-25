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
    public class Zonas
    {
        ILogger log = LogFactory.GetLogger(typeof(Zonas));

        private string _connectionString;

        public Zonas(string connectionString = null)
        {
            this._connectionString = connectionString;
        }
        public Zona Detalles(int id)
        {
            Zona result = null;

            try
            {
                string nombreCache = string.Format("device{0}", id);

                if (!CacheData.Exist(nombreCache))
                {
                    ZonaDAL model = new ZonaDAL(_connectionString);

                    var zone = model.Detalles(id);

                    result = Converter.ConvertToBL(zone);

                    CacheData.Add(nombreCache, result);
                }
                else
                {
                    result = (Zona)CacheData.Get(nombreCache);
                }
            }
            catch (Exception ex)
            {
                log.Error("Detalles()", ex);
            }

            return result;
        }

        public List<Zona> Listar()
        {
            List<Zona> result = null;

            try
            {
                ZonaDAL model = new ZonaDAL(_connectionString);
                var zonas = model.Listar();

                if (zonas != null)
                {
                    result = Converter.ConvertToBL(zonas);
                }
            }
            catch (Exception ex)
            {
                log.Error("ListarTodos()", ex);
            }

            return result;
        }
        public bool Modificar(Zona zona)
        {
            bool updated = false;

            try
            {
                DAL.DTO.Zona zonaDal = Converter.ConvertToDAL(zona);
                
                ZonaDAL mod = new ZonaDAL(_connectionString);

                updated = mod.Modificar(zonaDal);

            }
            catch (Exception er)
            {
                log.Error("Agregar()", er);
            }

            return updated;
        }

        public bool Eliminar(int Id_Zona)
        {
            bool sw = false;

            try
            {
                ZonaDAL model = new ZonaDAL(_connectionString);
                sw = model.Eliminar(Id_Zona);
            }
            catch (Exception er)
            {
                log.Error("Eliminar(Id_Zona: {0})", er, Id_Zona);
            }
            return sw;
        }
    }
}
