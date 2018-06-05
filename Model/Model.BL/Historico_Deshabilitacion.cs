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
    public class Historico_Deshabilitacion
    {
        ILogger log = LogFactory.GetLogger(typeof(Historico_Deshabilitacion));

        private string _connectionString;

        public Historico_Deshabilitacion(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public long Agregar(BL.DTO.Historico_Deshabilitacion historico)
        {
            long id = -1;

            try
            {
                DAL.DTO.Historico_Deshabilitacion historicoDal = Converter.ConvertToDAL(historico);
                

                Historico_DeshabilitacionDAL mod = new Historico_DeshabilitacionDAL(_connectionString);

                id = mod.Agregar(historicoDal);
            }
            catch (Exception er)
            {
                log.Error("Agregar()", er);
            }

            return id;
        }

        public List<DTO.Historico_Deshabilitacion> Listar()
        {
            List<DTO.Historico_Deshabilitacion> result = null;

            try
            {
                Historico_DeshabilitacionDAL dal = new Historico_DeshabilitacionDAL(_connectionString);

                var resultsDal = dal.Listar();

                result = Converter.ConvertToBL(resultsDal);
            }
            catch (Exception er)
            {
                log.Error("Listar()", er);
            }

            return result;
        }

        public DTO.Historico_Deshabilitacion Detalles(int id)
        {
            DTO.Historico_Deshabilitacion result = null;

            try
            {
                Historico_DeshabilitacionDAL dal = new Historico_DeshabilitacionDAL(_connectionString);

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
