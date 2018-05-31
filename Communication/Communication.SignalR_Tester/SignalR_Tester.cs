using LoggerManager;
using System;

namespace Communication.SignalR_Tester
{
    public class SignalR_Tester
    {
        ILogger log = LoggerManager.LogFactory.GetLogger(typeof(SignalR_Tester));

        public void TestConnection(string url, string strToken, Action<bool> callback)
        {
            bool sw = false;

            // Creamos la conexión
            var hubConnection = new HubConnection(url, string.Format("token={0}", strToken));
            IHubProxy hubProxy = hubConnection.CreateHubProxy("EccoHub");

            hubConnection.Start().ContinueWith(task =>
            {
                // Comprobamos si conectó
                if (!task.IsFaulted)
                {

                    // Realizamos una petición al servidor para comprobar el estado real
                    // Realizamos una petición a un método que devuelve la hora
                    hubProxy.Invoke("Test").ContinueWith((t) =>
                    {
                        if (!t.IsFaulted && t.Exception == null)
                        {
                            sw = true;
                        }                        
                        hubConnection.Stop();

                        if (callback != null) callback(sw);
                    });
                }
                else
                {
                    // No está conectado
                    if (callback != null) callback(sw);
                }
            });
        }

        /*
        public async Task<bool> Connect2(string url, string strToken)
        {
            bool sw = false;

            try
            {
                //string url = "http://localhost:6969/";

                // Creamos la conexión
                var hubConnection = new HubConnection(url, string.Format("token={0}", strToken));
                IHubProxy hubProxy = hubConnection.CreateHubProxy("EccoHub");

                //hubConnection.Start().ContinueWith(task => {
                //        // Comprobamos si conectó
                //        if (!task.IsFaulted)
                //        {
                //        
                //        hubProxy.On<string>("Send", (name, message) =>
                //        {
                //            Console.WriteLine("Incoming data: {0} {1}", name, message);
                //        });
                //        

                //        // Realizamos una petición al servidor para comprobar el estado real
                //        // Realizamos una petición a un método que devuelve la hora
                //        hubProxy.Invoke("Test").ContinueWith((t) =>
                //            {
                //                if (!t.IsFaulted && t.Exception == null)
                //                {
                //                    sw = true;
                //                }

                //                hubConnection.Stop();
                //            });
                //        }
                //    }
                //);


                var task = hubConnection.Start();
                await task;

                // Comprobamos si conectó
                if (!task.IsFaulted)
                {
                    
                    //hubProxy.On<string>("Send", (name, message) =>
                    //{
                    //    Console.WriteLine("Incoming data: {0} {1}", name, message);
                    //});
                    

                    // Realizamos una petición al servidor para comprobar el estado real
                    // Realizamos una petición a un método que devuelve la hora
                    var task2 = hubProxy.Invoke("Test");
                    //task2.Wait();
                    await task2;

                    if (!task2.IsFaulted && task2.Exception == null)
                    {
                        sw = true;
                    }

                }
                hubConnection.Stop();
            }
            catch (Exception er)
            {
                log.Error("Connect()", er);
            }

            return sw;
        }
        */



    }
}
