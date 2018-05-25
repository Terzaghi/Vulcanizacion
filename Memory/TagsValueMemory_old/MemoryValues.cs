
using Memory.Common.DTO;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using LoggerManager;

namespace TagsValueMemory
{
    public class MemoryValues: IXmlSerializable
    {
        ILogger log = LogFactory.GetLogger(typeof(MemoryValues));        

        private Dictionary<int, TagValue> _valoresTags { get; set; }

        public MemoryValues()
        {
            log.Debug("Inicializando diccionario que almacenará los valores de los tags recibidos");
            this._valoresTags = new Dictionary<int, TagValue>();
        }

        #region Inicialización y carga de valores
        public void LoadValues(Dictionary<int, TagValue> valores)
        {
            try
            {
                foreach (KeyValuePair<int, TagValue> registro in this._valoresTags)
                {
                    this._valoresTags.Add(registro.Key, registro.Value);
                }
            }
            catch (Exception er)
            {
                log.Error("LoadValues()", er);
            }
        }

        public Dictionary<int, TagValue> GetAll()
        {
            return this._valoresTags;
        }
        #endregion

        #region Devuelve el valor de un tag
        /// <summary>
        /// Devuelve el último valor que ha recivido el tag
        /// </summary>
        /// <param name="id_Tag"></param>
        public TagValue GetValue(int id_Tag)
        {
            TagValue valorTag;

            if (!this._valoresTags.TryGetValue(id_Tag, out valorTag))
            {
                // La señal no contiene valor 
                valorTag = null;
            }

            return valorTag;
        }             
        #endregion

        #region Asignar valor
        /// <summary>
        /// Almacena el valor de una tag en memoria
        /// </summary>
        /// <param name="id_Tag"></param>
        /// <param name="value"></param>
        /// <param name="moment"></param>
        /// <returns></returns>
        public bool AddValue(int id_Tag, object value, DateTime moment)
        {
            bool sw = false;

            try
            {
                if (this._valoresTags.ContainsKey(id_Tag))
                {
                    // La señal ya contiene un valor, lo actualizamos
                    TagValue valorTag;
                    if (this._valoresTags.TryGetValue(id_Tag, out valorTag))
                    {
                        // Actualizamos los valores usando el puntero del diccionario
                        valorTag.Date = moment;
                        valorTag.Value = value;

                        sw = true;
                    }
                }
                else
                {
                    // De esta señal nunca se había recivido un valor, lo agregamos
                    this._valoresTags.Add(id_Tag, new TagValue()
                    {
                        Id_Tag = id_Tag,
                        Value = value,
                        Date = moment
                    });

                    sw = true;
                }

#if DEBUG
                // En el caso de estar compilado como debug para desarrollo agregamos traza de log
                log.Debug("Almacenando valor en colas de memoria. Tag: {0}, Valor: {1}, Insertado: {2}", id_Tag, value, sw);
#endif                
            }
            catch (Exception er)
            {
                log.Error("AddValue()", er);
            }

            return sw;
        }

        public bool AddValue(int id_Tag, object value)
        {
            return AddValue(id_Tag, value, DateTime.Now);
        }
        #endregion

        #region IXmlSerializable
        // Dado que almacena distintos tipos de valores para las distintas señales (object), creamos un serializador propio
        // que incluye la definición de los tipos de datos que almacena
        public XmlSchema GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            try
            {
                // Iniciamos la variable
                this._valoresTags = new Dictionary<int, TagValue>();

                reader.Read();
                reader.ReadStartElement("dictionary");
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    string strJson = reader.ReadElementString("item");

                    TagValue result = Newtonsoft.Json.JsonConvert.DeserializeObject<TagValue>(strJson, new Newtonsoft.Json.JsonSerializerSettings
                    {
                        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
                    });

                    //reader.ReadEndElement();
                    reader.MoveToContent();

                    // Agregamos el valor
                    this._valoresTags.Add(result.Id_Tag, result);
                }
                reader.ReadEndElement();
            }
            catch (Exception er)
            {
                log.Error("ReadXml()", er);
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            try
            {
                // Serializamos el diccionario de valores (propiedad privada)      
                writer.WriteStartElement("dictionary");
                foreach (KeyValuePair<int, TagValue> entry in this._valoresTags)
                {
                    writer.WriteStartElement("item");

                    string result = Newtonsoft.Json.JsonConvert.SerializeObject(entry.Value, new Newtonsoft.Json.JsonSerializerSettings
                    {
                        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
                    });

                    writer.WriteString(result);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            catch (Exception er)
            {
                log.Error("WriteXml()", er);
            }
        }        
        #endregion

    }
}
