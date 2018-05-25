using System;
using System.Collections.Generic;
using System.Text;

namespace Model.BL.DTO
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Identity_Code { get; set; }
        public string Password { get; set; }

        public Usuario Encargado { get; set; }
        public List<Rol> RolesUsuario { get; set; }
        public List<Prensa> PrensasUsuario { get; set; }
    }
}
