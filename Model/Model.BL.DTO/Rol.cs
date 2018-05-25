using Model.BL.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.BL.DTO
{
    public class Rol
    {
        public Tipo_Rol rol { get; set; }
        public string Nombre { get; set; }
        public List<Usuario> UsuariosRol { get; set; }
      
    }
}
