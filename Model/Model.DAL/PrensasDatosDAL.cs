using LoggerManager;
using Model.DAL.Common;
using Model.DAL.DTO;
using Model.DAL.Database;
using Model.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.DAL
{
    public class PrensasDatosDAL : IPrensasDatos
    {
        ILogger log = LogFactory.GetLogger(typeof(PrensasDatosDAL));

        private string _connectionString;

        public PrensasDatosDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<PrensaDato> ListarNuevosRegistros(DateTime Fecha)
        {
            List<PrensaDato> datos = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                var sql = string.Format(@"SELECT
                                                FECHA_HORA AS {0},
                                                PRENSA_NAME AS {1},
                                                PRENSA_TAG_ACTIVA AS {2},
                                                PRENSA_TAG_CV AS {3},
                                                PRENSA_TAG_TEMP AS {4},
                                                PRENSA_TAG_CICLO AS {5},
                                                PRENSA_TAG_PROD AS {6},
                                                PRENSA_CAVIDAD AS {7}
                                            FROM PRENSAS_DATOS
                                            WHERE FECHA_HORA > :{8}
                                            ORDER BY FECHA_HORA ASC",
                                        Arguments.Fecha, Arguments.Nombre, Arguments.Prensa_Activa,
                                        Arguments.Tag_CV, Arguments.Tag_Temp, Arguments.Tag_Ciclo,
                                        Arguments.Tag_Prod, Arguments.Cavidad, Arguments.Fecha);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Fecha, Fecha),
                };

                var ds = accessor.FillDataSet(sql, parameters);

                datos = GetCollection(ds);
            }
            catch (Exception ex)
            {
                log.Error("ListarNuevosRegistros. ", ex);
            }

            return datos;
        }

        #region Private Interface

        private List<PrensaDato> GetCollection(DataSet ds)
        {
            List<PrensaDato> tags = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {
                    tags = new List<PrensaDato>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        tags.Add(new PrensaDato
                        {
                            Fecha = Convert.ToDateTime(ds.Tables[0].Rows[i][Arguments.Fecha]),
                            PrensaId = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id]),
                            TagActivaValue = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Prensa_Activa]),
                            TagCVValue = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Tag_CV]),
                            TagTempValue = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Tag_Temp]),
                            TagCicloValue = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Tag_Ciclo]),
                            TagProdValue = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Tag_Prod]),
                            Cavidad = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Cavidad])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return tags;
        }

        #endregion
    }
}
