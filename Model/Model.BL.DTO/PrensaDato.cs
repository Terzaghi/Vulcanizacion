using System;

namespace Model.BL.DTO
{
    public class PrensaDato
    {
        public DateTime Fecha { get; set; }
        public int PrensaId { get; set; }
        public string TagActivaValue { get; set; }
        public string TagCVValue { get; set; }
        public string TagTempValue { get; set; }
        public string TagCicloValue { get; set; }
        public string TagProdValue { get; set; }
        public string Cavidad { get; set; }
    }
}
