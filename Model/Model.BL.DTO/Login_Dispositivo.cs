using Model.BL.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.BL.DTO
{
    public class Login_Dispositivo
    {
        public long Id_Login;
        public DateTime Fecha { get; set; }
        public string Connection_Id { get; set; }
        public int Id_Dispositivo { get; set; }
        public int Id_Usuario { get; set; }
        public Tipo_Evento Evento { get; set; }
    }
}
