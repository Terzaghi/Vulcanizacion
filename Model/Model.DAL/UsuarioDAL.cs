using LoggerManager;
using Model.DAL.Common;
using Model.DAL.Interfaces;
using System;
using System.Collections.Generic;
using Model.DAL.Database;
using System.Data;
using Model.DAL.DTO;


namespace Model.DAL
{
    public class UsuarioDAL : IUsuario
    {
        ILogger log = LogFactory.GetLogger(typeof(UsuarioDAL));
        private string _connectionString;

        public UsuarioDAL(string connectionString)
        {
            _connectionString = connectionString;
        }


        public int Agregar(Usuario entidad)
        {
            int id = -1;
            try
            {

                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO USUARIO (NOMBRE, IDENTITY_CODE, PASSWORD)" +
                                       " VALUES (" + ic + "{0}, " + ic + "{1}," + ic + "{2}) " + accessor.sqlGetNewIdentity(Arguments.Id, "{3}"), 
                                       Arguments.Nombre, Arguments.Identity_Code, Arguments.Password,  Arguments.Id);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                    accessor.Parameter(Arguments.Identity_Code, entidad.Identity_Code),
                    accessor.Parameter(Arguments.Password, entidad.Password),
                    accessor.Parameter(Arguments.Id, 0, ParameterDirection.Output)
                };

                var result = accessor.ExecuteNonQueryWithResult(sql, parameters,false);

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

        public Usuario Detalles(int id)
        {
            Usuario user = null;

            try
            {
                var accessor =new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            Nombre AS {1},
                                            Identity_Code AS {2},
                                            Password AS {3}
                                        FROM USUARIO
                                        WHERE ID = " + ic + "{0}",
                                        Arguments.Id, Arguments.Nombre, Arguments.Identity_Code,
                                        Arguments.Password
                                        );

            

                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                user = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return user;
        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {

                var accessor = new DataAccesor(_connectionString);
                string sql = string.Format("DELETE FROM USUARIO WHERE ID = {0}", id);

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

        public IList<Usuario> Listar()
        {
            IList<Usuario> lst = null;

            try
            {
                var accessor =new DataAccesor(_connectionString);
                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            NOMBRE AS {1},
                                            IDENTITY_CODE AS {2},
                                            PASSWORD AS {3}
                                        FROM USUARIO",
                                        Arguments.Id,
                                        Arguments.Nombre,
                                        Arguments.Identity_Code, Arguments.Password);

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

        public bool Modificar(Usuario entidad)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format("UPDATE USUARIO SET NOMBRE = " + ic + "{0}, IDENTITY_CODE = " + ic + "{1}, \"PASSWORD\" = " + ic + "{2} WHERE ID = " + ic + "{3}",
                                        Arguments.Nombre, Arguments.Identity_Code,Arguments.Password, Arguments.Id);

           

                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                     accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                     accessor.Parameter(Arguments.Identity_Code, Convert.ToString(entidad.Identity_Code)),
                     accessor.Parameter(Arguments.Password, Convert.ToString(entidad.Password)),
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

        public Usuario GetUserByIdentityCode(string identityCode)
        {
            Usuario usuario = null;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            Nombre AS {1},
                                            Identity_Code AS {2},
                                            Password AS {3}
                                        FROM USUARIO
                                        WHERE Identity_Code = " + ic + "{2}",
                                        Arguments.Id, Arguments.Nombre, Arguments.Identity_Code,
                                        Arguments.Password
                                        );

                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                   accessor.Parameter(Arguments.Identity_Code, identityCode)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                usuario = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("GetUserByIdentityCode({0})", ex, identityCode);
            }
            return usuario;
        }



        private enum ResupuestasValidacion
        {
            Error = -1,
            Usuario_Inexistente = -2,
            Credenciales_Invalidas = -3,
            Password_Requerido = -4
        }
      
        public int ValidateCredentials(string user, string password)
        {
            int id_Usuario = (int)ResupuestasValidacion.Error;

            try
            {
                var accessor =  new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            Nombre AS {1},
                                            Identity_Code AS {2},
                                            Password AS {3}
                                        FROM USUARIO
                                        WHERE Identity_Code = " + ic + "{2}",
                                        Arguments.Id, Arguments.Nombre, Arguments.Identity_Code,
                                        Arguments.Password
                                        );



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Identity_Code, user)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        id_Usuario = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id]);

                        string pwd = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Password]);

                        // Validamos la pwd que coincida
                        if (pwd != password)
                        {
                            // Credenciales no válidas
                            id_Usuario = (int)ResupuestasValidacion.Credenciales_Invalidas;
                        }

                        if (password.Length == 0 && pwd.Length > 0)
                        {
                            // El usuario requiere una contraseña
                            id_Usuario = (int)ResupuestasValidacion.Password_Requerido;
                        }
                    }
                    else
                    {
                        // Se conectó a BD, pero el usuario no existe
                        id_Usuario = (int)ResupuestasValidacion.Usuario_Inexistente;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("ValidateCredentials(User: {0})", ex, user);
            }
            return id_Usuario;
        }

        public int ValidarIdentityCode_Libre(string identityCode)
        {
            int Id_User = -1;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format(@"select 
                                        Id
                                        from Usuario
                                        where
                                            IdentityCode = " + ic + "{0}",
                                        Arguments.Identity_Code);

          

                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                   accessor.Parameter(Arguments.Identity_Code, identityCode)
                };

                //var ds = accessor.FillDataSet(sql, parameters);
                Id_User = Convert.ToInt32(accessor.ExecuteScalar(sql, parameters, false));
            }
            catch (Exception ex)
            {
                log.Error("ValidarIdentityCode_Libre(IdentityCode: {0})", ex, identityCode);
            }
            return Id_User;
        }

        #region "Utilities"

        private Usuario GetSingle(DataSet ds)
        {
            Usuario usuario = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    usuario = new Usuario()
                    {
                        Id = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id]),
                        Nombre = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Nombre]),
                        Identity_Code = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Identity_Code]),
                        Password = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Password])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return usuario;
        }

        private IList<Usuario> GetCollection(DataSet ds)
        {
            IList<Usuario> users = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    users = new List<Usuario>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        users.Add(new Usuario
                        {
                            Id = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id]),
                            Nombre = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Nombre]),
                            Identity_Code = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Identity_Code]),
                            Password = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Password])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return users;
        }
        #endregion

        public bool Vincular(int Id_User, int Id_Prensa)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"INSERT INTO USUARIO_PRENSA (ID_USUARIO, ID_PRENSA,FECHA_ASIGNACION)
                                                            VALUES (" + ic + "{0}, " + ic + "{1}, " + ic + "{2})",
                                            Arguments.ID_Usuario,
                                            Arguments.Id_Prensa, Arguments.Fecha_Asignacion);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                   accessor.Parameter(Arguments.ID_Usuario, Id_User),
                   accessor.Parameter(Arguments.Id_Prensa, Id_Prensa),
                   accessor.Parameter(Arguments.Fecha_Asignacion, DateTime.Now)
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

        public bool Desvincular(int Id_User, int Id_Prensa)
        {
            bool sw = false;

            try
            {
                var accessor =  new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"DELETE FROM USUARIO_PRENSA WHERE ID_USUARIO = " + ic + "{0} AND ID_PRENSA= " + ic + "{1}",
                        Arguments.ID_Usuario, Arguments.Id_Prensa);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.ID_Usuario, Id_User),
                    accessor.Parameter(Arguments.IdRole, Id_Prensa)
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
        public IList<Tuple<int,int>> ListarPrensasUsuarios()
        {
            IList<Tuple<int, int>> lst = null;

            try
            {
                var accessor =new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_PRENSA AS {0},
                                            ID_USUARIO AS {1}
                                        FROM USUARIO_PRENSA",
                                        Arguments.Id_Prensa, Arguments.ID_Usuario);

                List<IDataParameter> parameters = new List<IDataParameter>();
               

                var ds = accessor.FillDataSet(sql, parameters);

               
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    lst = new List<Tuple<int, int>>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var id_prensa = Convert.ToInt16(ds.Tables[0].Rows[i][Arguments.Id_Prensa]);
                        var id_usuario = Convert.ToInt16(ds.Tables[0].Rows[i][Arguments.ID_Usuario]);
                        var tuple = new Tuple<int, int>(id_prensa,id_usuario);
                        lst.Add(tuple);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("ListarPrensasDeUsuario()", ex);
            }
            return lst;
        }
    }
}
