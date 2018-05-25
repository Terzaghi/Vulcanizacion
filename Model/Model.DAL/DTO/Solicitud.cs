using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DAL.DTO
{
    public class Solicitud
    {
        public long Id { get; set; }
        public DateTime Fecha_Generacion { get; set; }
        public int Id_Prensa { get; set; }
    }
}
