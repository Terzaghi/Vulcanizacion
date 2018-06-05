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
    public class Motivo_DeshabilitacionDAL : IMotivo_Deshabilitacion
    {
        ILogger log = LogFactory.GetLogger(typeof(Motivo_DeshabilitacionDAL));
        private string _connectionString;

        public Motivo_DeshabilitacionDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Motivo_Deshabilitacion entidad)
        {
            int id = -1;
            try
            {

                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO ESTADO (NOMBRE)" +
                                       " VALUES (" + ic + "{0}) " + accessor.sqlGetNewIdentity(Arguments.ID_Motivo, "{1}"),
                                       Arguments.Nombre, Arguments.ID_Motivo);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                    accessor.Parameter(Arguments.ID_Motivo, 0, ParameterDirection.Output)
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

        public Motivo_Deshabilitacion Detalles(int id)
        {
            Motivo_Deshabilitacion motivo = null;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID_Motivo AS {0},
                                            Nombre AS {1}                                            
                                        FROM MOTIVO_DESHABILITACION
                                        WHERE ID_Motivo= " + ic + "{0}",
                                        Arguments.ID_Motivo, Arguments.Nombre);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.ID_Motivo, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                motivo = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return motivo;
        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {

                var accessor = new DataAccesor(_connectionString);
                string sql = string.Format("DELETE FROM MOTIVO_DESHABILITACION WHERE ID_MOTIVO = {0}", id);

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

        public IList<Motivo_Deshabilitacion> Listar()
        {
            IList<Motivo_Deshabilitacion> lst = null;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                var sql = string.Format(@"SELECT
                                            ID_MOTIVO  AS {0},
                                            NOMBRE AS {1}
                                        FROM MOTIVO_DESHABILITACION",
                                        Arguments.ID_Motivo,
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

        public bool Modificar(Motivo_Deshabilitacion entidad)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format("UPDATE MOTIVO_DESHABILITACION SET NOMBRE = " + ic + "{0} WHERE ID_MOTIVO = " + ic + "{1}",
                                        Arguments.Nombre, Arguments.ID_Motivo);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                     accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                     accessor.Parameter(Arguments.ID_Motivo, entidad.Id_Motivo)
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

        private Motivo_Deshabilitacion GetSingle(DataSet ds)
        {
            Motivo_Deshabilitacion motivo = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    motivo = new Motivo_Deshabilitacion()
                    {
                        Id_Motivo = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.ID_Motivo]),
                        Nombre = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Nombre])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return motivo;
        }

        private IList<Motivo_Deshabilitacion> GetCollection(DataSet ds)
        {
            IList<Motivo_Deshabilitacion> motivos = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    motivos= new List<Motivo_Deshabilitacion>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        motivos.Add(new Motivo_Deshabilitacion
                        {
                            Id_Motivo = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.ID_Motivo]),
                            Nombre = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Nombre])
                          
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return motivos;
        }
        #endregion
    }
}
