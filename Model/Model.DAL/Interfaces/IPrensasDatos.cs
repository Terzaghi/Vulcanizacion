using System;
using System.Collections.Generic;

namespace Model.DAL.Interfaces
{
    public interface IPrensasDatos
    {
        List<DTO.PrensaDato> ListarNuevosRegistros(DateTime Fecha);
    }
}
