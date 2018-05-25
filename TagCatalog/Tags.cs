using LoggerManager;
using Model.BL;
using Model.BL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagCatalog.DTO;


namespace TagCatalog
{
    public class Tags
    {
        ILogger log = LogFactory.GetLogger(typeof(Tags));

        public Dictionary<int, TagInfo> _caracteristicasTags { get; set; }  // Para búsqueda por Id_Tag

        public Tags()
        {
            LoadTagCatalog();
        }


        /// <summary>
        /// Añade un tag al catálogo
        /// </summary>
        public bool AddTag(Tag tag)
        {
            bool sw = false;

            try
            {
                TagInfo tagBase = ConvertToTagInfo(tag);

                if (tagBase != null)
                {
                    if (!_caracteristicasTags.ContainsKey(tagBase.Id_Tag))
                    {
                        _caracteristicasTags.Add(tagBase.Id_Tag, tagBase);
                    }
                    sw = true;
                }
            }
            catch (Exception er)
            {
                log.Error("AddTag", er);
            }

            return sw;
        }

        private bool LoadTagCatalog()
        {
            bool sw = false;

            try
            {
                log.Debug("Cargando catálogo de tags");

                this._caracteristicasTags = new Dictionary<int, TagInfo>();
                List<TagInfo> lstTags = CargarCatalogoTags();

                if (lstTags != null)
                {
                    foreach (var tag in lstTags)
                    {
                        // Agregamos la configuración de cada tag en el diccionario
                        _caracteristicasTags.Add(tag.Id_Tag, tag);

                    }
                    sw = true;
                }
                else
                {
                    log.Warning("LoadTagCatalog(). No se pudo cargar el catálogo de tags");
                }
            }
            catch (Exception er)
            {
                log.Error("LoadTagCatalog()", er);
            }

            return sw;
        }

        private List<TagInfo> CargarCatalogoTags()
        {
            List<TagInfo> lstTags = new List<TagInfo>();

            try
            {
                Model.BL.Tags model = new Model.BL.Tags();
                //var tags = model.GetTagsByService(Id_Service);

                //if (tags != null)
                //{
                //    lstTags = ConvertToTagBase(tags);
                //}
            }
            catch (Exception er)
            {
                log.Error("CargarCatalogoTags()", er);
            }

            return lstTags;
        }

        private TagInfo ConvertToTagInfo(Tag tag)
        {
            TagInfo result = null;

            try
            {
                if (tag != null)
                {
                    // Carga las características y campos específicos para cada tipo de datos
                    var tagToResult = new TagInfo
                    {
                        Id_Tag = tag.Id,
                        TagName = tag.Descripcion

                    };
                    result = tagToResult;
                   
                }
            }
            catch (Exception er)
            {
                log.Error("ConvertToTagBase (Single)", er);
            }

            return result;
        }

        private List<TagInfo> ConvertToTagInfo(List<Tag> tags)
        {
            List<TagInfo> result = null;

            try
            {
                if (tags != null)
                {
                    result = new List<TagInfo>();

                    foreach (var item in tags)
                    {
                        var tag = ConvertToTagInfo(item);

                        if (tag != null)
                        {
                            result.Add(tag);
                        }
                    }
                }
            }
            catch (Exception er)
            {
                log.Error("ConvertToTagBase()", er);
            }
            return result;
        }
    }
}
