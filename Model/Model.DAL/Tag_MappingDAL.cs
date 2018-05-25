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
    public class Tag_MappingDAL : ITag_Mapping
    {
        ILogger log = LogFactory.GetLogger(typeof(EstadoDAL));
        private string _connectionString;

        public Tag_MappingDAL(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Agregar(Tag_Mapping entidad)
        {
            int id = -1;
            try
            {

                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();

                var sql = string.Format("INSERT INTO TAG_MAPPING (NOMBRE)" +
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

        public Tag_Mapping Detalles(int id)
        {
            Tag_Mapping mapping = null;

            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT
                                            ID AS {0},
                                            Nombre AS {1}                                            
                                        FROM TAG_MAPPING
                                        WHERE ID= " + ic + "{0}",
                                        Arguments.Id, Arguments.Nombre);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                mapping = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", ex, id);
            }
            return mapping;
        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {

                var accessor = new DataAccesor();
                string sql = string.Format("DELETE FROM TAG_MAPPING WHERE ID = {0}", id);

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

        public IList<Tag_Mapping> Listar()
        {
            IList<Tag_Mapping> lst = null;

            try
            {
                var accessor = new DataAccesor();
                var sql = string.Format(@"SELECT
                                            ID  AS {0},
                                            NOMBRE AS {1}
                                        FROM TAG_MAPPING",
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

        public bool Modificar(Tag_Mapping entidad)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format("UPDATE TAG_MAPPING SET NOMBRE = " + ic + "{0} WHERE ID = " + ic + "{1}",
                                        Arguments.Nombre, Arguments.Id_Estado);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                     accessor.Parameter(Arguments.Nombre, entidad.Nombre),
                     accessor.Parameter(Arguments.Id_Estado, entidad.Id)
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

        private Tag_Mapping GetSingle(DataSet ds)
        {
            Tag_Mapping mapping = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    mapping = new Tag_Mapping()
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

            return mapping;
        }

        private IList<Tag_Mapping> GetCollection(DataSet ds)
        {
            IList<Tag_Mapping> mappings = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    mappings= new List<Tag_Mapping>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        mappings.Add(new Tag_Mapping
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

            return mappings;
        }
        #endregion
    }
}
