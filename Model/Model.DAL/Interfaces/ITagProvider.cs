using Model.DAL.DTO;
using System.Collections.Generic;

namespace Model.DAL.Interfaces
{
    public interface ITagProvider : IDAL<Tag_Provider, int>
    {
        IList<Tag_Provider> ListByService(int Id_Service);
    }
}
