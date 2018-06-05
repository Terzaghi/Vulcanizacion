using LoggerManager;
using Model.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DAL.DTO;
using Model.DAL.Database;
using Model.DAL.Common;
using System.Data;

namespace Model.DAL
{
    public class Historico_ContramedidasDAL: IHistorico_Contramedidas
    {
        ILogger log = LogFactory.GetLogger(typeof(Historico_ContramedidasDAL));
        private string _connectionString;

        public Historico_ContramedidasDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Historico_Contramedidas entidad)
        {
            int id = -1;
            try
            {

                var accessor =  new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO HISTORICO_CONTRAMEDIDAS (EXPIRACION,CV,LOTE, ID_PRENSA)" +
                                       " VALUES (" + ic + "{0}, " + ic + "{1}, " + ic + "{2}," + ic + "{3} ) " + accessor.sqlGetNewIdentity(Arguments.Id, "{4}"),
                                       Arguments.Expiracion, Arguments.CV, Arguments.lote, Arguments.Id_Prensa, Arguments.Id);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Expiracion, entidad.Expiracion),
                    accessor.Parameter(Arguments.CV, entidad.CV),
                    accessor.Parameter(Arguments.lote, entidad.Lote),
                    accessor.Parameter(Arguments.Id_Prensa, entidad.Id_Prensa),
                    accessor.Parameter(Arguments.Id, 0, ParameterDirection.Output)
                };

                var result = accessor.ExecuteNonQueryWithResult(sql, parameters, false);

                if (result != null && typeof(int).Equals(result.GetType()))
                    id = (int)result;
                else
                    log.Warning("Agregar() No se ha completado la inserción");
            }
            catch (Exception ex)
            {
                log.Error("Agregar()", ex);
            }

            return id;
        }

        public Historico_Contramedidas Detalles(int id)
        {
            Historico_Contramedidas historico = null;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            Expiracion AS {1},
                                            CV AS {2},
                                            Lote AS {3},
                                            Id_Prensa AS {4}
                                        FROM HISTORICO_CONTRAMEDIDAS
                                        WHERE ID= " + ic + "{0}",
                                        Arguments.Id, Arguments.Expiracion, Arguments.CV, Arguments.lote, Arguments.Id_Prensa);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                historico = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return historico;
        }

        public IList<Historico_Contramedidas> Listar()
        {
            IList<Historico_Contramedidas> lst = null;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            Expiracion AS {1},
                                            CV AS {2},
                                            Lote AS {3},
                                            Id_Prensa AS {4}
                                        FROM HISTORICO_CONTRAMEDIDAS",
                                        Arguments.Id_Evento,
                                        Arguments.Nombre);

                List<IDataParameter> parameters = new List<IDataParameter>();

                var ds = accessor.FillDataSet(sql, parameters);

                lst = GetCollection(ds);
            }
            catch (Exception ex)
            {
                log.Error("Listar()", ex);
            }
            return lst;
        }


        #region "Utilities"

        private Historico_Contramedidas GetSingle(DataSet ds)
        {
            Historico_Contramedidas historico = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    historico = new Historico_Contramedidas()
                    {
                        Id = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id]),
                        Expiracion = Convert.ToDateTime(ds.Tables[0].Rows[0][Arguments.Expiracion]),
                        CV= Convert.ToString(ds.Tables[0].Rows[0][Arguments.CV]),
                        Lote = Convert.ToString(ds.Tables[0].Rows[0][Arguments.lote]),
                        Id_Prensa = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Prensa])

                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return historico;
        }

        private IList<Historico_Contramedidas> GetCollection(DataSet ds)
        {
            IList<Historico_Contramedidas> historicos = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    historicos = new List<Historico_Contramedidas>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        historicos.Add(new Historico_Contramedidas
                        {
                            Id = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id]),
                            Expiracion = Convert.ToDateTime(ds.Tables[0].Rows[i][Arguments.Expiracion]),
                            CV = Convert.ToString(ds.Tables[0].Rows[i][Arguments.CV]),
                            Lote = Convert.ToString(ds.Tables[0].Rows[i][Arguments.lote]),
                            Id_Prensa = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id_Prensa])

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return historicos;
        }
        #endregion
    }
}
