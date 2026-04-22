using System.Collections.Generic;
using EF = DRL.Model.Models;

namespace DRL.Model.Repository.Interface
{
    public interface IBDMasterRepository : IGenericRepository<EF.BDMaster>
    {
        List<EF.BDMaster> GetAllBDs();
    }
}
