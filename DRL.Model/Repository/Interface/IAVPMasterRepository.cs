using System.Collections.Generic;
using EF = DRL.Model.Models;

namespace DRL.Model.Repository.Interface
{
    public interface IAVPMasterRepository : IGenericRepository<EF.AVPMaster>
    {
        List<EF.AVPMaster> GetAllAVPs();
    }
}
