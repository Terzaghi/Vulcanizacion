using Communication.SignalR;
using RequestManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using WCF_RuleMotorServer;

namespace WCF_RequestMotorServer.Clases
{
    public class RequestManagerServiceHostFactory1 : ServiceHostFactory
    {        
        private RequestMotor _motorSolicitudes;
        private SignalRManager _signalR;

        public RequestManagerServiceHostFactory1()
        {
            //this.dep = new MyClass();            
        }

        protected override ServiceHost CreateServiceHost(Type serviceType,
            Uri[] baseAddresses)
        {
            return new RequestManagerServiceHost(ref this._motorSolicitudes, ref this._signalR, serviceType, baseAddresses);
        }
    }

    public class RequestManagerServiceHost : ServiceHost
    {
        public RequestManagerServiceHost(ref RequestMotor motorSolicitudes, ref SignalRManager signalR, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            if (motorSolicitudes == null)
            {
                throw new ArgumentNullException("motorSolicitudes");
            }

            

            foreach (var cd in this.ImplementedContracts.Values)
            {
                cd.Behaviors.Add(new RequestManagerInstanceProvider(ref motorSolicitudes, ref signalR));
            }
        }
      
    }

    public class RequestManagerInstanceProvider : IInstanceProvider, IContractBehavior
    {
  
        private RequestMotor _motorSolicitudes;
        private SignalRManager _signalR;

        public RequestManagerInstanceProvider(ref RequestMotor motorSolicitudes, ref SignalRManager signalR)
        {
            if (motorSolicitudes == null)
            {
                throw new ArgumentNullException("motorSolicitudes");
            }

        
            this._motorSolicitudes = motorSolicitudes;
            this._signalR = signalR;
        }
       
        #region IInstanceProvider Members

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return this.GetInstance(instanceContext);
        }

        public object GetInstance(InstanceContext instanceContext)
        {            
            return new RequestServer(ref this._motorSolicitudes,  ref this._signalR);            
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
        
        #endregion

        #region IContractBehavior Members

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.InstanceProvider = this;
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        #endregion
    }
}
