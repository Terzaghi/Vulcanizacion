using LoggerManager;
using Model.BL.DTO;
using Model.BL.Utils;
using Model.DAL;
using System;
using System.Collections.Generic;

namespace Model.BL
{
    public class PrensasDatos
    {
        ILogger log = LogFactory.GetLogger(typeof(PrensasDatos));

        private string _connectionString;

        public PrensasDatos(string connectionString=null)
        {
            this._connectionString = connectionString;
        }

        public List<PrensaDato> ListarNuevosRegistros(DateTime Fecha)
        {
            List<PrensaDato> result = null;

            try
            {
                PrensasDatosDAL model = new PrensasDatosDAL(_connectionString);

                var datosDAL = model.ListarNuevosRegistros(Fecha);

                result = Converter.ConvertToBL(datosDAL);
            }
            catch (Exception ex)
            {
                log.Error("ListarNuevosRegistros. ", ex);
            }

            return result;
        }
    }
}
