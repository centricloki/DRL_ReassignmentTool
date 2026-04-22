using System;
using System.Collections.Generic;
using EF = DRL.Model.Models;

namespace DRL.Model.Repository.Interface
{
    public interface IUserRepository : IGenericRepository<EF.UserMaster>
    {
        EF.UserMaster GetUser(long userId);
        List<EF.UserMaster> GetAllUsers();
        List<EF.UserMaster> GetActiveUsers();
        List<EF.UserMaster> GetAllUsersByRoleId(Int32 RoleId);
        List<EF.UserMaster> GetAllUsersByManagerId(Int32 ManagerId);
    }
}
