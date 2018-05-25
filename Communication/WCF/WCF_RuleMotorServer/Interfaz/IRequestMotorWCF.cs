using RuleManager.DTO;
using System.Collections.Generic;
using System.ServiceModel;

namespace WCF_RuleMotorServer.Interfaz
{
    [ServiceContract]
    public interface IRequestMotorWCF
    {
        [OperationContract]
        bool IsActive();

        [OperationContract]
        string ListActiveRequest(List<int> Ids_Requests);

        [OperationContract]
        string ListPendingRequestsWithState(int? id_User, int? id_Device, int numeroElementos);


        [OperationContract]
        void MarkAllAs_Async(long[] Ids_RequestGenerateds, int state, int? id_User, int? id_Device);

        [OperationContract]
        object ReadValue(string tag);
    }
}
