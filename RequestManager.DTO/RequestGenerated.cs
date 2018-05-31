using System;
using System.Collections.Generic;

namespace RequestManager.DTO
{
    public class RequestGenerated : Request, ICloneable
    {
        public long Id_Request_Generated { get; set; }
        public DateTime DateGeneration { get; set; }
        public int Id_Prensa { get; set; }
        public List<int> Ids_Users { get; set; }
        public List<int> Ids_Devices { get; set; }



        public RequestGenerated(Request request, long Id_Request_Generated)
        {
            if (request != null)
            {
                // Propiedades de notification             
                this.Id_Request = request.Id_Request;
                this.AckRequiered = request.AckRequiered;
                this.AckRequiered_AllUsers = request.AckRequiered_AllUsers;
                this.AckRequiered_AllDevices = request.AckRequiered_AllDevices;
                this.Description = request.Description;
                this.Name = request.Name;
                this.ShowPopup = request.ShowPopup;
                this.TagParameters = request.TagParameters;
                //this.Type = request.Type;
                this.TTL = request.TTL;
                this.DisplayRequiered = request.DisplayRequiered;
            }

            // Propiedades propias de request generated
            this.Id_Request_Generated = Id_Request_Generated;
            this.DateGeneration = DateTime.Now;


        }

        #region IClonable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
