
using Communication.SignalR.DTO;
using System;
using System.Collections.Generic;

namespace Communication.Interfaces
{
    public delegate void ClientConnectedEventHandler(string connectionId, string ip, int Id_User);
    public delegate void ClientDisconnectedEventHandler(string connectionId, string ip, int Id_User);
    public delegate void RequestAcceptedEventHandler(string connectionId, int requestId);
   

    public interface ICommunication : IDisposable
    {
        //Un cliente se conecta al servidor
        event ClientConnectedEventHandler OnClientConnected;
        //Un cliente se desconecta del servidor
        event ClientDisconnectedEventHandler OnClientDisconnected;

        //Notificación recibida por el servidor        
        event RequestAcceptedEventHandler OnRequestAccepted;
     

        //Arranque del servidor
        bool Start();

        //Parada del servidor
        bool Stop();
        
        //bool SendNotification(string ip, int notificationId, string message, string json, DateTime fecha);
        // El servidor envia la notificacion a los usuarios/clientes 
        //bool SendRequest(List<int> IdsUsuarios,List<string> DevicesIPs, string jsonNotificacion, DateTime fecha, bool activa);

       //bool SendChangeState(List<int> IdsUsuarios,List<string> DevicesIPs, RequestToSend request, StateToSend State);

    
       
    }
}
