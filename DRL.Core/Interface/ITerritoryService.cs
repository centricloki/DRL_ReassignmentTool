using DRL.Entity;
using DRL.Library;
using System;
using System.Collections.Generic;

namespace DRL.Core.Interface
{
    public interface ITerritoryService
    {
        List<ENTLookUpItem> GetAllTerritoryLookup();
        List<ENTLookUpItem> GetAllTerritoryLookupByUserId(long UserId);
        ActionStatus CheckTerritoryNameExists(string territoryName, int territoryID);
        ENTTeam GetTerritory(long TeamId);
        ActionStatus Insert(ENTTeam Team);
        ActionStatus Update(ENTTeam Team);
        List<ENTTeam> GetAllTerritories();
        List<ENTTeam> GetAllUserTerritories(long userId);
        List<ENTTeam> GetAllBDTerritories(int bdId);
        ActionStatus DeleteTerritory(ENTPatchRequest activeStatus);
        List<ENTLookUpItem> GetCustReassignTerritoriesByRoleIds(string[] roleIds);
        List<ENTLookUpItem> GetCustReassignTeamsByRoleIds(string[] roleIds);
        List<ENTTeam> GetTeamListFromRegionId(long regionId);
        bool SyncBDTerritories(int BDId, List<int> territoryIds, long currentUserId);
        bool AssignBDToTerritories(int BDId, List<int> territoryIds, long currentUserId);
        bool RemoveBDFromTerritories(List<int> territoryIds, long currentUserId);
    }
}
