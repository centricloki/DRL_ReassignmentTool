using DRL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
