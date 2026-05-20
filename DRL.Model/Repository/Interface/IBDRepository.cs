using System.Collections.Generic;
using EF = DRL.Model.Models;

namespace DRL.Model.Repository.Interface
{
    public interface IBDRepository : IGenericRepository<EF.BDMaster>
    {
        List<EF.BDMaster> GetAllBD();
        EF.BDMaster GetBDFindById(int BDId);
    }
}
