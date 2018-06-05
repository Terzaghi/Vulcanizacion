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
    public class RolDAL : IRol
    {
        ILogger log = LogFactory.GetLogger(typeof(RolDAL));
        private string _connectionString;

        public RolDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Rol entidad)
        {
            int id = -1;
            try
            {

                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO ROL (NOMBRE)" +
                                       " VALUES (" + ic + "{0}) " + accessor.sqlGetNewIdentity(Arguments.Id, "{1}"),
                                       Arguments.Nombre, Arguments.Id);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Nombre, entidad.Nombre),
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

        public Rol Detalles(int id)
        {
            Rol rol = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            Nombre AS {1}                                            
                                        FROM ROL
                                        WHERE ID= " + ic + "{0}",
                                        Arguments.Id, Arguments.Nombre);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                rol = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return rol;
        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {

                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string sql = string.Format("DELETE FROM ROL WHERE ID= {0}", id);

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

        public IList<Rol> Listar()
        {
            IList<Rol> lst = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                var sql = string.Format(@"SELECT
                                            ID  AS {0},
                                            NOMBRE AS {1}
                                        FROM ROL",
                                        Arguments.Id,
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

        public bool Modificar(Rol entidad)
        {
            bool sw = false;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format("UPDATE ROL SET NOMBRE = " + ic + "{0} WHERE ID = " + ic + "{1}",
                                        Arguments.Nombre, Arguments.Id);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                     accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                     accessor.Parameter(Arguments.Id, entidad.Id)
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

        private Rol GetSingle(DataSet ds)
        {
            Rol rol = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    rol = new Rol()
                    {
                        Id = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id]),
                        Nombre = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Nombre])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return rol;
        }

        private IList<Rol> GetCollection(DataSet ds)
        {
            IList<Rol> roles = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    roles= new List<Rol>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        roles.Add(new Rol
                        {
                            Id = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id]),
                            Nombre = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Nombre])
                          
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return roles;
        }
        #endregion

        public bool Vincular(int Id_User, int Id_Rol)
        {
            bool sw = false;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"INSERT INTO ROL_USUARIO (ID_USUARIO, ID_ROL)
                                                            VALUES (" + ic + "{0}, " + ic + "{1})",
                                            Arguments.ID_Usuario,
                                            Arguments.IdRole);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                   accessor.Parameter(Arguments.ID_Usuario, Id_User),
                   accessor.Parameter(Arguments.IdRole, Id_Rol)
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

        public bool Desvincular(int Id_User, int Id_Rol)
        {
            bool sw = false;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"DELETE FROM USER_ROLES WHERE ID_USUARIO = " + ic + "{0} AND ID_ROL = " + ic + "{1}",
                        Arguments.ID_Usuario, Arguments.IdRole);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.ID_Usuario, Id_User),
                    accessor.Parameter(Arguments.IdRole, Id_Rol)
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
