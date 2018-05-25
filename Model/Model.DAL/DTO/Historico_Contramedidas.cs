using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DAL.DTO
{
   public class Historico_Contramedidas
    {
        public long Id { get; set; }
        public DateTime Expiracion { get; set; }
        public string CV { get; set; }
        public string Lote { get; set; }

        public int Id_Contramedida { get; set; }
        public int Id_Prensa { get; set; }
    }
}
