using System.Collections.Generic;
using EF = DRL.Model.Models;

namespace DRL.Model.Repository.Interface
{
    public interface IStateRepository : IGenericRepository<EF.StateMaster>
    {
        List<EF.StateMaster> GetStates();
    }
}
