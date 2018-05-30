using Model.BL.DTO.Enums;
using RuleManager.DTO;
using System.Collections.Generic;
using System.ServiceModel;

namespace WCF_RuleMotorServer.Interfaz
{
    [ServiceContract]
    public interface IRequestMotorWCF
    {
        [OperationContract]
        void MarkAs_Async(long ids_Request, Estado_Solicitud state, int? id_User, int? id_Device);
        [OperationContract]
        void AddPrensa(int id_Prensa);
        [OperationContract]
        void ModifyPrensa();
        [OperationContract]
        void RemovePrensa(int id_prensa);
        [OperationContract]
        bool IsBarcodeValid(string barcode, int id_prensa);
        [OperationContract]
        Tipo_Contramedidas getContramedidas(int id_prensa);
    }
}
