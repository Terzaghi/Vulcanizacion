using Model.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DAL.DTO;
using LoggerManager;
using Model.DAL.Database;
using Model.DAL.Common;
using System.Data;

namespace Model.DAL
{
    public class PermisoDAL : IPermiso
    {
        ILogger log = LogFactory.GetLogger(typeof(PermisoDAL));
        private string _connectionString;

        public PermisoDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Permiso entidad)
        {
            int id = -1;
            try
            {

                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO ESTADO (NOMBRE)" +
                                       " VALUES (" + ic + "{0}) " + accessor.sqlGetNewIdentity(Arguments.ID_Permiso, "{1}"),
                                       Arguments.Nombre, Arguments.ID_Permiso);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                    accessor.Parameter(Arguments.ID_Permiso, 0, ParameterDirection.Output)
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

        public Permiso Detalles(int id)
        {
            Permiso permiso = null;

            try
            {
                var accessor =new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_PERMISO AS {0},
                                            Nombre AS {1}                                            
                                        FROM PERMISO
                                        WHERE ID_Permiso= " + ic + "{0}",
                                        Arguments.ID_Permiso, Arguments.Nombre);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.ID_Permiso, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                permiso = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return permiso;
        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {

                var accessor = new DataAccesor(_connectionString);
                string sql = string.Format("DELETE FROM PERMISO WHERE ID_PERMISO= {0}", id);

                List<IDataParameter> parameters = new List<IDataParameter>();


                var result = accessor.ExecuteNonQuery(sql, parameters, false);

                if (typeof(int).Equals(result.GetType()))
                    sw = (int)result > 0;
                else
                    log.Warning("Eliminar() No se ha completado la eliminación");
            }
            catch (Exception ex)
            {
                log.Error("Eliminar()", ex);
            }

            return sw;
        }

        public IList<Permiso> Listar()
        {
            IList<Permiso> lst = null;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                var sql = string.Format(@"SELECT
                                            ID_PERMISO  AS {0},
                                            NOMBRE AS {1}
                                        FROM PERMISO",
                                        Arguments.ID_Permiso,
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

        public bool Modificar(Permiso entidad)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format("UPDATE PERMISO SET NOMBRE = " + ic + "{0} WHERE ID_PERMISO = " + ic + "{1}",
                                        Arguments.Nombre, Arguments.ID_Permiso);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                     accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                     accessor.Parameter(Arguments.Id, entidad.Id_Permiso)
                };

                var result = accessor.ExecuteNonQuery(sql, parameters, false);

                if (typeof(int).Equals(result.GetType()))
                    sw = (int)result > 0;
                else
                    log.Warning("Modificar() No se ha completado la modificación");
            }
            catch (Exception ex)
            {
                log.Error("Modificar()", ex);
            }

            return sw;
        }


        #region "Utilities"

        private Permiso GetSingle(DataSet ds)
        {
            Permiso permiso = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    permiso = new Permiso()
                    {
                        Id_Permiso = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.ID_Permiso]),
                        Nombre = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Nombre])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return permiso;
        }

        private IList<Permiso> GetCollection(DataSet ds)
        {
            IList<Permiso> permisos = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    permisos= new List<Permiso>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        permisos.Add(new Permiso
                        {
                            Id_Permiso = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.ID_Permiso]),
                            Nombre = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Nombre])
                          
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return permisos;
        }
        #endregion

        public bool Vincular(int Id_Permiso, int Id_Rol, int Id_Motivo)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"INSERT INTO PERMISO_ROL (ID_PERMISO, ID_ROL, ID_Motivo)
                                                            VALUES (" + ic + "{0}, " + ic + "{1}, " + ic + "{2} )",
                                            Arguments.ID_Permiso,
                                            Arguments.IdRole,
                                            Arguments.ID_Motivo);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                   accessor.Parameter(Arguments.ID_Permiso, Id_Permiso),
                   accessor.Parameter(Arguments.IdRole, Id_Rol),
                   accessor.Parameter(Arguments.ID_Motivo, Id_Motivo)
                };


                int filasAfectadas = accessor.ExecuteNonQuery(sql, parameters, false);

                if (filasAfectadas > 0)
                    sw = true;
            }
            catch (Exception ex)
            {
                log.Error("Vincular()", ex);
            }

            return sw;
        }

        public bool Desvincular(int Id_User, int Id_Rol, int Id_Motivo)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"DELETE FROM USER_ROLES WHERE ID_USUARIO = " + ic + "{0} AND ID_ROL = " + ic + "{1}  AND ID_MOTIVO= " + ic + "{2}",
                        Arguments.ID_Usuario, Arguments.IdRole, Arguments.ID_Motivo);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.ID_Usuario, Id_User),
                    accessor.Parameter(Arguments.IdRole, Id_Rol),
                    accessor.Parameter(Arguments.ID_Motivo, Id_Motivo)
                };

                int numFilas = accessor.ExecuteNonQuery(sql, parameters, false);

                if (numFilas > 0)
                    sw = true;
            }
            catch (Exception ex)
            {
                log.Error("Desvincular()", ex);
            }

            return sw;
        }
    }
}
