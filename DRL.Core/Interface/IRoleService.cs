using DRL.Entity;
using DRL.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Core.Interface
{
    public interface IRoleService
    {
        ENTRole GetRole(long RoleId);
        ENTRole GetRole(string roleName);
        List<ENTRole> GetAllRoles();
        List<ENTLookUpItem> GetActiveRoles();
        ActionStatus Insert(ENTRole Role);
        ActionStatus Update(ENTRole Role);
        ActionStatus CheckRoleNameExists(string roleName, int roleID);
        ActionStatus DeleteRole(ENTPatchRequest activeStatus);
        List<ENTLookUpItem> GetCustomerReassignmentRoles();
    }
}
