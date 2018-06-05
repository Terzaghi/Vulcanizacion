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
    public class Siguiente_CubiertaDAL : ISiguienteCubierta
    {
        ILogger log = LogFactory.GetLogger(typeof(PermisoDAL));
        private string _connectionString;

        public Siguiente_CubiertaDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Siguiente_Cubierta entidad)
        {
            int id = -1;
            try
            {

                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO SIGUIENTE_CUBIERTA (ID_PRENSA, FECHA_CHEQUEO, BARCODE_CUBIERTA, CV)" +
                                       " VALUES (" + ic + "{0}, " + ic + "{1}, " + ic + "{2}, " + ic + "{3}) ",
                                       Arguments.Id_Prensa, Arguments.Fecha_Chequeo, Arguments.Barcode_Cubierta, Arguments.CV);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id_Prensa, entidad.Id_Prensa),
                    accessor.Parameter(Arguments.Fecha_Chequeo, entidad.Fecha_Chequeo),
                    accessor.Parameter(Arguments.Barcode_Cubierta, entidad.Barcode_Cubierta),
                    accessor.Parameter(Arguments.CV, entidad.CV)
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

        public Siguiente_Cubierta Detalles(int id)
        {
            throw new NotImplementedException();
        }
        public bool Eliminar(int id_prensa)
        {
            throw new NotImplementedException();
        }
        public bool Eliminar(int id_prensa, string cv)
        {
            bool sw = false;

            try
            {

                var accessor = new DataAccesor(_connectionString);
                string sql = string.Format("DELETE FROM SIGUIENTE_CUBIERTA WHERE ID_PRENSA = {0} AND CV='{1}'", id_prensa, cv);

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

        public IList<Siguiente_Cubierta> Listar()
        {
            IList<Siguiente_Cubierta> lst = null;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                var sql = string.Format(@"SELECT
                                            ID_PRENSA  AS {0},
                                            FECHA_CHEQUEO AS {1},
                                            BARCODE_CUBIERTA AS {2},
                                            CV AS {3}
                                        FROM SIGUIENTE_CUBIERTA",
                                        Arguments.Id_Prensa,
                                        Arguments.Fecha_Chequeo, Arguments.Barcode_Cubierta, Arguments.CV);

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

        public IList<Siguiente_Cubierta> Listar(string cv)
        {
            IList<Siguiente_Cubierta> lst = null;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_PRENSA  AS {0},
                                            FECHA_CHEQUEO AS {1},
                                            BARCODE_CUBIERTA AS {2},
                                            CV AS {3}
                                        FROM SIGUIENTE_CUBIERTA
                                        WHERE CV=" + ic + "{3}",
                                        Arguments.Id_Prensa,
                                        Arguments.Fecha_Chequeo, Arguments.Barcode_Cubierta, Arguments.CV);

                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.CV, cv)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                lst = GetCollection(ds);
            }
            catch (Exception ex)
            {
                log.Error("Listar()", ex);
            }
            return lst;
        }
        public bool Modificar(Siguiente_Cubierta entidad)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format("UPDATE SIGUIENTE_CUBIERTA SET ID_PRENSA = " + ic + "{0}," +
                    " FECHA_CHEQUEO= " + ic + "{1}, BARCODE_CUBIERTA=" + ic + "{2}, CV=" + ic + "{3} " +
                    " WHERE ID_PRENSA = " + ic + "{0} AND CV=" + ic + "{3}",
                                        Arguments.Id_Prensa, Arguments.Fecha_Chequeo, Arguments.Barcode_Cubierta, Arguments.CV);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                     accessor.Parameter(Arguments.Id_Prensa, entidad.Id_Prensa),
                     accessor.Parameter(Arguments.Fecha_Chequeo, entidad.Fecha_Chequeo),
                     accessor.Parameter(Arguments.Barcode_Cubierta, entidad.Barcode_Cubierta),
                     accessor.Parameter(Arguments.CV, entidad.CV)
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

        private Siguiente_Cubierta GetSingle(DataSet ds)
        {
            Siguiente_Cubierta siguiente_Cubierta = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    siguiente_Cubierta = new Siguiente_Cubierta()
                    {
                        Id_Prensa = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Prensa]),
                        Fecha_Chequeo = Convert.ToDateTime(ds.Tables[0].Rows[0][Arguments.Fecha_Chequeo]),
                        Barcode_Cubierta = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Barcode_Cubierta]),
                        CV = Convert.ToString(ds.Tables[0].Rows[0][Arguments.CV])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return siguiente_Cubierta;
        }

        private IList<Siguiente_Cubierta> GetCollection(DataSet ds)
        {
            IList<Siguiente_Cubierta> siguientes_Cubiertas = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    siguientes_Cubiertas = new List<Siguiente_Cubierta>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        siguientes_Cubiertas.Add(new Siguiente_Cubierta
                        {
                            Id_Prensa = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id_Prensa]),
                            Fecha_Chequeo = Convert.ToDateTime(ds.Tables[0].Rows[i][Arguments.Fecha_Chequeo]),
                            Barcode_Cubierta= Convert.ToString(ds.Tables[0].Rows[i][Arguments.Barcode_Cubierta]),
                            CV= Convert.ToString(ds.Tables[0].Rows[i][Arguments.CV])

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return siguientes_Cubiertas;
        }
        #endregion

    }
}
