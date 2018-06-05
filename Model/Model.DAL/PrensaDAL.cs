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
    public class PrensaDAL:IPrensa
    {
        ILogger log = LogFactory.GetLogger(typeof(PrensaDAL));

        private string _connectionString;

        public PrensaDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Agregar(Prensa entidad)
        {
            int id = -1;
            try
            {

                var accessor = new DataAccesor(_connectionString);

                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO PRENSA (NOMBRE, TOPIC, BARCODE_PRENSA, BARCODE_PINTADO, BARCODE_PINCHADO, ID_ZONA)" +
                                       " VALUES (" + ic + "{0}, " + ic + "{1}," + ic + "{2}, " + ic + "{3}, " + ic + "{4}, " + ic + "{5}) " + accessor.sqlGetNewIdentity(Arguments.Id, "{6}"),
                                       Arguments.Nombre, Arguments.Topic, Arguments.Barcode_Prensa, 
                                       Arguments.Barcode_Pintado, Arguments.Barcode_Pinchado, Arguments.ID_Zona, Arguments.Id);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                    accessor.Parameter(Arguments.Barcode_Prensa, entidad.Barcode_Prensa),
                    accessor.Parameter(Arguments.Barcode_Pintado, entidad.Barcode_Pintado),
                    accessor.Parameter(Arguments.Barcode_Pintado, entidad.Barcode_Pinchado),
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

        public Prensa Detalles(int id)
        {
            Prensa prensa = null;

            try
            {

                var accessor = new DataAccesor(_connectionString);

                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            Nombre AS {1},
                                            TOPIC AS {2},
                                            Tag AS {3},
                                            Tag_CV AS {4},
                                            Tag_Temp AS {5},
                                            Tag_Ciclo AS {6},
                                            Tag_Prod AS {7},
                                            Barcode_Prensa AS {8},
                                            Barcode_Pintado AS {9},
                                            Barcode_Pinchado AS {10}
                                            Prensa_Activa AS {13},
                                            ID_Zona AS {14}
                                        FROM PRENSA
                                        WHERE ID = " + ic + "{0}",
                                        Arguments.Id, Arguments.Nombre, Arguments.Topic,
                                        Arguments.Tag, Arguments.Tag_CV,Arguments.Tag_Temp,Arguments.Tag_Ciclo,
                                        Arguments.Tag_Prod, Arguments.Barcode_Prensa, Arguments.Barcode_Pintado,
                                        Arguments.Barcode_Pinchado, Arguments.Prensa_Activa);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                prensa = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return prensa;
        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {

                var accessor = new DataAccesor(_connectionString);

                string sql = string.Format("DELETE FROM PRENSA WHERE ID = {0}", id);

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

        public IList<Prensa> Listar()
        {
            IList<Prensa> lst = null;

            try
            {
                var accessor = new DataAccesor(_connectionString);

                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            Nombre AS {1},
                                            Barcode_Prensa AS {2},
                                            Barcode_Pintado AS {3},
                                            Barcode_Pinchado AS {4},
                                            Prensa_Activa AS {5},
                                            ID_Zona AS {6}
                                        FROM PRENSA",
                                        Arguments.Id, Arguments.Nombre, 
                                        Arguments.Barcode_Prensa, Arguments.Barcode_Pintado,
                                        Arguments.Barcode_Pinchado, Arguments.Prensa_Activa, Arguments.ID_Zona);

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

   
        public bool Modificar(Prensa entidad)
        {
            throw new NotImplementedException();
        }

        #region "Utilities"

        private Prensa GetSingle(DataSet ds)
        {
            Prensa prensa = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    prensa = new Prensa()
                    {
                        Id = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id]),
                        Nombre = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Nombre]),
                        Barcode_Prensa = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Barcode_Prensa]),
                        Barcode_Pintado = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Barcode_Pintado]),
                        Barcode_Pinchado = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Barcode_Pinchado]),
                        Prensa_Activa = Convert.ToInt16(ds.Tables[0].Rows[0][Arguments.Prensa_Activa]),
                        Id_Zone = Convert.ToInt16(ds.Tables[0].Rows[0][Arguments.ID_Zona])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return prensa;
        }

        private IList<Prensa> GetCollection(DataSet ds)
        {
            IList<Prensa> prensas = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    prensas = new List<Prensa>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        prensas.Add(new Prensa
                        {
                            Id = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id]),
                            Nombre = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Nombre]),
                            Barcode_Prensa = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Barcode_Prensa]),
                            Barcode_Pintado = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Barcode_Pintado]),
                            Barcode_Pinchado = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Barcode_Pinchado]),
                            Prensa_Activa = Convert.ToInt16(ds.Tables[0].Rows[i][Arguments.Prensa_Activa]),
                            Id_Zone=Convert.ToInt16(ds.Tables[0].Rows[i][Arguments.ID_Zona])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return prensas;
        }

        #endregion
    }
}

