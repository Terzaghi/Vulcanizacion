using Communication.SignalR.Clases;
using LoggerManager;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using System;

namespace Communication.SignalR
{
    public class Startup
    {
        ILogger log = LogFactory.GetLogger(typeof(Startup));

        public void Configuration(IAppBuilder app)
        {
            log.Information("Iniciando SignalR");
            log.Information("Iniciando CORS para signalR");

            // Agrega Application_PreSendRequestHeaders a OWIN (no hay iis)
            //AddPreSendRequestHeaders(app);
            /*
            app.Use(async (context, next) =>
            {
                context.Response.OnSendingHeaders(state =>
                {
                    ((Microsoft.Owin.OwinResponse)state).Headers.Remove("Server");

                }, context.Response);

                await next();
            });
            */

            //Permite la conexión de los clientes desde el exterior de la aplicación
            app.UseCors(CorsOptions.AllowAll);
            // Cargamos la configuración para timeouts
            ConfigureGlobalHost(app);

            app.MapSignalR();






            // Para reinicios
            /*
            var hubConfiguration = new HubConfiguration { Resolver = new DefaultDependencyResolver() };
            app.MapSignalR(hubConfiguration);
            */
            //var hubConfiguration = new HubConfiguration { EnableDetailedErrors = true, Resolver = new DefaultDependencyResolver() };
            //app.MapSignalR(hubConfiguration);








            //var hubConfiguration = new HubConfiguration();
            //hubConfiguration.EnableDetailedErrors = true;

            ////app.UseCors(CorsOptions.AllowAll);
            //app.MapSignalR("/signalr", hubConfiguration);
        }

        /*
        private void AddPreSendRequestHeaders(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.OnSendingHeaders(state =>
                {
                    ((Microsoft.Owin.OwinResponse)state).Headers.Remove("Server");

                }, context.Response);

                await next();
            });
        }
        
        private void Application_PreSendRequestHeaders(IAppBuilder app)
        {

        }
        */
        public void ConfigureGlobalHost(IAppBuilder app)
        {
            try
            {

                log.Debug("Configurando supervisión de signalR");

                // Creamos una clase que loguea los errores del servidor de SignalR
                GlobalHost.HubPipeline.AddModule(new HubHandlingPipelineModule());

                log.Debug("Configurando tiempos de espera");

                GlobalHost.Configuration.TransportConnectTimeout = TimeSpan.FromSeconds(10);


                // Make long polling connections wait a maximum of 110 seconds for a
                // response. When that time expires, trigger a timeout command and
                // make the client reconnect.
                GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(110);

                // Wait a maximum of 30 seconds after a transport connection is lost
                // before raising the Disconnected event to terminate the SignalR connection.
                GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(30);

                // For transports other than long polling, send a keepalive packet every
                // 10 seconds. 
                // This value must be no more than 1/3 of the DisconnectTimeout value.
                GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(10);

                //Setting up the message buffer size
                GlobalHost.Configuration.DefaultMessageBufferSize = 500;
            }
            catch (Exception er)
            {
                log.Error("ConfigureGlobalHost()", er);
            }

        }


    }
}
