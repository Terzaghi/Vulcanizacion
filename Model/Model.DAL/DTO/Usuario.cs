using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DAL.DTO
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Identity_Code { get; set; }
        public string Password { get; set; }

        public int Id_Encargado { get; set; }
    }
}
