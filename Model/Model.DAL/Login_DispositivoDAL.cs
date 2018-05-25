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
    public class Login_DispositivoDAL: ILogin_Dispositivo
    {
        ILogger log = LogFactory.GetLogger(typeof(Login_DispositivoDAL));
        private string _connectionString;

        public Login_DispositivoDAL(string connectionString=null)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Login_Dispositivo entidad)
        {
            int id = -1;
            try
            {

                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO LOGIN_DISPOSITIVO (ID_DISPOSITIVO,FECHA,CONNECTION_ID, ID_EVENTO, ID_USUARIO)" +
                                       " VALUES (" + ic + "{0}, " + ic + "{1}, " + ic + "{2}," + ic + "{3}, " + ic + "{4}) " + accessor.sqlGetNewIdentity(Arguments.ID_Login, "{5}"),
                                        Arguments.Id_Dispositivo, Arguments.Fecha, Arguments.Connection_ID, Arguments.Id_Evento, Arguments.ID_Usuario,Arguments.ID_Login);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id_Dispositivo, entidad.Id_Dispositivo),
                    accessor.Parameter(Arguments.Fecha, entidad.Fecha),
                    accessor.Parameter(Arguments.Connection_ID, entidad.Connection_Id),
                    accessor.Parameter(Arguments.Id_Evento, entidad.Id_Evento),
                    accessor.Parameter(Arguments.ID_Usuario, entidad.Id_Usuario),
                    accessor.Parameter(Arguments.ID_Login, 0, ParameterDirection.Output)
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

        public Login_Dispositivo Detalles(int id)
        {
            Login_Dispositivo historico = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_LOGIN AS {0},
                                            ID_DISPOSITIVO AS {1},
                                            FECHA AS {2},
                                            CONNECTION_ID AS {3}, 
                                            ID_EVENTO As {4}, 
                                            ID_USUARIO AS {5}
                                        FROM LOGIN_DISPOSITIVO
                                        WHERE ID_LOGIN= " + ic + "{0}",
                                        Arguments.ID_Login, Arguments.Id_Dispositivo, Arguments.Fecha, 
                                        Arguments.Connection_ID, Arguments.Id_Evento, Arguments.ID_Usuario);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.ID_Login, id)
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

        public IList<Login_Dispositivo> Listar()
        {
            IList<Login_Dispositivo> lst = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                var sql = string.Format(@"SELECT
                                           ID_LOGIN AS {0},
                                            ID_DISPOSITIVO AS {1},
                                            FECHA AS {2},
                                            CONNECTION_ID AS {3}, 
                                            ID_EVENTO As {4}, 
                                            ID_USUARIO AS {5}
                                        FROM LOGIN_DISPOSITIVO",
                                        Arguments.ID_Login, Arguments.Id_Dispositivo, Arguments.Fecha,
                                        Arguments.Connection_ID, Arguments.Id_Evento, Arguments.ID_Usuario);


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

        public IList<Login_Dispositivo> Listar(int lastIdDeviceLogin, int excludeEvent)
        {
            IList<Login_Dispositivo> logins = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_LOGIN AS {0},
                                            ID_DISPOSITIVO AS {1},
                                            FECHA AS {2},
                                            CONNECTION_ID AS {3}, 
                                            ID_EVENTO As {4}, 
                                            ID_USUARIO AS {5}
                                        FROM LOGIN_DISPOSITIVO
                                        WHERE ID_LOGIN > " + ic + "{6} "  +
                                        "AND ID_EVENTO <> " + ic + "{ 7}  " + 
                                        " ORDER BY DATEMOMENT",
                                    Arguments.ID_Login, Arguments.Id_Dispositivo,
                                    Arguments.Fecha, Arguments.Connection_ID,
                                    Arguments.Id_Evento, Arguments.ID_Usuario,
                                    Arguments.UltimoIdLoginDispositivo, Arguments.Evento_Excluido);

             

                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                   accessor.Parameter(Arguments.UltimoIdLoginDispositivo, lastIdDeviceLogin),
                   accessor.Parameter(Arguments.Evento_Excluido, excludeEvent)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                logins = GetCollection(ds);
            }
            catch (Exception ex)
            {
                log.Error("ListarEventosFinales()", ex);
            }

            return logins;
        }


        #region "Utilities"

        private Login_Dispositivo GetSingle(DataSet ds)
        {
            Login_Dispositivo historico = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    historico = new Login_Dispositivo()
                    {
                        Id_Login = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.ID_Login]),
                        Id_Dispositivo = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Dispositivo]),
                        Fecha= Convert.ToDateTime(ds.Tables[0].Rows[0][Arguments.Fecha]),
                        Connection_Id= Convert.ToString(ds.Tables[0].Rows[0][Arguments.Connection_ID]),
                        Id_Evento = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Evento]),
                        Id_Usuario = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.ID_Usuario])

                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return historico;
        }

        private IList<Login_Dispositivo> GetCollection(DataSet ds)
        {
            IList<Login_Dispositivo> historicos = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    historicos = new List<Login_Dispositivo>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        historicos.Add(new Login_Dispositivo
                        {
                            Id_Login = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.ID_Login]),
                            Id_Dispositivo = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id_Dispositivo]),
                            Fecha = Convert.ToDateTime(ds.Tables[0].Rows[i][Arguments.Fecha]),
                            Connection_Id = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Connection_ID]),
                            Id_Evento = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id_Evento]),
                            Id_Usuario = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.ID_Usuario])

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
