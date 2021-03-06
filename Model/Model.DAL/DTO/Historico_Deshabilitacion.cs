﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DAL.DTO
{
    public class Historico_Deshabilitacion
    {
        public long Id_Deshabilitacion { get; set; }
        public DateTime Fecha { get; set; }
        public string Comentario { get; set; }

        public int Id_Motivo { get; set; }
        public int Id_Permiso { get; set; }
        public int Id_Prensa { get; set; }
        public int Id_Usuario { get; set; }
        public int Id_Dispositivo { get; set; }
    }
}
