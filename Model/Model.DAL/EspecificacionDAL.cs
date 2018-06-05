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
    public class EspecificacionDAL : IEspecificacion
    {
        ILogger log = LogFactory.GetLogger(typeof(EspecificacionDAL));
        private string _connectionString;

        public EspecificacionDAL(string connectionString)
        {
            _connectionString = connectionString;
        }


        public int Agregar(Especificacion entidad)
        {
            int id = -1;
            try
            {

                var accessor =new DataAccesor(_connectionString);
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO ESPECIFICACION (CV, MINUTOS_LIMITE_VULCANIZADO)" +
                                       " VALUES (" + ic + "{0}, " + ic + "{1})", 
                                       Arguments.CV, Arguments.Minutos_Limite_Vulcanizado);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.CV, entidad.CV),
                    accessor.Parameter(Arguments.Minutos_Limite_Vulcanizado, entidad.Minutos_Limite_Vulcanizado),
                    
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

        public Especificacion Detalles(int id)
        {
            throw new NotImplementedException();
        }

        public bool Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Especificacion> Listar()
        {
            IList<Especificacion> lst = null;

            try
            {
                var accessor = new DataAccesor(_connectionString);
                var sql = string.Format(@"SELECT
                                            CV AS {0},
                                            MINUTOS_LIMITE_VULCANIZADO AS {1}
                                        FROM ESPECIFICACION",
                                        Arguments.CV,
                                        Arguments.Minutos_Limite_Vulcanizado);

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

        public bool Modificar(Especificacion entidad)
        {
            throw new NotImplementedException();
        }

        #region "Utilities"

        private Especificacion GetSingle(DataSet ds)
        {
            Especificacion especificacion = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    especificacion = new Especificacion()
                    {
                        CV = Convert.ToString(ds.Tables[0].Rows[0][Arguments.CV]),
                        Minutos_Limite_Vulcanizado = Convert.ToInt16(ds.Tables[0].Rows[0][Arguments.Minutos_Limite_Vulcanizado])
                    };
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return especificacion;
        }

        private IList<Especificacion> GetCollection(DataSet ds)
        {
            IList<Especificacion> especificaciones = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    especificaciones = new List<Especificacion>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        especificaciones.Add(new Especificacion
                        {
                            CV = Convert.ToString(ds.Tables[0].Rows[i][Arguments.CV]),
                            Minutos_Limite_Vulcanizado = Convert.ToInt16(ds.Tables[0].Rows[i][Arguments.Minutos_Limite_Vulcanizado])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return especificaciones;
        }
        #endregion


    }
}
