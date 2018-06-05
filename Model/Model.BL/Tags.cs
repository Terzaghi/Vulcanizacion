using Common.Cache;
using Common.Enums;
using Common.Security;
using LoggerManager;
using Model.BL.DTO;
using Model.BL.Utils;
using Model.DAL;
using System;
using System.Collections.Generic;

namespace Model.BL
{
    public class Tags
    {
        ILogger log = LogFactory.GetLogger(typeof(Tags));
        private string _connectionString;

        public Tags(string connectionString)
        {
            _connectionString = connectionString;
        }
        #region Securización
        public Token SecurityToken { get; set; }

        public Tags(Token token = null)
        {
            this.SecurityToken = token;
        }
        #endregion

        
        public int Agregar(Tag tag)
        {
            int id = -1;

            try
            {
                DAL.DTO.Tag tagDal = Converter.ConvertToDAL(tag);

                TagsDAL mod = new TagsDAL(_connectionString);
                id = mod.Agregar(tagDal);                

            }
            catch (Exception er)
            {
                log.Error("Agregar()", er);
            }

            return id;
        }

        public bool Modificar(Tag tag)
        {
            bool result = false;

            try
            {
                            

                DAL.DTO.Tag tagDal = Converter.ConvertToDAL(tag);

                TagsDAL mod = new TagsDAL(_connectionString);
                result = mod.Modificar(tagDal);

            }
            catch (Exception er)
            {
                log.Error("Modificar()", er);
                result = false;
            }

            return result;

        }

        public bool Eliminar(int id)
        {
            bool result = false;

            try
            {
                TagsDAL mod = new TagsDAL(_connectionString);
                result = mod.Eliminar(id);
            }
            catch (Exception er)
            {
                log.Error(string.Format("Eliminar({0})", id), er);
                result = false;
            }

            return result;
        }

        public Tag Detalles(int id)
        {
            Tag result = null;

            try
            {
                string nombreCache = string.Format("tag{0}", id);

                if (!CacheData.Exist(nombreCache))
                {
                    TagsDAL mod = new TagsDAL(_connectionString);
                    var tag = mod.Detalles(id);

                    result = Converter.ConvertToBL(tag);


                    //result = Diccionario_Get(id);

                    if (result != null)
                        // Guardamos en caché para la próxima
                        CacheData.Add(nombreCache, result);
                }
                else
                {
                    result = (Tag)CacheData.Get(nombreCache);
                }
            }
            catch (Exception er)
            {
                log.Error(string.Format("Detalles({0})", id), er);
            }

            return result;
        }

   

        public List<Tag> GetTagsByProvider(int Id_Provider)
        {
            List<DTO.Tag> result = null;

            try
            {
                TagsDAL tagModel = new TagsDAL(_connectionString);

                var tags = tagModel.GetTagsByProvider(Id_Provider) as List<DAL.DTO.Tag>;
                result = Converter.ConvertToBL(tags);

                
            }
            catch (Exception ex)
            {
                log.Error("GetTagsByProvider()", ex);
            }
            return result;
        }

        public List<Tag> Listar()
        {
            List<Tag> result = null;

            try
            {
                TagsDAL tagModel = new TagsDAL(_connectionString);

                var tags = tagModel.Listar() as List<DAL.DTO.Tag>;
                result = Converter.ConvertToBL(tags);

                // TODO: El listado de datos hay que ponerlo que cargue de la caché, y revisar los metodos agregar, etc, de momento para esta versión no da tiempo
                //Diccionario_Agregar(result);
            }
            catch (Exception ex)
            {
                log.Error("Listar()", ex);
            }
            return result;
        }

       

        #region Diccionario de tags

        //private const string nombreDiccionario = "diccionarioTags";

        //private void Diccionario_Agregar(List<Tag> lstTags)
        //{
        //    if (!CacheData.Exist(nombreDiccionario))
        //    {
        //        Dictionary<int, Tag> dicTags = new Dictionary<int, Tag>();
        //        foreach (var tag in lstTags)
        //        {
        //            dicTags.Add(tag.Id_Tag, tag);
        //        }
        //        CacheData.Add(nombreDiccionario, dicTags);
        //    }
        //}

        //private void Diccionario_Clear()
        //{
        //    if (CacheData.Exist(nombreDiccionario))
        //    {
        //        CacheData.Remove(nombreDiccionario);
        //    }
        //}

        //private Tag Diccionario_Get(int Id_Tag)
        //{
        //    Tag result = null;

        //    try
        //    {
        //        if (!CacheData.Exist(nombreDiccionario))
        //        {
        //            Listar();
        //        }

        //        if (CacheData.Exist(nombreDiccionario))
        //        {
        //            var dictionary = (Dictionary<int, Tag>)CacheData.Get(nombreDiccionario);

        //            if (dictionary.ContainsKey(Id_Tag))
        //            {
        //                result = dictionary[Id_Tag];
        //            }
        //        }
        //    }
        //    catch (Exception er)
        //    {
        //        log.Error("Diccionario_Get", er);
        //    }

        //    return result;
        //}
        #endregion

        #region Private Interface

        

       

        #endregion

       
    }
}
