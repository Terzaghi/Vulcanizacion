using Common.Security;
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
    public class Historico_Contramedidas
    {
        ILogger log = LogFactory.GetLogger(typeof(Historico_Contramedidas));

        private string _connectionString;

        public Historico_Contramedidas(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public long Agregar(BL.DTO.Historico_Contramedidas historico)
        {
            long id = -1;

            try
            {
                DAL.DTO.Historico_Contramedidas historicoDal = Converter.ConvertToDAL(historico);
               
                Historico_ContramedidasDAL mod = new Historico_ContramedidasDAL(_connectionString);

                id = mod.Agregar(historicoDal);
            }
            catch (Exception er)
            {
                log.Error("Agregar()", er);
            }

            return id;
        }

        public List<DTO.Historico_Contramedidas> Listar()
        {
            List<DTO.Historico_Contramedidas> result = null;

            try
            {
                Historico_ContramedidasDAL dal = new Historico_ContramedidasDAL(_connectionString);

                var resultsDal = dal.Listar();

                result = Converter.ConvertToBL(resultsDal);
            }
            catch (Exception er)
            {
                log.Error("Listar()", er);
            }

            return result;
        }

        public DTO.Historico_Contramedidas Detalles(int id)
        {
            DTO.Historico_Contramedidas result = null;

            try
            {
                Historico_ContramedidasDAL dal = new Historico_ContramedidasDAL(_connectionString);

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
