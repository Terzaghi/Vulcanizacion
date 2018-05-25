using System;
using System.Collections.Generic;
using System.Text;

namespace Model.BL.DTO
{
    public class Siguiente_Cubierta
    {
        public Prensa Prensa { get; set; }
        public DateTime Fecha_Chequeo { get; set; }
        public string Barcode_Cubierta { get; set; }
        public string CV { get; set; }
    }
}
