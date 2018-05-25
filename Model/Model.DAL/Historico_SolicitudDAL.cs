using LoggerManager;
using Model.DAL.Common;
using Model.DAL.Database;
using Model.DAL.DTO;
using Model.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAL
{
    public class Historico_SolicitudDAL:IHistorico_Solicitud
    {
        ILogger log = LogFactory.GetLogger(typeof(Historico_SolicitudDAL));
        private string _connectionString;

        public Historico_SolicitudDAL(string connectionString=null)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Historico_Solicitud entidad)
        {
            int id = -1;
            try
            {

                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO HISTORICO_SOLICITUD (FECHA,ID_SOLICITUD,ID_ESTADO, ID_USUARIO, ID_DISPOSITIVO)" +
                                       " VALUES (" + ic + "{0}, " + ic + "{1}, " + ic + "{2}," + ic + "{3}, " + ic + "{4}) " + accessor.sqlGetNewIdentity(Arguments.ID_Historico, "{5}"),
                                       Arguments.Fecha, Arguments.ID_Solicitud, Arguments.Id_Estado, Arguments.ID_Usuario, Arguments.Id_Dispositivo, 
                                       Arguments.ID_Historico);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Fecha, entidad.Fecha),
                    accessor.Parameter(Arguments.ID_Solicitud, entidad.Id_Solicitud),
                    accessor.Parameter(Arguments.Id_Estado, entidad.Id_Estado),
                    accessor.Parameter(Arguments.ID_Usuario, entidad.Id_Usuario),
                    accessor.Parameter(Arguments.Id_Dispositivo, entidad.Id_Dispositivo),
                    accessor.Parameter(Arguments.ID_Historico, 0, ParameterDirection.Output)
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

        public Historico_Solicitud Detalles(int id)
        {
            Historico_Solicitud historico = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_HISTORICO AS {0},
                                            FECHA AS {1},
                                            ID_SOLICITUD AS {2},
                                            ID_ESTADO AS {3},
                                            ID_USUARIO AS {4}, 
                                            ID_DISPOSITIVO AS {5}
                                        FROM HISTORICO_SOLICITUD
                                        WHERE ID_HISTORICO= " + ic + "{0}",
                                        Arguments.ID_Historico, Arguments.Fecha, Arguments.ID_Solicitud, 
                                        Arguments.Id_Estado, Arguments.ID_Usuario, Arguments.Id_Dispositivo);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.ID_Historico, id)
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

        public IList<Historico_Solicitud> Listar()
        {
            IList<Historico_Solicitud> lst = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                var sql = string.Format(@"SELECT
                                           ID_HISTORICO AS {0},
                                            FECHA AS {1},
                                            ID_SOLICITUD AS {2},
                                            ID_ESTADO AS {3},
                                            ID_USUARIO AS {4}, 
                                            ID_DISPOSITIVO AS {5}
                                        FROM HISTORICO_SOLICITUD",
                                        Arguments.ID_Historico, Arguments.Fecha, Arguments.ID_Solicitud,
                                        Arguments.Id_Estado, Arguments.ID_Usuario, Arguments.Id_Dispositivo);

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

        private Historico_Solicitud GetSingle(DataSet ds)
        {
            Historico_Solicitud historico = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    historico = new Historico_Solicitud()
                    {
                        Id_Historico = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.ID_Historico]),
                        Fecha = Convert.ToDateTime(ds.Tables[0].Rows[0][Arguments.Fecha]),
                        Id_Solicitud = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.ID_Solicitud]),
                        Id_Estado = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Estado]),
                        Id_Usuario = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.ID_Usuario]),
                        Id_Dispositivo= Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Dispositivo])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return historico;
        }

        private IList<Historico_Solicitud> GetCollection(DataSet ds)
        {
            IList<Historico_Solicitud> historicos = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    historicos = new List<Historico_Solicitud>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        historicos.Add(new Historico_Solicitud
                        {
                            Id_Historico = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.ID_Historico]),
                            Fecha = Convert.ToDateTime(ds.Tables[0].Rows[i][Arguments.Fecha]),
                            Id_Solicitud = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.ID_Solicitud]),
                            Id_Estado = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id_Estado]),
                            Id_Usuario = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.ID_Usuario]),
                            Id_Dispositivo = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id_Dispositivo])

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
