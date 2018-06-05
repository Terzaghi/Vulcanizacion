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
    public class EventoDAL : IEvento
    {
        ILogger log = LogFactory.GetLogger(typeof(EventoDAL));
        private string _connectionString;

        public EventoDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Evento entidad)
        {
            int id = -1;
            try
            {

                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO EVENTO (NOMBRE)" +
                                       " VALUES (" + ic + "{0}) " + accessor.sqlGetNewIdentity(Arguments.Id_Evento, "{1}"),
                                       Arguments.Nombre, Arguments.Id_Evento);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                    accessor.Parameter(Arguments.Id_Evento, 0, ParameterDirection.Output)
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

        public Evento Detalles(int id)
        {
            Evento evento = null;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_EVENTO AS {0},
                                            Nombre AS {1}                                            
                                        FROM EVENTO
                                        WHERE ID_Evento= " + ic + "{0}",
                                        Arguments.Id_Evento, Arguments.Nombre);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id_Evento, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                evento = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return evento;
        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {

                var accessor = new DataAccesor(_connectionString);
                string sql = string.Format("DELETE FROM EVENTO WHERE ID_EVENTO = {0}", id);

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

        public IList<Evento> Listar()
        {
            IList<Evento> lst = null;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                var sql = string.Format(@"SELECT
                                            ID_EVENTO  AS {0},
                                            NOMBRE AS {1}
                                        FROM EVENTO",
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

        public bool Modificar(Evento entidad)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format("UPDATE EVENTO SET NOMBRE = " + ic + "{0} WHERE ID_EVENTO = " + ic + "{1}",
                                        Arguments.Nombre, Arguments.Id_Estado);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                     accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                     accessor.Parameter(Arguments.Id_Estado, entidad.Id_Evento)
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

        private Evento GetSingle(DataSet ds)
        {
            Evento evento = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    evento = new Evento()
                    {
                        Id_Evento = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Evento]),
                        Nombre = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Nombre])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return evento;
        }

        private IList<Evento> GetCollection(DataSet ds)
        {
            IList<Evento> eventos = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    eventos= new List<Evento>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        eventos.Add(new Evento
                        {
                            Id_Evento = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id_Evento]),
                            Nombre = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Nombre])
                          
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return eventos;
        }
        #endregion
    }
}
