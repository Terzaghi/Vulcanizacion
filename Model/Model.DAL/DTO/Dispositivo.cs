using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DAL.DTO
{
   public class Dispositivo
    {
        public int Id_Disposito { get; set; }
        public string Serial_Number { get; set; }
        public string IP { get; set; }
        public string Descripcion { get; set; }
    }
}
