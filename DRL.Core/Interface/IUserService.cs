using DRL.Entity;
using DRL.Entity.Response;
using DRL.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Core.Interface
{
    public interface IUserService
    {
        ENTUser GetUser(long userId);
        List<ENTUser> GetAllUsers();
        List<ENTUserResponse> GetUserList();
        List<ENTUser> GetActiveUsers();
        ActionStatus Insert(ENTUser user);
        ActionStatus Update(ENTUser user);
        ActionStatus CheckUserNameExists(string userName, long userId);
        Int32 GetDefTerritoryIdByUserId(long UserId);
        ActionStatus ManageUserStatus(ENTPatchRequest activeStatus);
        ActionStatus DeleteUser(ENTPatchRequest activeStatus);
        List<ENTTerriotyUsers> GetAllUserByTerritoryId(Int32 TerritoryId);
        ActionStatus UpdateUserTerritory(Int32 UserId, Int32 TerritoryId, long UpdatedBy);
        ActionStatus DeleteUserTerritory(Int32 UserId, Int32 TerritoryId, long UpdatedBy);
        List<ENTUser> GetAllUsersByRoleId(Int32 roleId);
        List<ENTUser> GetAllUsersByManagerId(Int32 managerId);
        List<ENTReassignmentUsers> GetReassignUsers(int? page = 1, int? pageSize = 10, string TerritoryId = "",string userName="");
        ActionStatus ChangeUserDetails(ENTChangeUserDetailsRequest request);
        List<ENTTerriotyUsers> GetUsersByTerritoryIdAndUserId(Int32 TerritoryId, Int32 UserId);

        int GetUserIdByUserName(string Username);
    }
}
