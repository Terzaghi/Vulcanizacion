using Model.BL.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.BL.DTO
{
    public class Historico_Solicitud
    {
        public long Id_Historico { get; set; }
        public DateTime Fecha { get; set; }

        public long Id_Solicitud { get; set; }
        public Estado_Solicitud Estado { get; set; }
        public int Id_Usuario { get; set; }
        public int Id_Dispositivo { get; set; }
    }
}
