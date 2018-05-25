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
    public class EstadoDAL : IEstado
    {
        ILogger log = LogFactory.GetLogger(typeof(EstadoDAL));
        private string _connectionString;

        public EstadoDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Estado entidad)
        {
            int id = -1;
            try
            {

                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO ESTADO (NOMBRE)" +
                                       " VALUES (" + ic + "{0}) " + accessor.sqlGetNewIdentity(Arguments.Id_Estado, "{1}"),
                                       Arguments.Nombre, Arguments.Id_Estado);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                    accessor.Parameter(Arguments.Id_Estado, 0, ParameterDirection.Output)
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

        public Estado Detalles(int id)
        {
            Estado estado = null;

            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_Estado AS {0},
                                            Nombre AS {1}                                            
                                        FROM Estado
                                        WHERE ID_Estado= " + ic + "{0}",
                                        Arguments.Id_Estado, Arguments.Nombre);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id_Estado, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                estado = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return estado;
        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {

                var accessor = new DataAccesor();
                string sql = string.Format("DELETE FROM ESTADO WHERE ID_ESTADO = {0}", id);

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

        public IList<Estado> Listar()
        {
            IList<Estado> lst = null;

            try
            {
                var accessor = new DataAccesor();
                var sql = string.Format(@"SELECT
                                            ID_Estado  AS {0},
                                            NOMBRE AS {1}
                                        FROM ESTADO",
                                        Arguments.Id_Estado,
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

        public bool Modificar(Estado entidad)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format("UPDATE ESTADO SET NOMBRE = " + ic + "{0} WHERE ID_ESTADO = " + ic + "{1}",
                                        Arguments.Nombre, Arguments.Id_Estado);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                     accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                     accessor.Parameter(Arguments.Id_Estado, entidad.Id_Estado)
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

        private Estado GetSingle(DataSet ds)
        {
            Estado estado = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    estado = new Estado()
                    {
                        Id_Estado = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Estado]),
                        Nombre = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Nombre])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return estado;
        }

        private IList<Estado> GetCollection(DataSet ds)
        {
            IList<Estado> estados = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    estados= new List<Estado>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        estados.Add(new Estado
                        {
                            Id_Estado = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id_Estado]),
                            Nombre = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Nombre])
                          
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return estados;
        }
        #endregion
    }
}
