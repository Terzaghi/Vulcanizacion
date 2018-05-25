using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DAL.Interfaces
{
    public interface IHistoricoDAL<TEntity, KId>
    {
        TEntity Detalles(KId id);
        IList<TEntity> Listar();

        int Agregar(TEntity entidad);
    
    }
}
