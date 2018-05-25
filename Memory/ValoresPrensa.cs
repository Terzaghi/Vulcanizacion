using System.Collections.Generic;

namespace Memory.Common
{
    public class ValoresPrensa
    {
        public int Id_Prensa { get; set; }

        public Dictionary<TagType, TagValue> Valores { get; set; }
    }
}
