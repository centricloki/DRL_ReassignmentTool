using System.Collections.Generic;
using EF = DRL.Model.Models;

namespace DRL.Model.Repository.Interface
{
    public interface IRoleRepository : IGenericRepository<EF.RoleMaster>
    {
        EF.RoleMaster GetRole(long roleId);
        List<EF.RoleMaster> GetAllRoles();
        List<EF.RoleMaster> GetActiveRoles();
    }
}
