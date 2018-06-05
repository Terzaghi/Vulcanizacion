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
    public class Historico_DeshabilitacionDAL:IHistorico_Deshabilitacion
    {
        ILogger log = LogFactory.GetLogger(typeof(Historico_DeshabilitacionDAL));
        private string _connectionString;

        public Historico_DeshabilitacionDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Historico_Deshabilitacion entidad)
        {
            int id = -1;
            try
            {

                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO HISTORICO_DESHABILITACION (FECHA,COMENTARIO,ID_MOTIVO,ID_PERMISO, ID_PRENSA, ID_USUARIO, ID_DISPOSITIVO)" +
                                       " VALUES (" + ic + "{0}, " + ic + "{1}, " + ic + "{2}," + ic + "{3}, " + ic + "{4}, " + ic + "{5}, " + ic + "{6} ) " + accessor.sqlGetNewIdentity(Arguments.ID_Deshabilitacion, "{7}"),
                                       Arguments.Fecha, Arguments.Comentario, Arguments.ID_Motivo, Arguments.ID_Permiso, Arguments.Id_Prensa, 
                                       Arguments.ID_Usuario, Arguments.Id_Dispositivo, Arguments.ID_Deshabilitacion);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Fecha, entidad.Fecha),
                    accessor.Parameter(Arguments.Comentario, entidad.Comentario),
                    accessor.Parameter(Arguments.ID_Motivo, entidad.Id_Motivo),
                    accessor.Parameter(Arguments.ID_Permiso, entidad.Id_Permiso),
                    accessor.Parameter(Arguments.Id_Prensa, entidad.Id_Prensa),
                    accessor.Parameter(Arguments.ID_Usuario, entidad.Id_Usuario),
                    accessor.Parameter(Arguments.Id_Dispositivo, entidad.Id_Dispositivo),
                    accessor.Parameter(Arguments.ID_Deshabilitacion, 0, ParameterDirection.Output)
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

        public Historico_Deshabilitacion Detalles(int id)
        {
            Historico_Deshabilitacion historico = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_DESHABILITACION AS {0},
                                            FECHA AS {1},
                                            COMENTARIO AS {2},
                                            ID_MOTIVO AS {3},
                                            ID_PERMISO AS {4},
                                            ID_PRENSA AS {5},
                                            ID_USUARIO AS {6},
                                            ID_DISPOSITIVO AS {7}
                                        FROM HISTORICO_DESHABILITACION
                                        WHERE ID_DESHABILITACION= " + ic + "{0}",
                                        Arguments.ID_Deshabilitacion, Arguments.Fecha, Arguments.Comentario, 
                                        Arguments.ID_Motivo, Arguments.ID_Permiso, Arguments.Id_Prensa, Arguments.ID_Usuario, Arguments.Id_Dispositivo);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.ID_Deshabilitacion, id)
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

        public IList<Historico_Deshabilitacion> Listar()
        {
            IList<Historico_Deshabilitacion> lst = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                var sql = string.Format(@"SELECT
                                            ID_DESHABILITACION AS {0},
                                            FECHA AS {1},
                                            COMENTARIO AS {2},
                                            ID_MOTIVO AS {3},
                                            ID_PERMISO AS {4},
                                            ID_PRENSA AS {5},
                                            ID_USUARIO AS {6},
                                            ID_DISPOSITIVO AS {7}
                                        FROM HISTORICO_DESHABILITACION",
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

        private Historico_Deshabilitacion GetSingle(DataSet ds)
        {
            Historico_Deshabilitacion historico = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    historico = new Historico_Deshabilitacion()
                    {
                        Id_Deshabilitacion = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.ID_Deshabilitacion]),
                        Fecha = Convert.ToDateTime(ds.Tables[0].Rows[0][Arguments.Fecha]),
                        Comentario = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Comentario]),
                        Id_Motivo = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.ID_Motivo]),
                        Id_Permiso = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.ID_Permiso]),
                        Id_Prensa= Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Prensa]),
                        Id_Usuario= Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.ID_Usuario]),
                        Id_Dispositivo = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Dispositivo])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return historico;
        }

        private IList<Historico_Deshabilitacion> GetCollection(DataSet ds)
        {
            IList<Historico_Deshabilitacion> historicos = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    historicos = new List<Historico_Deshabilitacion>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        historicos.Add(new Historico_Deshabilitacion
                        {
                            Id_Deshabilitacion = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.ID_Deshabilitacion]),
                            Fecha = Convert.ToDateTime(ds.Tables[0].Rows[i][Arguments.Fecha]),
                            Comentario = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Comentario]),
                            Id_Motivo = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.ID_Motivo]),
                            Id_Permiso = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.ID_Permiso]),
                            Id_Prensa = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id_Prensa]),
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
