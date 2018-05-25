
using LoggerManager;
using Memory.Common;
using Memory.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ValuesMemory
{
    public class MemoryValues : IXmlSerializable
    {
        ILogger log = LogFactory.GetLogger(typeof(MemoryValues));

        private Dictionary<string, TagValue> _valoresPrensa { get; set; }

        public MemoryValues()
        {
            log.Debug("Inicializando diccionario que almacenará los valores de los tags recibidos");
            this._valoresPrensa = new Dictionary<string, TagValue>();
        }

        #region Inicialización y carga de valores

        public void LoadValues(Dictionary<string, TagValue> valores)
        {
            try
            {
                foreach (KeyValuePair<string, TagValue> registro in valores)
                {
                    if (!_valoresPrensa.ContainsKey(registro.Key))
                        this._valoresPrensa.Add(registro.Key, registro.Value);
                }
            }
            catch (Exception er)
            {
                log.Error("LoadValues()", er);
            }
        }

        public Dictionary<string, TagValue> GetAll()
        {
            return this._valoresPrensa;
        }

        #endregion

        #region Devuelve el valor de un tag
        
        public bool ContainsKey(string Key)
        {
            if (!string.IsNullOrEmpty(Key))
                return this._valoresPrensa.ContainsKey(Key);
            else
                return false;
        }

        /// <summary>
        /// Devuelve el último valor que ha recibido el tag de la prensa
        /// </summary>
        public TagValue GetTagValue(int Id_Prensa, TagType Type)
        {
            TagValue result = null;

            var key = Helper.ToKey(Id_Prensa, Type);

            if (!string.IsNullOrEmpty(key))
            {
                this._valoresPrensa.TryGetValue(key, out result);
            }

            return result;
        }

        #endregion

        #region Asignar valor

        /// <summary>
        /// Almacena el valor de una tag en memoria
        /// </summary>>
        /// <returns></returns>
        public bool AddValue(int Id_Prensa, TagType Type, string Value, DateTime Moment)
        {
            bool sw = false;

            try
            {
                var key = Helper.ToKey(Id_Prensa, Type);

                if (!string.IsNullOrEmpty(key))
                {
                    if (this._valoresPrensa.ContainsKey(key))
                    {
                        TagValue valor;

                        if (this._valoresPrensa.TryGetValue(key, out valor))
                        {
                            if (valor == null)
                            {
                                //No hay ningún valor asociado, le añadimos
                                valor = CreateTagValue(Id_Prensa, Type, Value, Moment);

                                sw = true;
                            }
                            else
                            {
                                // La señal ya contiene un valor, lo actualizamos
                                valor.Date = Moment;
                                valor.Value = Value;

                                sw = true;
                            }
                        }
                    }
                    else
                    {
                        // De esta señal nunca se había recibido un valor, lo agregamos
                        var valor = CreateTagValue(Id_Prensa, Type, Value, Moment);

                        this._valoresPrensa.Add(key, valor);

                        sw = true;
                    }

#if DEBUG
                    // En el caso de estar compilado como debug para desarrollo agregamos traza de log
                    log.Debug("Almacenando valor en colas de memoria. Prensa: {0}, Tipo: {1}, Insertado: {2}", Id_Prensa, Type.ToString(), sw);
#endif
                }
            }
            catch (Exception er)
            {
                log.Error("AddValue()", er);
            }

            return sw;
        }

        public bool AddValue(int Id_Prensa, TagType Type, string Value)
        {
            return AddValue(Id_Prensa, Type, Value, DateTime.Now);
        }

        #endregion

        #region Interfaz IXmlSerializable

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            try
            {
                // Iniciamos la variable
                this._valoresPrensa = new Dictionary<string, TagValue>();

                reader.Read();
                reader.ReadStartElement("dictionary");
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    string strJson = reader.ReadElementString("item");

                    TagValue result = Newtonsoft.Json.JsonConvert.DeserializeObject<TagValue>(strJson, new Newtonsoft.Json.JsonSerializerSettings
                    {
                        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
                    });

                    reader.MoveToContent();

                    // Agregamos el valor
                    this._valoresPrensa.Add(result.ToKey(), result);
                }
                reader.ReadEndElement();
            }
            catch (Exception er)
            {
                log.Error("ReadXml()", er);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            try
            {
                // Serializamos el diccionario de valores (propiedad privada)      
                writer.WriteStartElement("dictionary");

                foreach (KeyValuePair<string, TagValue> entry in this._valoresPrensa)
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

        #region Interfaz Privada
        
        private TagValue CreateTagValue(int Id_Prensa, TagType Type, string Value, DateTime Moment)
        {
            return new TagValue
            {
                Id_Prensa = Id_Prensa,
                Type = Type,
                Date = Moment,
                Value = Value
            };
        }

        #endregion
    }
}
