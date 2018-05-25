using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DAL.Interfaces
{
    public interface IDAL<TEntity, KId>
    {
        TEntity Detalles(KId id);
        IList<TEntity> Listar();

        int Agregar(TEntity entidad);
        bool Modificar(TEntity entidad);
        bool Eliminar(KId id);
    }
}
