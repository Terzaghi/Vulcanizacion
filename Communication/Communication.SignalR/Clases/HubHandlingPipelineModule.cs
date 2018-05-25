using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using LoggerManager;

namespace BSHP_PSM.Communication.SignalR.Clases
{
    public class HubHandlingPipelineModule : HubPipelineModule
    {
        ILogger log = LogFactory.GetLogger(typeof(HubHandlingPipelineModule));

        // Github errores en su versión
        // https://github.com/SignalR/SignalR/issues?utf8=%E2%9C%93&q=is%3Aissue%20is%3Aopen

        // Métodos 
        // https://msdn.microsoft.com/en-us/library/microsoft.aspnet.signalr.hubs.hubpipelinemodule(v=vs.118).aspx

        // Funcionamiento 
        // https://github.com/aspnet/Docs/blob/master/aspnet/signalr/overview/guide-to-the-api/handling-connection-lifetime-events.md#serverdisconnect


        #region Métodos que no existen en la 2.0.3     
        /*   
        public override Func<IHub, bool, Task> BuildDisconnect(Func<IHub, bool, Task> disconnect)
        {
            log.Debug("SignalR. HubHandlingPipelineModule. BuildDisconnect()");
            return base.BuildDisconnect(disconnect);
        }

        protected override void OnAfterDisconnect(IHub hub, bool stopCalled)
        {
            log.Debug("SignalR. HubHandlingPipelineModule. OnAfterDisconnect()");
            base.OnAfterDisconnect(hub, stopCalled);
        }

        protected override bool OnBeforeDisconnect(IHub hub, bool stopCalled)
        {
            bool sw = true;

            log.Debug("SignalR. HubHandlingPipelineModule. OnBeforeDisconnect(stopCalled: {0})", stopCalled);

            sw = base.OnBeforeDisconnect(hub, stopCalled);

            log.Debug("base.OnBeforeDisconnect(). Respondió: {0}", sw);

            return sw;
        }
        */
        #endregion

        #region Public
        public override Func<HubDescriptor, IRequest, bool> BuildAuthorizeConnect(Func<HubDescriptor, IRequest, bool> authorizeConnect)
        {
            log.Debug("SignalR. HubHandlingPipelineModule. BuildAuthorizeConnect()");
            return base.BuildAuthorizeConnect(authorizeConnect);
        }

        public override Func<IHub, Task> BuildConnect(Func<IHub, Task> connect)
        {
            log.Debug("SignalR. HubHandlingPipelineModule. BuildConnect()");
            return base.BuildConnect(connect);
        }

        public override Func<IHubIncomingInvokerContext, Task<object>> BuildIncoming(Func<IHubIncomingInvokerContext, Task<object>> invoke)
        {
            log.Debug("SignalR. HubHandlingPipelineModule. BuildIncoming()");
            return base.BuildIncoming(invoke);
        }

        public override Func<IHubOutgoingInvokerContext, Task> BuildOutgoing(Func<IHubOutgoingInvokerContext, Task> send)
        {
            log.Debug("SignalR. HubHandlingPipelineModule. BuildOutgoing()");
            return base.BuildOutgoing(send);
        }

        public override Func<IHub, Task> BuildReconnect(Func<IHub, Task> reconnect)
        {
            log.Debug("SignalR. HubHandlingPipelineModule. BuildReconnect()");
            return base.BuildReconnect(reconnect);
        }

        public override Func<HubDescriptor, IRequest, IList<string>, IList<string>> BuildRejoiningGroups(Func<HubDescriptor, IRequest, IList<string>, IList<string>> rejoiningGroups)
        {
            log.Debug("SignalR. HubHandlingPipelineModule. BuildRejoiningGroups()");
            return base.BuildRejoiningGroups(rejoiningGroups);
        }
        #endregion

        #region Protected
        protected override void OnAfterConnect(IHub hub)
        {
            log.Debug("SignalR. HubHandlingPipelineModule. OnAfterConnect()");
            base.OnAfterConnect(hub);
        }

        protected override object OnAfterIncoming(object result, IHubIncomingInvokerContext context)
        {
            log.Debug("SignalR. HubHandlingPipelineModule. OnAfterIncoming()");
            return base.OnAfterIncoming(result, context);
        }

        protected override void OnAfterOutgoing(IHubOutgoingInvokerContext context)
        {
            log.Debug("SignalR. HubHandlingPipelineModule. OnAfterOutgoing()");
            base.OnAfterOutgoing(context);
        }

        protected override void OnAfterReconnect(IHub hub)
        {
            log.Debug("SignalR. HubHandlingPipelineModule. OnAfterReconnect()");
            base.OnAfterReconnect(hub);
        }

        protected override bool OnBeforeAuthorizeConnect(HubDescriptor hubDescriptor, IRequest request)
        {
            bool sw = false;

            try
            {
                sw = base.OnBeforeAuthorizeConnect(hubDescriptor, request);

                log.Debug("SignalR. HubHandlingPipelineModule. OnBeforeAuthorizeConnect(). Result: {0}", sw);
                
                // Cargamos el estado de las variables de la conexión
                if (!sw)
                {
                    log.Warning("SignalR. HubHandlingPipelineModule. OnBeforeAuthorizeConnect(). DEVOLVIÓ FALSE -----------------------------------------------------------");

                    string url = string.Empty,
                           sessionId = string.Empty;

                    if (request != null && request.Url != null) url = request.Url.OriginalString;
                    //if (request.Cookies["ASP.NET_SessionId"] != null) sessionId = request.Cookies["ASP.NET_SessionId"].Value;

                    Cookie c;
                    if (request.Cookies.TryGetValue("ASP.NET_SessionId", out c))
                    {
                        sessionId = c.Value;
                    }


                    log.Debug("Original string: {0}", url);
                    log.Debug("SessionID: {0}", sessionId);

                    //foreach (KeyValuePair<string, object> dato in request.Environment.Keys)                    
                    object obj;
                    foreach (string dato in request.Environment.Keys)
                    {
                        if (!request.Environment.TryGetValue(dato, out obj))
                            obj = null;

                        if (obj != null)
                        {
                            log.Debug("  {0}: {1}", dato, obj.ToString());
                        }
                        else
                        {
                            log.Debug("  {0}: NULL", dato);
                        }
                    }

                    log.Warning("HA DEVUELTO FALSE. LE DECIMOS QUE TRUE!");
                    sw = true;
                }

                //sw = false;   // Para probar el error

            }
            catch (Exception er)
            {
                log.Error("OnBeforeAuthorizeConnect()", er);
            }

            return sw;
        }

        protected override bool OnBeforeConnect(IHub hub)
        {
            bool sw = true;

            sw = base.OnBeforeConnect(hub);
            log.Debug("SignalR. HubHandlingPipelineModule. OnBeforeConnect(). Result: {0}", sw);
            
            return sw;            
        }
        
        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            bool sw = true;

            try
            {
                sw = base.OnBeforeIncoming(context);

                log.Debug("SignalR. HubHandlingPipelineModule. OnBeforeIncoming(). Result: {0}", sw);

                if (log.isDebugEnabled)
                {
                    log.Debug("=> Invoking " + context.MethodDescriptor.Name + " on hub " + context.MethodDescriptor.Hub.Name);
                }

                //return false;   // *************
            }
            catch (Exception er)
            {
                log.Error("OnBeforeIncoming()", er);
            }

            return sw;
        }

        protected override bool OnBeforeOutgoing(IHubOutgoingInvokerContext context)
        {
            bool sw = true;

            try
            {
                sw = base.OnBeforeOutgoing(context);
                log.Debug("SignalR. HubHandlingPipelineModule. OnBeforeOutgoing(). Result: {0}", sw);

                if (log.isDebugEnabled)
                {
                    //log.Debug("<= Invoking {0} on client hub {1}, data: {2}", context.Invocation.Method, context.Invocation.Hub, string.Join(", ", context.Invocation.Args));
                    string parametros = string.Join(", ", context.Invocation.Args);
                    if (parametros.Length > 100)
                        parametros = parametros.Substring(0, 100) + "...(continua)...";
                    log.Debug("<= Invoking {0} on client hub {1}, data: {2}", context.Invocation.Method, context.Invocation.Hub, parametros);
                }

                //return false; //*********        
            }
            catch (Exception er)
            {
                log.Error("OnBeforeOutgoing()", er);
            }

            return sw;
        }

        protected override bool OnBeforeReconnect(IHub hub)
        {
            bool sw = true;

            sw = base.OnBeforeReconnect(hub);
            log.Debug("SignalR. HubHandlingPipelineModule. OnBeforeReconnect(). Result: {0}", sw);

            //return false;   // *********

            return sw;
        }
        #endregion


        /// <summary>
        /// Guardamos la traza cuando se produce un error
        /// </summary>
        /// <param name="exceptionContext"></param>
        /// <param name="invokerContext"></param>
        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            try
            {
                log.Debug("SignalR. HubHandlingPipelineModule. OnIncomingError()");                

                dynamic caller = invokerContext.Hub.Clients.Caller;
                caller.ExceptionHandler(exceptionContext.Error.Message);

                log.Debug("SignalR. OnIncomingError(). Mensaje: {0}", exceptionContext.Error.Message);

                
                log.Debug("=> Exception " + exceptionContext.Error.Message);
                if (exceptionContext.Error.InnerException != null)
                {
                    log.Debug("=> Inner Exception " + exceptionContext.Error.InnerException.Message);
                }

            }
            catch (Exception er)
            {
                log.Error("OnIncomingError()", er);
            }            

            base.OnIncomingError(exceptionContext, invokerContext);
        }
    }
}
