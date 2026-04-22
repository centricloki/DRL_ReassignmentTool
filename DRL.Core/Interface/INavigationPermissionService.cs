using DRL.Entity;
using DRL.Library;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DRL.Core.Interface
{
    public interface INavigationPermissionService
    {
        Task<List<ENTLinkPermission>> GetPermissionsForGroupAsync(string groupName);
        Task<Dictionary<int, string>> GetActiveUserGroupsAsync();
        void ClearUserGroupCache();
    }
}
