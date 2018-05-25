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
    public class Especificaciones
    {
        ILogger log = LogFactory.GetLogger(typeof(Dispositivos));

        private string _connectionString;

        public Especificaciones(string connectionString = null)
        {
            this._connectionString = connectionString;
        }
        public List<Especificacion> Listar()
        {
            List<Especificacion> result = null;

            try
            {
                EspecificacionDAL model = new EspecificacionDAL(_connectionString);

                IList<DAL.DTO.Especificacion> especificaciones = null;
                            
                especificaciones = model.Listar();
                

                if (especificaciones != null)
                {
                    result = Converter.ConvertToBL(especificaciones);
                }
            }
            catch (Exception ex)
            {
                log.Error("Listar()", ex);
            }

            return result;
        }
    }
}
