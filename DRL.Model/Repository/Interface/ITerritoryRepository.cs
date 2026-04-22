using System;
using System.Collections.Generic;
using EF = DRL.Model.Models;

namespace DRL.Model.Repository.Interface
{
    public interface ITerritoryRepository : IGenericRepository<EF.TerritoryMaster>
    {
        List<EF.TerritoryMaster> GetAllTerritory();
        List<EF.TerritoryMaster> GetAllTerritoryByUserId(long UserId);
        EF.TerritoryMaster GetTerritory(long TeamId);
        List<EF.TerritoryMaster> GetAllTerritories();
        List<EF.TerritoryMaster> GetCustReassignTerritoriesByRoleIds(string[] RoleId);
        List<EF.TerritoryMaster> GetCustReassignTeamByRoleIds(string[] RoleId);
    }
}
