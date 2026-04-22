using DRL.Entity;
using DRL.Entity.Response;
using DRL.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Core.Interface
{
    public interface IRegionService
    {
        List<ENTLookUpItem> GetAllRegionLookup();
        ENTRegion GetRegion(long RegionId);
        List<ENTRegion> GetAllRegions();
        ActionStatus CheckRegionNameExists(string regionName, int regionId);
        ActionStatus DeleteRegion(ENTPatchRequest activeStatus);
        ActionStatus Insert(ENTRegion Region);
        ActionStatus Update(ENTRegion Region);
        List<ENTRegionResponse> GetRegionList();
        ActionStatus ManageRegionStatus(ENTPatchRequest activeStatus);
    }
}
