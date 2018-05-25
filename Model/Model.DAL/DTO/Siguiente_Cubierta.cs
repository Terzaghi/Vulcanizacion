using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DAL.DTO
{
    public class Siguiente_Cubierta
    {
        public int Id_Prensa { get; set; }
        public DateTime Fecha_Chequeo { get; set; }
        public string Barcode_Cubierta { get; set; }
        public string CV { get; set; }
    }
}
