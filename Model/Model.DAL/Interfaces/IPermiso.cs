using Model.DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DAL.Interfaces
{
    public interface IPermiso: IDAL<Permiso, int>
    {
        bool Vincular(int Id_Permiso, int Id_Rol, int Id_Motivo);
        bool Desvincular(int Id_Permiso, int Id_Rol, int Id_Motivo);
    }
}
