using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Client.Classes;

namespace WebApi.Client
{
    public class RequestMotorClient
    {
        private readonly string _host;

        public RequestMotorClient(string host)
        {
            this._host = host;
        }

        #region Test

        public async Task<bool> GetTest()
        {
            bool result = false;

            try
            {
                string message = "Hola Mundo";

                string url = string.Format("api/RequestMotor/GetTest/{0}", message);

                using (var request = new Request(this._host))
                {
                    var response = await request.GetAsync(url, typeof(bool));

                    if (response != null)
                    {
                        result = (bool)response;
                    }
                }
            }
            catch (Exception er)
            {
                string exception = er.Message;
            }
            return result;
        }

        public async Task<bool> GetStatus()
        {
            bool result = false;

            try
            {
                string url = string.Format("api/RequestMotor/GetStatus");

                using (var request = new Request(this._host))
                {
                    var response = await request.GetAsync(url, typeof(bool));

                    if (response != null)
                    {
                        result = (bool)response;
                    }
                }
            }
            catch (Exception er)
            {
                string exception = er.Message;
            }
            return result;
        }

        public async Task<bool> PostTest()
        {
            bool result = false;

            try
            {
                string url = string.Format("api/RequestMotor/PostTest");

                JObject attributes = new JObject();
                attributes.Add("parameter1", JToken.FromObject(new List<int>() {1,2,3 }));
                attributes.Add("parameter2", "Hola Mundo");

                using (var request = new Request(this._host))
                {
                    var response = await request.PostAsync(url, typeof(bool), attributes);

                    if (response != null)
                    {
                        //var a = response as bool;
                        result = (bool)response;
                    }
                }
            }
            catch (Exception er)
            {
                string exception = er.Message;
            }
            return result;
        }

        #endregion

       
    }
}