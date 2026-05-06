using DRL.Entity;
using DRL.Entity.Response;
using DRL.Library;
using System.Collections.Generic;

namespace DRL.Core.Interface
{
    public interface IZoneService
    {
        List<ENTLookUpItem> GetAllZoneLookup();
        List<ENTLookUpItem> GetAllZoneLookupByAVP(long userId);
        List<ENTZone> GetAllZones();
        List<ENTZone> GetAllZoneByAVP(int avpId);
        bool SyncAVPZones(int AVPId, List<int> zoneIds);
        bool AssignAVPToZones(int AVPId, List<int> zoneIds);
        bool RemoveAVPFromZones(List<int> zoneIds);
        
        // CRUD methods similar to RegionService
        ENTZone GetZone(int zoneId);
        List<ENTZoneResponse> GetZoneList();
        ActionStatus CheckZoneNameExists(string zoneName, int zoneId);
        ActionStatus Insert(ENTZone zone);
        ActionStatus Update(ENTZone zone);
        ActionStatus DeleteZone(ENTPatchRequest activeStatus);
        ActionStatus ManageZoneStatus(ENTPatchRequest activeStatus);
        ActionStatus UpdateZoneAndMappingTables(ENTZone zone);
    }
}
