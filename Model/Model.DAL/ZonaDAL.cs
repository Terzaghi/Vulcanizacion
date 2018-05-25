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
    public class ZonaDAL : IZona
    {
        ILogger log = LogFactory.GetLogger(typeof(ZonaDAL));
        private string _connectionString;

        public ZonaDAL(string connectionString=null)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Zona entidad)
        {
            int id = -1;
            try
            {

                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO ZONA (NOMBRE)" +
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

        public Zona Detalles(int id)
        {
            Zona zona = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            Nombre AS {1}                                            
                                        FROM ZONA
                                        WHERE ID= " + ic + "{0}",
                                        Arguments.Id, Arguments.Nombre);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                zona = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return zona;
        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {

                var accessor = new DataAccesor();
                string sql = string.Format("DELETE FROM ZONA WHERE ID= {0}", id);

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

        public IList<Zona> Listar()
        {
            IList<Zona> lst = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                var sql = string.Format(@"SELECT
                                            ID  AS {0},
                                            NOMBRE AS {1}
                                        FROM ZONA",
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

        public bool Modificar(Zona entidad)
        {
            bool sw = false;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format("UPDATE ZONA SET NOMBRE = " + ic + "{0} WHERE ID = " + ic + "{1}",
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

        private Zona GetSingle(DataSet ds)
        {
            Zona zona = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    zona = new Zona()
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

            return zona;
        }

        private IList<Zona> GetCollection(DataSet ds)
        {
            IList<Zona> zonas = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    zonas= new List<Zona>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        zonas.Add(new Zona
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

            return zonas;
        }
        #endregion
    }
}
