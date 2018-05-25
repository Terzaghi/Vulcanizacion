using Model.DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAL.Interfaces
{
    public interface ISiguienteCubierta: IDAL<Siguiente_Cubierta, int>
    {
        bool Eliminar(int id_prensa, string cv);
    }
}
