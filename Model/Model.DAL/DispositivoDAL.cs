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
    public class DispositivoDAL : IDispositivo
    {
        ILogger log = LogFactory.GetLogger(typeof(DispositivoDAL));
        private string _connectionString;

        public DispositivoDAL(string connectionString=null)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Dispositivo entidad)
        {
            int id = -1;
            try
            {

                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO DISPOSITIVO (SERIAL_NUMBER, IP, DESCRIPCION)" +
                                       " VALUES (" + ic + "{0}, " + ic + "{1}, " + ic + "{2}) " + accessor.sqlGetNewIdentity(Arguments.Id_Dispositivo, "{3}"),
                                       Arguments.Serial_Number, Arguments.IP, Arguments.Descripcion, Arguments.Id_Dispositivo);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Serial_Number, entidad.Serial_Number),
                    accessor.Parameter(Arguments.IP, entidad.IP),
                    accessor.Parameter(Arguments.Descripcion, entidad.Descripcion),
                    accessor.Parameter(Arguments.Id_Dispositivo, 0, ParameterDirection.Output)
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

        public Dispositivo Detalles(int id)
        {
            Dispositivo dispositivo = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_Dispositivo AS {0},
                                            Serial_Number AS {1},
                                            IP AS {2},
                                            Descripcion AS {3}
                                        FROM DISPOSITIVO
                                        WHERE ID_Dispositivo = " + ic + "{0}",
                                        Arguments.Id_Dispositivo, Arguments.Serial_Number, 
                                        Arguments.IP,Arguments.Descripcion);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id_Dispositivo, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                dispositivo = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return dispositivo;
        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {

                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string sql = string.Format("DELETE FROM DISPOSITIVO WHERE ID_DISPOSITIVO = {0}", id);

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

        public IList<Dispositivo> Listar()
        {
            IList<Dispositivo> lst = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                var sql = string.Format(@"SELECT
                                            ID_DISPOSITIVO AS {0},
                                            SERIAL_NUMBER AS {1},
                                            IP AS {2},
                                            DESCRIPCION AS {3}
                                        FROM DISPOSITIVO",
                                        Arguments.Id_Dispositivo,
                                        Arguments.Serial_Number, Arguments.IP, Arguments.Descripcion);

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

        public bool Modificar(Dispositivo entidad)
        {
            bool sw = false;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format("UPDATE Dispositivo SET Serial_Number = " + ic + "{0}, " +
                                        " IP = " + ic + "{1}, Descripcion= " + ic + "{2} WHERE ID_DISPOSITIVO = " + ic + "{3}",
                                        Arguments.Serial_Number, Arguments.IP, Arguments.Descripcion, Arguments.Id_Dispositivo);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                     accessor.Parameter(Arguments.Serial_Number, entidad.Serial_Number),
                     accessor.Parameter(Arguments.IP, Convert.ToString(entidad.IP)),
                     accessor.Parameter(Arguments.Descripcion, Convert.ToString(entidad.Descripcion)),
                     accessor.Parameter(Arguments.Id_Dispositivo, entidad.Id_Disposito)
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

        public Dispositivo GetDetailsByIp(string ip)
        {
            Dispositivo dispositivo = null;

            try
            {
                var accessor = !string.IsNullOrEmpty(_connectionString) ? new DataAccesor(_connectionString) : new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_Dispositivo AS {0},
                                            Serial_Number AS {1},
                                            IP AS {2},
                                            Descripcion AS {3}
                                        FROM DISPOSITIVO
                                        WHERE IP = " + ic + "{2}",
                                        Arguments.Id_Dispositivo, Arguments.Serial_Number,
                                        Arguments.IP, Arguments.Descripcion);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.IP, ip)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                dispositivo = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("GetDetailsByIp({0})", !string.IsNullOrEmpty(ip) ? ip : "null", ex);
            }
            return dispositivo;
        }


        #region "Utilities"

        private Dispositivo GetSingle(DataSet ds)
        {
            Dispositivo dispositivo = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dispositivo = new Dispositivo()
                    {
                        Id_Disposito = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Dispositivo]),
                        Serial_Number = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Serial_Number]),
                        IP = Convert.ToString(ds.Tables[0].Rows[0][Arguments.IP]),
                        Descripcion= Convert.ToString(ds.Tables[0].Rows[0][Arguments.Descripcion])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return dispositivo;
        }

        private IList<Dispositivo> GetCollection(DataSet ds)
        {
            IList<Dispositivo> dispositivos = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dispositivos = new List<Dispositivo>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dispositivos.Add(new Dispositivo
                        {
                            Id_Disposito = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id_Dispositivo]),
                            Serial_Number = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Serial_Number]),
                            IP = Convert.ToString(ds.Tables[0].Rows[i][Arguments.IP]),
                            Descripcion = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Descripcion])

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return dispositivos;
        }
        #endregion
    }
}
