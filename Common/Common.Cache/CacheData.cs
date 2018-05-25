using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using LoggerManager;

namespace Common.Cache
{
    public static class CacheData
    {
        /// <summary>
        /// Configura la duración de la caché. Definida por el sistema o expiración tras 30 minutos
        /// </summary>
        public enum Duration
        {
            Expire,
            NoExpire 
        }

        /// <summary>
        /// Almacena un objeto en la caché
        /// Utiliza la expiración por el propio servidor, sin forzar expiración por tiempo
        /// </summary>
        /// <param name="nombreCache"></param>
        /// <param name="datos"></param>
        public static void Add(string nombreCache, object datos)
        {
            Add(nombreCache, datos, Duration.NoExpire);
        }

        /// <summary>
        /// Almacena un objeto en caché especificando si será permanente o tendrá una duración de 30 minutos
        /// </summary>
        /// <param name="nombreCache"></param>
        /// <param name="datos"></param>
        /// <param name="duracion"></param>
        /// <returns></returns>
        public static bool Add(string nombreCache, object datos, Duration duracion)
        {
            bool sw = false;

            try
            {
                ObjectCache cache = MemoryCache.Default;                

                if (cache.Contains(nombreCache))
                {
                    // La caché ya existe, la eliminamos primero                    
                    ILogger log = LogFactory.GetLogger(typeof(CacheData));
                    log.Warning("Add({0}, obj, {1}). Esta caché ya existía. Reemplazando.", nombreCache, duracion);

                    cache.Remove(nombreCache);
                }

                var politicaDuracion = new CacheItemPolicy();
                if (duracion == Duration.Expire)
                    politicaDuracion.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(30));

                if (datos != null)
                    sw = cache.Add(nombreCache, datos, politicaDuracion);
            }
            catch (Exception er)
            {
                ILogger log = LogFactory.GetLogger(typeof(CacheData));
                log.Error("Add()", er);
            }

            return sw;
        }

        /// <summary>
        /// Almacena un objeto en caché especificando una duración en segundos
        /// </summary>
        /// <param name="nombreCache"></param>
        /// <param name="datos"></param>
        /// <param name="segundos"></param>
        /// <returns></returns>
        public static bool Add(string nombreCache, object datos, long segundos)
        {
            bool sw = false;
            try
            {
                ObjectCache cache = MemoryCache.Default;

                if (cache.Contains(nombreCache))
                {
                    // La caché ya existe, la eliminamos primero
                    ILogger log = LogFactory.GetLogger(typeof(CacheData));
                    log.Warning("Add({0}, obj, {1}). Esta caché ya existía. Reemplazando.", nombreCache, segundos);

                    cache.Remove(nombreCache);
                }

                var politicaDuracion = new CacheItemPolicy();
                politicaDuracion.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddSeconds(segundos));

                if (datos != null)
                    sw = cache.Add(nombreCache, datos, politicaDuracion);
            }
            catch (Exception e)
            {
                ILogger log = LogFactory.GetLogger(typeof(CacheData));
                log.Error("Add()", e);
            }
            return sw;
        }

        public static bool Add(string nombreCache, object datos, DateTime FechaExpiracion)
        {
            ILogger log = LogFactory.GetLogger(typeof(CacheData));

            bool sw = false;

            try
            {
                long diffSeconds = (long)FechaExpiracion.Subtract(DateTime.Now).TotalSeconds;
                sw = Add(nombreCache, datos, diffSeconds);
            }
            catch (Exception ex)
            {
                log.Error("CacheData -> Add()", ex);
            }

            return sw;
        }

        /// <summary>
        /// Elimina un objeto de la caché
        /// </summary>
        /// <param name="nombreCache"></param>
        /// <returns></returns>
        public static bool Remove(string nombreCache)
        {
            bool sw = false;

            try
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache.Contains(nombreCache))
                {
                    cache.Remove(nombreCache);
                    sw = true;
                }
            }
            catch (Exception er)
            {
                ILogger log = LogFactory.GetLogger(typeof(CacheData));
                log.Error("Remove()", er);
            }

            return sw;
        }

        /// <summary>
        /// Elimina todos los objetos de la caché que contengan en su nombre el parametro indicado
        /// </summary>
        /// <param name="nombreCache"></param>
        /// <returns></returns>
        public static bool RemoveContains(string nombreCache)
        {
            bool sw = false;

            try
            {
                ObjectCache cache = MemoryCache.Default;

                foreach (var obj in (MemoryCache)cache)
                {
                    string nombre = obj.Key;

                    if (nombre.Contains(nombreCache))
                        CacheData.Remove(nombre);
                }
                sw = true;
            }
            catch (Exception er)
            {
                ILogger log = LogFactory.GetLogger(typeof(CacheData));
                log.Error("RemoveContains()", er);
            }

            return sw;
        }

        public static bool RemoveAll()
        {
            bool sw = false;

            try
            {
                ObjectCache cache = MemoryCache.Default;

                foreach (var obj in (MemoryCache)cache)
                {
                    string nombre = obj.Key;

                    CacheData.Remove(nombre);
                }
                sw = true;
            }
            catch (Exception er)
            {
                ILogger log = LogFactory.GetLogger(typeof(CacheData));
                log.Error("RemoveAll()", er);
            }

            return sw;
        }



        /// <summary>
        /// Devuelve si existe o no el objeto en la caché
        /// </summary>
        /// <param name="nombreCache"></param>
        /// <returns></returns>
        public static bool Exist(string nombreCache)
        {
            bool sw = false;

            try
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache.Contains(nombreCache))
                {
                    // Encontrado
                    sw = true;
                }
            }
            catch (Exception er)
            {
                ILogger log = LogFactory.GetLogger(typeof(CacheData));
                log.Error("Exist()", er);
            }

            return sw;
        }

        /// <summary>
        /// Devuelve el objeto de la caché en el caso de existir. Sino devuelve null
        /// </summary>
        /// <param name="nombreCache"></param>
        /// <returns></returns>
        public static object Get(string nombreCache)
        {
            object obj = null;

            try
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache.Contains(nombreCache))
                {
                    obj = cache.Get(nombreCache);

                }
            }
            catch (Exception er)
            {
                ILogger log = LogFactory.GetLogger(typeof(CacheData));
                log.Error("Get()", er);
            }

            return obj;
        }

        /// <summary>
        /// Devuelve el nombre de todos los elementos presentes en la caché
        /// </summary>
        /// <returns></returns>
        public static List<string> List()
        {
            List<string> lstCache = new List<string>();

            try
            {
                ObjectCache cache = MemoryCache.Default;

                foreach (var obj in (MemoryCache)cache)
                {
                    /*
                    object o = obj.Value;
                    
                    string texto = string.Format("Name: {0}  Class: {2}",
                        obj.Key,                        
                        o.ToString());
                    */
                    lstCache.Add(obj.Key);
                }
            }
            catch (Exception er)
            {
                ILogger log = LogFactory.GetLogger(typeof(CacheData));
                log.Error("List()", er);
            }

            return lstCache;
        }

    }

}
