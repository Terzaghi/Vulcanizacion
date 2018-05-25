using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAL.DTO
{
    public class Usuario_Prensa
    {
        public int Id_Prensa { get; set; }
        public int Id_Usuario { get; set; }
        public DateTime Fecha_Asignacion { get; set; }
    }
}
