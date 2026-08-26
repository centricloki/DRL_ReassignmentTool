using System.Collections.Generic;
using EF = DRL.Model.Models;

namespace DRL.Model.Repository.Interface
{
    public interface IBDMasterRepository : IGenericRepository<EF.BDMaster>
    {
        List<EF.BDMaster> GetAllBDs();
        EF.BDMaster GetBDFindById(int BDId);
        List<EF.BDMaster> GetBDFindByRegionId(int regionId);
    }
}
