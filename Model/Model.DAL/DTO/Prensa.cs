using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DAL.DTO
{
    public class Prensa
    {
        public int Id { get; set; }
        public string Nombre { get; set; }       
        public string Barcode_Prensa { get; set; }
        public string Barcode_Pintado { get; set; }
        public string Barcode_Pinchado { get; set; }
        public int Prensa_Activa { get; set; }


        public int Id_Zone { get; set; }

    }
}
