using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApi.Client.Classes
{
    internal class Request : System.IDisposable
    {
        private string _host;
        private int? _timeout = null;

        public Request(string host, int? timeoutSeconds = null)
        {
            this._host = host;
            this._timeout = timeoutSeconds;
        }

        public void Dispose()
        {
            _host = null;
        }

        public async Task<object> GetAsync(string uri, Type responseType)
        {
            object result = null;

            try
            {
                using (var connector = new Connector(_host, _timeout))
                {
                    using (var response = await connector.Client.GetAsync(uri))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();

                            result = JsonConvert.DeserializeObject(json, responseType);
                        }
                    }
                }
            }
            catch (TaskCanceledException er)
            {
                //Timeout establecido en connector.Client.Timeout
                string exception = er.Message;
            }
            catch (Exception er)
            {
                //TODO: Qué hacer con las excepciones en Android
                string exception = er.Message;
            }

            return result;
        }
        
        public async Task<object> PostAsync(string uri, Type responseType, object attributes)
        {
            object result = null;

            try
            {
                var jsonText = JsonConvert.SerializeObject(attributes);

                using (var connector = new Connector(_host, _timeout))
                {
                    var httpContent = new StringContent(jsonText);
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    using (var response = await connector.Client.PostAsync(uri, httpContent))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, responseType);
                        }
                    }
                }
            }
            catch (TaskCanceledException er)
            {
                //Timeout establecido en connector.Client.Timeout
                string exception = er.Message;
            }
            catch (Exception er)
            {
                //TODO: Qué hacer con las excepciones en Android
                string exception = er.Message;
            }

            return result;
        }

        //public static async Task<object> PostAsync(string uri, Type responseType, JObject attributes)
        //{
        //    object result = null;

        //    try
        //    {
        //        string jsonText = attributes.ToString();

        //        using (var connector = new Connector())
        //        {
        //            var httpContent = new StringContent(jsonText);
        //            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        //            using (var response = await connector.Client.PostAsync(uri, httpContent))
        //            {
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, responseType);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //TODO: Qué hacer con las excepciones en Android
        //        var a = 0;
        //    }

        //    return result;
        //}
    }
}
