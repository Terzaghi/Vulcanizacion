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
    public class ContramedidaDAL : IContramedida
    {
        ILogger log = LogFactory.GetLogger(typeof(ContramedidaDAL));
        private string _connectionString;

        public ContramedidaDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Contramedida entidad)
        {
            int id = -1;
            try
            {

                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO CONTRAMEDIDA (NOMBRE)" +
                                       " VALUES (" + ic + "{0}) " + accessor.sqlGetNewIdentity(Arguments.Id_Contramedida, "{1}"),
                                       Arguments.Nombre, Arguments.Id_Contramedida);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                    accessor.Parameter(Arguments.Id_Contramedida, 0, ParameterDirection.Output)
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

        public Contramedida Detalles(int id)
        {
            Contramedida contramedida = null;

            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_Contramedida AS {0},
                                            Nombre AS {1}                                            
                                        FROM Contramedida
                                        WHERE ID_Contramedida = " + ic + "{0}",
                                        Arguments.Id_Contramedida, Arguments.Nombre);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id_Contramedida, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                contramedida = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return contramedida;
        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {

                var accessor = new DataAccesor();
                string sql = string.Format("DELETE FROM CONTRAMEDIDA WHERE ID_Contramedida = {0}", id);

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

        public IList<Contramedida> Listar()
        {
            IList<Contramedida> lst = null;

            try
            {
                var accessor = new DataAccesor();
                var sql = string.Format(@"SELECT
                                            ID_Contramedida AS {0},
                                            NOMBRE AS {1}
                                        FROM CONTRAMEDIDA",
                                        Arguments.Id_Contramedida,
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

        public bool Modificar(Contramedida entidad)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format("UPDATE Contramedida SET NOMBRE = " + ic + "{0} WHERE ID_Contramedida = " + ic + "{1}",
                                        Arguments.Nombre, Arguments.Id_Contramedida);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                     accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                     accessor.Parameter(Arguments.Id_Contramedida, entidad.Id_Contramedida)
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

        private Contramedida GetSingle(DataSet ds)
        {
            Contramedida contramedida = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    contramedida = new Contramedida()
                    {
                        Id_Contramedida = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Contramedida]),
                        Nombre = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Nombre])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return contramedida;
        }

        private IList<Contramedida> GetCollection(DataSet ds)
        {
            IList<Contramedida> contramedidas = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    contramedidas = new List<Contramedida>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        contramedidas.Add(new Contramedida
                        {
                            Id_Contramedida = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id_Contramedida]),
                            Nombre = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Nombre])
                          
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return contramedidas;
        }
        #endregion
    }
}
