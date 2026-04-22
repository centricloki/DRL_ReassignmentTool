using System.Collections.Generic;
using EF = DRL.Model.Models;

namespace DRL.Model.Repository.Interface
{
    public interface IRegionRepository : IGenericRepository<EF.RegionMaster>
    {
        List<EF.RegionMaster> GetAllRegion();
        EF.RegionMaster GetRegion(long RegionId);
        List<EF.RegionMaster> GetAllActiveRegions();
    }
}
