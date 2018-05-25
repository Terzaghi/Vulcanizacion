using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.SignalR.DTO
{
    public class RequestToSend
    {
        public int Id_Request { get; set; }
        public long Id_RequestGenerated { get; set; }

        public RequestToSend(int Id_Request, long Id_RequestGenerated)
        {
            this.Id_Request = Id_Request;
            this.Id_RequestGenerated = Id_RequestGenerated;
        }
    }
}
