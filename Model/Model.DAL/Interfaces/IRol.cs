using Model.DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAL.Interfaces
{
    public interface IRol: IDAL<Rol, int>
    {
        bool Vincular(int Id_User, int Id_Rol);
        bool Desvincular(int Id_User, int Id_Rol);

    }
}
