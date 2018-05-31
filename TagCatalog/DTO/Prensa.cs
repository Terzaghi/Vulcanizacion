using Model.BL.DTO.Enums;
using PrensaCatalog.Enums;

namespace PrensaCatalog.DTO
{
    public class Prensa
    {
        public Model.BL.DTO.Prensa prensa { get; set; }
        public string Barcode_Cubierta { get; set; }
        public string Barcode_Siguiente_Cubierta { get; set; }

        public Prioridad prioridad {get; set;}
        public Tipo_Contramedidas[] Contramedidas { get; set; }
    }
}
