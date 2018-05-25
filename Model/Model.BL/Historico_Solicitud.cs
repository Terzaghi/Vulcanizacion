using LoggerManager;
using Model.BL.Utils;
using Model.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.BL
{
    public class Historico_Solicitud
    {
        ILogger log = LogFactory.GetLogger(typeof(Historico_Contramedidas));

        private string _connectionString;

        public Historico_Solicitud(string connectionString=null)
        {
            this._connectionString = connectionString;
        }

        public long Agregar(BL.DTO.Historico_Solicitud historico)
        {
            long id = -1;

            try
            {
                DAL.DTO.Historico_Solicitud historicoDal = Converter.ConvertToDAL(historico);
               

                Historico_SolicitudDAL mod = new Historico_SolicitudDAL(_connectionString);

                id = mod.Agregar(historicoDal);
            }
            catch (Exception er)
            {
                log.Error("Agregar()", er);
            }

            return id;
        }

        public List<DTO.Historico_Solicitud> Listar()
        {
            List<DTO.Historico_Solicitud> result = null;

            try
            {
                Historico_SolicitudDAL dal = new Historico_SolicitudDAL(_connectionString);

                var resultsDal = dal.Listar();

                result = Converter.ConvertToBL(resultsDal);
            }
            catch (Exception er)
            {
                log.Error("Listar()", er);
            }

            return result;
        }

        public DTO.Historico_Solicitud Detalles(int id)
        {
            DTO.Historico_Solicitud result = null;

            try
            {
                Historico_SolicitudDAL dal = new Historico_SolicitudDAL(_connectionString);

                var resultsDal = dal.Detalles(id);

            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return result;
        }
    }
}
