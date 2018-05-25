using System;
using System.Net.Http;

namespace WebApi.Client.Classes
{
    internal class Connector : System.IDisposable
    {
        //ILogger log = BSHP.LoggerManager.LogFactory.GetLogger(typeof(ControllerConnector));

        private HttpClient _client;

        public HttpClient Client
        {
            get
            {
                return _client;
            }
        }

        public Connector(string host, int? timeoutSeconds)
        {
            this._client = Conectar(host, timeoutSeconds);
        }

        private HttpClient Conectar(string host, int? timeoutSeconds)
        {
            HttpClient client = null;

            try
            {
                //string host = "http://localhost:8080";
                
                client = new HttpClient();
                client.BaseAddress = new Uri(host);

                if (timeoutSeconds == null)
                {
                    //Por defecto se establece un timeout de 15 segundos para todas las llamadas
                    client.Timeout = new TimeSpan(0, 0, 15);    // Timeout 15sg
                }
                else if (timeoutSeconds.Value > 0)
                {
                    //Si es mayor de 0, se establece el timeout. Si es menor de 0, no se establece ningún Timeout
                    client.Timeout = new TimeSpan(0, 0, timeoutSeconds.Value);
                }

            }
            catch (Exception er)
            {
                string exception = er.Message;
                //log.Error("Conectar()", er);
            }
            return client;
        }

        public void Dispose()
        {
            _client = null;
        }
    }
}
