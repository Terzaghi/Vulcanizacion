using LoggerManager;
using Memory.Common;
using System;
using System.Collections.Generic;

namespace DataProvider.TManager.Utils
{
    internal static class Converter
    {
        public static Dictionary<string, TagValue> ConvertToTagValueDictionary(Model.BL.DTO.PrensaDato registro)
        {
            Dictionary<string, TagValue> result = null;

            try
            {
                if (registro != null)
                {
                    result = new Dictionary<string, TagValue>();

                    result.Add(Memory.Common.Utils.Helper.ToKey(registro.PrensaId, TagType.Activa), 
                        ConvertToTagValue(registro.PrensaId, TagType.Activa, registro.Fecha, registro.TagActivaValue));

                    result.Add(Memory.Common.Utils.Helper.ToKey(registro.PrensaId, TagType.CV),
                        ConvertToTagValue(registro.PrensaId, TagType.CV, registro.Fecha, registro.TagCVValue));

                    result.Add(Memory.Common.Utils.Helper.ToKey(registro.PrensaId, TagType.Temp),
                        ConvertToTagValue(registro.PrensaId, TagType.Temp, registro.Fecha, registro.TagTempValue));

                    result.Add(Memory.Common.Utils.Helper.ToKey(registro.PrensaId, TagType.Prod),
                        ConvertToTagValue(registro.PrensaId, TagType.Prod, registro.Fecha, registro.TagProdValue));

                    result.Add(Memory.Common.Utils.Helper.ToKey(registro.PrensaId, TagType.Ciclo),
                        ConvertToTagValue(registro.PrensaId, TagType.Ciclo, registro.Fecha, registro.TagCicloValue));

                    result.Add(Memory.Common.Utils.Helper.ToKey(registro.PrensaId, TagType.Cavidad),
                        ConvertToTagValue(registro.PrensaId, TagType.Cavidad, registro.Fecha, registro.Cavidad));
                }
            }
            catch (Exception ex)
            {
                ILogger log = LogFactory.GetLogger(typeof(Converter));
                log.Error("ConvertToTagValueDictionary()", ex);
            }

            return result;

            
        }

        public static TagValue ConvertToTagValue(int Id_Prensa, TagType Type, DateTime Date, string Value)
        {
            return new TagValue()
            {
                Id_Prensa = Id_Prensa,
                Type = Type,
                Date = Date,
                Value = Value
            };
        }

        //public static ValoresPrensa ConvertToValoresPrensa(PrensaDato registro)
        //{
        //    ValoresPrensa result = null;

        //    try
        //    {
        //        if (registro != null && registro.PrensaId > 0)
        //        {
        //            result = new ValoresPrensa
        //            {
        //                Id_Prensa = registro.PrensaId,
        //                Valores = new Dictionary<TagType, TagValue>()
        //            };
                    
        //            result.Valores.Add(TagType.Activa, ConvertToTagValue(registro.PrensaId, TagType.Activa, registro.Fecha, registro.TagActivaValue));
        //            result.Valores.Add(TagType.CV, ConvertToTagValue(registro.PrensaId, TagType.CV, registro.Fecha, registro.TagCVValue));
        //            result.Valores.Add(TagType.Temp, ConvertToTagValue(registro.PrensaId, TagType.Temp, registro.Fecha, registro.TagTempValue));
        //            result.Valores.Add(TagType.Prod, ConvertToTagValue(registro.PrensaId, TagType.Prod, registro.Fecha, registro.TagProdValue));
        //            result.Valores.Add(TagType.Ciclo, ConvertToTagValue(registro.PrensaId, TagType.Ciclo, registro.Fecha, registro.TagCicloValue));
        //            result.Valores.Add(TagType.Cavidad, ConvertToTagValue(registro.PrensaId, TagType.Cavidad, registro.Fecha, registro.Cavidad));

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ILogger log = BSHP.LoggerManager.LogFactory.GetLogger(typeof(Converter));
        //        log.Error("ConvertToValoresPrensa()", ex);
        //    }

        //    return result;
        //}
    }
}
