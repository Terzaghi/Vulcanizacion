
using Memory.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManager.DTO
{
    public class Request
    {
        /// <summary>
        /// Identificador de la solicitud 
        /// </summary>
        public int Id_Request { get; set; }
        /// <summary>
        /// Nombre de la solicitud 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Descripción de la solicitud
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Indica el tipo de de solicitud 
        /// </summary>
        //public RequestType Type { get; set; }
      

        // Vinculaciones a otros datos:

        /// <summary>
        /// Contiene las señales que están vinculadas a la solicitud para informar al usuario de sus valores
        /// </summary>
        /// 
        public List<TagValue> TagParameters { get; set; }

        /// <summary>
        /// Se configura para indicar que el paso final de la solicitud sea requerir que se reconozca la acción (acknowledgement)
        /// </summary>
        public bool AckRequiered { get; set; }
        /// <summary>
        /// Se configura para indicar que el paso final de la solicitud sea requerir que se reconozca la acción por todos los usuarios (acknowledgement)
        /// </summary>
        public bool AckRequiered_AllUsers { get; set; }
        /// <summary>
        /// Se configura para indicar que el paso final de la solicitud sea requerir que se reconozca la acción por todos los dispositivos (acknowledgement)
        /// </summary>
        public bool AckRequiered_AllDevices { get; set; }
        /// <summary>        
        /// Hará que la solicitud, si el usuario no la ha visto nunca, se le muestre aunque esté ya reconocida, hasta que se le haya mostrado en pantalla alguna vez
        /// </summary>
        public bool DisplayRequiered { get; set; }
        /// <summary>
        /// Indica si al recibirse la solicitud es necesario que se abra como popup o no (aparecería en la lista de notificaciones pero no se abriría sola)
        /// </summary>
        public bool ShowPopup { get; set; }
        /// <summary>
        /// Tiempo de vida de la solicitud una vez lanzada
        /// </summary>
        public int TTL { get; set; }



        public Request()
        {
            this.Id_Request = 0;
            //this.Type = RequestType.Media;
            this.TagParameters = new List<TagValue>();
            this.AckRequiered = false;
            this.AckRequiered_AllUsers = false;
            this.AckRequiered_AllDevices = false;
            this.ShowPopup = false;
            this.DisplayRequiered = false;
        }
    }
}
