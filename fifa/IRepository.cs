using System.Collections.Generic;
using fifa.Models;

namespace fifa
{
    public interface IRepository
    {
        IEnumerable<League> GetAll();
        League Get(int id);
        void Create(League league);
    }
}