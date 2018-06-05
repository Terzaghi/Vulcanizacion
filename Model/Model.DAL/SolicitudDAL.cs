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
    public class SolicitudDAL:ISolicitud
    {
        ILogger log = LogFactory.GetLogger(typeof(SolicitudDAL));
        private string _connectionString;

        public SolicitudDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Agregar(Solicitud entidad)
        {
            int id = -1;
            try
            {

                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format("INSERT INTO SOLICITUD (FECHA_GENERACION, ID_PRENSA)" +
                                       " VALUES (" + ic + "{0}, " + ic + "{1}) " + accessor.sqlGetNewIdentity(Arguments.Id, "{2}"),
                                       Arguments.Fecha_Generacion, Arguments.Id_Prensa, Arguments.Id);

                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Fecha_Generacion, entidad.Fecha_Generacion),
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

        public Solicitud Detalles(int id)
        {
            Solicitud solicitud = null;

            try
            {

                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            FECHA_GENERACION AS {1},
                                            ID_PRENSA AS {2}
                                        FROM SOLICITUD
                                        WHERE ID = " + ic + "{0}",
                                        Arguments.Id, Arguments.Fecha_Generacion, Arguments.Id_Prensa);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                solicitud = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return solicitud;
        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {

                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string sql = string.Format("DELETE FROM SOLICITUD WHERE ID = {0}", id);

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

        public IList<Solicitud> Listar()
        {
            IList<Solicitud> lst = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            FECHA_GENERACION AS {1},
                                            ID_PRENSA AS {2}
                                         FROM SOLICITUD",
                                         Arguments.Id, Arguments.Fecha_Generacion, Arguments.Id_Prensa);

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

        public bool Modificar(Solicitud entidad)
        {
            throw new NotImplementedException();
        }

        #region "Utilities"

        private Solicitud GetSingle(DataSet ds)
        {
            Solicitud solicitud = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    solicitud = new Solicitud()
                    {
                        Id = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id]),
                        Fecha_Generacion = Convert.ToDateTime(ds.Tables[0].Rows[0][Arguments.Fecha_Generacion]),
                        Id_Prensa=Convert.ToInt16(ds.Tables[0].Rows[0][Arguments.Id_Prensa])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return solicitud;
        }

        private IList<Solicitud> GetCollection(DataSet ds)
        {
            IList<Solicitud> solicitudes = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    solicitudes = new List<Solicitud>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        solicitudes.Add(new Solicitud
                        {
                            Id = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id]),
                            Fecha_Generacion = Convert.ToDateTime(ds.Tables[0].Rows[i][Arguments.Fecha_Generacion]),
                            Id_Prensa = Convert.ToInt16(ds.Tables[0].Rows[i][Arguments.Id_Prensa])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return solicitudes;
        }

        #endregion
    }
}
