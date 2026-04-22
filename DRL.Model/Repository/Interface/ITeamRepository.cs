using System.Collections.Generic;
using EF = DRL.Model.Models;

namespace DRL.Model.Repository.Interface
{
    public interface ITeamRepository : IGenericRepository<EF.TerritoryMaster>
    {
        List<EF.TerritoryMaster> GetUserTeams(long userId);
    }
}
