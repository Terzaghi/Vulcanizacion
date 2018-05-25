using Model.DAL.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DAL.Interfaces
{
    public interface IUsuario: IDAL<Usuario, int>
    {
        bool Vincular(int Id_User, int Id_Prensa);
        bool Desvincular(int Id_User, int Id_Prensa);
    }
}
