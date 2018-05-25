using LoggerManager;
using Model.DAL.Common;
using Model.DAL.Database;
using Model.DAL.DTO;
using Model.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.DAL
{
    public class TagsDAL : ITags
    {
        ILogger log = LogFactory.GetLogger(typeof(TagsDAL));

        public int Agregar(Tag tag)
        {
            int id = -1;
            
            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"INSERT INTO TAGS (DESCRIPCION, ID_PROVEEDOR)
                                                            VALUES (" + ic + "{0}, " + ic + "{1}) " + accessor.sqlGetNewIdentity(Arguments.Id, "{2}"),
                                            Arguments.Descripcion,
                                            Arguments.Id_Proveedor, Arguments.Id);

              

                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                   
                    accessor.Parameter(Arguments.Descripcion, tag.Descripcion),
                    accessor.Parameter(Arguments.Id_Proveedor, tag.Id_Proveedor),                  
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

        public Tag Detalles(int id)
        {
            Tag tag = null;

            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT ID AS {0},
                                                            DESCRIPCION AS {1},
                                                            ID_PROVEEDOR AS {2}
                                                        FROM TAGS
                                                        WHERE ID_TAG = " + ic + "{0}",
                                            Arguments.Id, Arguments.Descripcion,
                                            Arguments.Id_Proveedor);

    

                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                   accessor.Parameter(Arguments.Id, id)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                tag = GetSingle(ds);
            }
            catch (Exception ex)
            {
                log.Error("Detalles({0})", id, ex);
            }
            return tag;

        }

        public bool Eliminar(int id)
        {
            bool sw = false;

            try
            {


                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();

                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                   accessor.Parameter(Arguments.Id, id)
                };

                var result = accessor.ExecuteNonQuery(String.Format("DELETE FROM TAG WHERE ID = " + ic + "{0}", Arguments.Id), parameters, false);
              

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
        

        public IList<Tag> Listar()
        {
            IList<Tag> tags = null;

            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT ID AS {0},
                                                            DESCRIPCION AS {1},
                                                            ID_PROVEEDOR AS {2}
                                                        FROM TAG",
                                            Arguments.Id, Arguments.Descripcion,
                                            Arguments.Id_Proveedor);


                var ds = accessor.FillDataSet(sql);

                tags = GetCollection(ds);
            }
            catch (Exception ex)
            {
                log.Error("Listar()", ex);
            }

            return tags;
        }

        public IList<Tag> GetTagsByProvider(int Id_TagProvider)
        {
            IList<Tag> tags = null;

            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"SELECT 
                                                TAG.ID AS {0},
                                                TAG.DESCRIPCION AS {1},
                                                TAG.ID_PROVEEDOR AS {2}
                                            FROM TAG
                                            WHERE TAGS.ID_PROVEEDOR = " + ic + "{2}",
                                            Arguments.Id, Arguments.Descripcion,
                                            Arguments.Id_Proveedor);

              

                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.Id_Proveedor, Id_TagProvider)
                };

                var ds = accessor.FillDataSet(sql, parameters);

                tags = GetCollection(ds);
            }
            catch (Exception ex)
            {
                log.Error("GetTagsByProvider()", ex);
            }

            return tags;
        }


        public bool Modificar(Tag entidad)
        {
            bool updated = false;

            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"UPDATE TAGS 
                                           SET
                                                ID = " + ic + "{1}," + 
                                                "DESCRIPCION = " + "{2}," + 
                                                "ID_PROVEEDOR = " + ic + "{3}" ,
                                            Arguments.Id,
                                            Arguments.Descripcion,
                                            Arguments.Id_Proveedor);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.IdTag, entidad.Id),                 
                    accessor.Parameter(Arguments.Id_Proveedor, entidad.Id_Proveedor),
                    accessor.Parameter(Arguments.Descripcion, entidad.Descripcion)                    
                };

                var result = accessor.ExecuteNonQuery(sql, parameters, false);

                if (typeof(int).Equals(result.GetType()))
                    updated = (int)result > 0;
                else
                    log.Warning("Modificar() No se ha completado la modificación");
            }
            catch (Exception ex)
            {
                log.Error("Modificar()", ex);
            }

            return updated;
        }

        #region Private Interface

        private IList<Tag> GetCollection(DataSet ds)
        {
            IList<Tag> tags = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    tags = new List<Tag>();

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        tags.Add(new Tag
                        {
                            Id= Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id]),
                            Descripcion = Convert.ToString(ds.Tables[0].Rows[i][Arguments.Description]),
                            Id_Proveedor = Convert.ToInt32(ds.Tables[0].Rows[i][Arguments.Id_Proveedor])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetCollection", ex);
            }

            return tags;
        }

        private Tag GetSingle(DataSet ds)
        {
            Tag tag = null;

            try
            {
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    tag = new Tag()
                    {                        
                        Id = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id]),
                        Descripcion = Convert.ToString(ds.Tables[0].Rows[0][Arguments.Descripcion]),
                        Id_Proveedor = Convert.ToInt32(ds.Tables[0].Rows[0][Arguments.Id_Proveedor])
                    };                    
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSingle", ex);
            }

            return tag;
        }

        #endregion

        public bool Vincular(int Id_Tag, int Id_Prensa)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"INSERT INTO TAG_PRENSA (ID_TAG, ID_PRENSA)
                                                            VALUES (" + ic + "{0}, " + ic + "{1})",
                                            Arguments.IdTag,
                                            Arguments.Id_Prensa);



                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                   accessor.Parameter(Arguments.IdTag, Id_Tag),
                   accessor.Parameter(Arguments.Id_Prensa, Id_Prensa)
                };


                int filasAfectadas = accessor.ExecuteNonQuery(sql, parameters, false);

                if (filasAfectadas > 0)
                    sw = true;
            }
            catch (Exception ex)
            {
                log.Error("Vincular()", ex);
            }

            return sw;
        }

        public bool Desvincular(int Id_Tag, int Id_Prensa)
        {
            bool sw = false;

            try
            {
                var accessor = new DataAccesor();
                string ic = accessor.ParameterIdentifierCharacter();
                var sql = string.Format(@"DELETE FROM TAG_PRENSA WHERE ID_TAG = " + ic + "{0} AND ID_PRENSA= " + ic + "{1}",
                        Arguments.IdTag, Arguments.Id_Prensa);


                List<IDataParameter> parameters = new List<IDataParameter>()
                {
                    accessor.Parameter(Arguments.IdTag, Id_Tag),
                    accessor.Parameter(Arguments.IdRole, Id_Prensa)
                };

                int numFilas = accessor.ExecuteNonQuery(sql, parameters, false);

                if (numFilas > 0)
                    sw = true;
            }
            catch (Exception ex)
            {
                log.Error("Desvincular()", ex);
            }

            return sw;
        }
    }
}
