using DRL.Core.Interface;
using DRL.Core.Mapper;
using DRL.Entity;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using EF = DRL.Model.Models;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using DRL.Model.DataBase;
using DRL.Entity.Response;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace DRL.Core.Manager
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IRegionRepository _regionRepository;
        private readonly IZoneRepository _zoneRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper CommonHelper;
        private readonly IConfiguration _configuration;

        public UserService(IUnitOfWork unitofwork, IUserRepository userRepository, ITeamRepository teamRepository
            , IRegionRepository regionRepository, IZoneRepository zoneRepository, ILogManager logManager
            , IConfiguration configuration)
        {
            _userRepository = userRepository;
            _teamRepository = teamRepository;
            _regionRepository = regionRepository;
            _zoneRepository = zoneRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            CommonHelper = new CommonHelper();
            _configuration = configuration;
        }

        public ENTUser GetUser(long userId)
        {
            ENTUser result = new ENTUser();
            try
            {
                result = Configuration.Mapper.Map<ENTUser>(_userRepository.GetUser(userId));
                result.Teams = _teamRepository.GetUserTeams(userId).Select(p => Configuration.Mapper.Map<ENTTeam>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.GetUser" + ex);
            }
            return result;
        }

        public List<ENTUser> GetAllUsers()
        {
            List<ENTUser> result = new List<ENTUser>();
            try
            {
                result = _userRepository.GetAllUsers().Select(p => Configuration.Mapper.Map<ENTUser>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.GetAllUsers" + ex);
            }
            return result;
        }

        public List<ENTUserResponse> GetUserList()
        {
            List<ENTUserResponse> result = new List<ENTUserResponse>();
            string connString = _configuration.GetConnectionString("DefaultConnection"); ;
            try
            {
                result = SqlDBHelper.RawSqlQuery("EXEC [sp_DSD_GetUserList] ", x => new ENTUserResponse
                {
                    Name = x[0].ToString(),
                    Email = x[1].ToString(),
                    UserName = x[2].ToString(),
                    PIN = x[3].ToString(),
                    RoleName = x[4].ToString(),
                    TerritoryName = x[5].ToString(),
                    ManagerName = x[6].ToString(),
                    UserId = Convert.ToInt32(Convert.ToString(x[7])),
                    IsActive = !Convert.ToBoolean(Convert.ToString(x[8])),
                    CreatedDate = Convert.ToDateTime(x[9]),
                    AVPName = Convert.ToString(x[10]),
                    ZoneName = Convert.ToString(x[11]),
                    RegionName = Convert.ToString(x[12]),
                    BDName = Convert.ToString(x[13]),
                    UserFileName = Convert.ToString(x[14])
                }, connString).OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.GetUserList" + ex);
            }
            return result;
        }

        public List<ENTUser> GetActiveUsers()
        {
            List<ENTUser> result = new List<ENTUser>();
            try
            {
                result = _userRepository.GetActiveUsers().Select(p => Configuration.Mapper.Map<ENTUser>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.GetActiveUsers" + ex);
            }
            return result;
        }

        private (int zoneId, int regionId, int bdId, int avpId) GetHierarchyFromDefaultTeam(int? defaultTeamId)
        {
            int zoneId = 0, regionId = 0, bdId = 0, avpId = 0;
            if (defaultTeamId.HasValue)
            {
                var terr = _teamRepository.GetByWhere(t => t.TerritoryId == defaultTeamId.Value).FirstOrDefault();
                if (terr != null)
                {
                    regionId = terr.RegionId;
                    bdId = terr.BDID ?? 0;
                    var region = _regionRepository.GetByWhere(r => r.RegionId == terr.RegionId).FirstOrDefault();
                    if (region != null)
                    {
                        zoneId = region.ZoneId;
                        var zone = _zoneRepository.GetByWhere(z => z.ZoneId == region.ZoneId).FirstOrDefault();
                        if (zone != null) avpId = zone.AVPID ?? 0;
                    }
                }
            }
            return (zoneId, regionId, bdId, avpId);
        }

        public ActionStatus Insert(ENTUser user)
        {
            // For Insert, we will now use the same logic as Update, calling only the stored procedure.
            // The procedure will handle the insert logic based on the @UserId (which will be 0 or less for a new record).
            return Update(user);
        }

        public ActionStatus Update(ENTUser user)
        {
            try
            {
                // Territory CSV from Teams list (e.g., TW102 = 102)
                string territoryCsv = string.Join(",",
                    user.Teams?.Where(x => x.TeamId.HasValue).Select(x => x.TeamId.Value)
                    ?? new List<int>());

                // Zone CSV for AVP only (e.g., ZE1,ZE3)
                string zoneCsv = string.Join(",",
                    user.Zones?.Select(x => x.ZoneId) ?? new List<int>());

                string conn = _configuration.GetConnectionString("DefaultConnection");

                // Effective CreatedBy / UpdatedBy - proc checks UserId=0 for INSERT
                long effectiveCreatedBy = user.CreatedBy != 0 ? user.CreatedBy : 1;
                long effectiveUpdatedBy = user.UserId ?? effectiveCreatedBy;

                // FIXED: Derive Zone/Region/BD/AVP from DefaultTeamId if available
                // Proc also derives from TerritoryMaster, but passing from UI is better
                int zoneId = 0, regionId = 0, bdId = 0, avpId = 0;
                if (user.DefaultTeamId.HasValue && user.DefaultTeamId.Value > 0)
                {
                    // Uncomment if you have GetHierarchy method - it should return from TerritoryMaster
                    // var hierarchy = GetHierarchyFromDefaultTeam(user.DefaultTeamId.Value);
                    // zoneId = hierarchy.zoneId; regionId = hierarchy.regionId; 
                    // bdId = hierarchy.bdId; avpId = hierarchy.avpId;

                    // If not, let proc derive - but use user.BDID/AVPID as fallback
                    zoneId = 0;
                    regionId = 0;
                    bdId = user.BDID != 0 ? user.BDID : 0;
                    avpId = user.AVPID != 0 ? user.AVPID : 0;
                }
                else
                {
                    // Use values from user object directly (from UI dropdowns)
                    zoneId = 0;
                    regionId = 0;
                    bdId = user.BDID;
                    avpId = user.AVPID;
                }

                var parms = new List<SqlParameter>
{
    new SqlParameter("@UserId", user.UserId ?? 0),
    new SqlParameter("@RoleId", user.RoleId),
    new SqlParameter("@ZoneId", user.ZoneId), // Now using the property from ENTUser
    new SqlParameter("@RegionId", user.RegionId), // Now using the property from ENTUser
    new SqlParameter("@BDID", user.BDID), // Removed ?? 0 since BDID is int not int?
    new SqlParameter("@AVPID", user.AVPID), // Removed ?? 0 since AVPID is int not int?
    new SqlParameter("@TerritoryIds", string.IsNullOrWhiteSpace(territoryCsv) ? (object)DBNull.Value : territoryCsv),
    new SqlParameter("@ZoneIds", string.IsNullOrWhiteSpace(zoneCsv) ? (object)DBNull.Value : zoneCsv),
    new SqlParameter("@DefTerritoryId", user.DefaultTeamId ?? 0), // Using DefaultTeamId instead of DefTerritoryId
    
    // ✅ V33 CRITICAL FIX: Handle empty strings to prevent wiping out existing DB values
    new SqlParameter("@FirstName", string.IsNullOrWhiteSpace(user.FirstName) ? DBNull.Value : (object)user.FirstName),
    new SqlParameter("@LastName", string.IsNullOrWhiteSpace(user.LastName) ? DBNull.Value : (object)user.LastName),
    new SqlParameter("@UserName", string.IsNullOrWhiteSpace(user.UserName) ? DBNull.Value : (object)user.UserName),
    new SqlParameter("@EmailID", string.IsNullOrWhiteSpace(user.Email) ? DBNull.Value : (object)user.Email), // Using Email instead of EmailID
    new SqlParameter("@CreatedBy", user.CreatedBy), // Removed ?? 1 since CreatedBy is long not long?
    new SqlParameter("@UpdatedBy", user.UpdatedBy ?? 1), // Kept ?? 1 since UpdatedBy is long?
    new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output }
};

                // Execute the procedure (adjust method name based on your actual SqlDBHelper implementation)
                SqlDBHelper.ExecuteNonQuery("sp_DSD_UserScreen_ManageUser", ref parms, conn);

                string err = parms.Last().Value?.ToString();
                if (!string.IsNullOrEmpty(err))
                    return new ActionStatus { Success = false, Message = err };

                return new ActionStatus { Success = true, Result = user };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.Update" + ex);
                return new ActionStatus { Success = false, Message = ex.Message };
            }
        }

        public ActionStatus CheckUserNameExists(string userName, long userId)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                result.Success = (_userRepository.FindBy(r => r.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase) && r.UserId != userId && r.IsDeleted == false).Count() > 0);
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.CheckUserNameExists" + ex);
            }
            return result;
        }

        public Int32 GetDefTerritoryIdByUserId(long userId)
        {
            Int32 result = 0;
            try
            {
                var userData = _userRepository.GetUser(userId);
                if (userData != null)
                {
                    result = userData.DefTerritoryId ?? 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.GetDefTerritoryIdByUserId" + ex);
            }
            return result;
        }

        public ActionStatus ManageUserStatus(ENTPatchRequest activeStatus)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                var dbUser = _userRepository.FindBy(x => x.UserId == activeStatus.Id).FirstOrDefault();
                if (dbUser != null)
                {
                    dbUser.IsInActive = !activeStatus.status;
                    if (dbUser.IsInActive)
                    {
                        dbUser.Pin = null;
                        dbUser.TerritoryId = string.Empty;
                        dbUser.DefTerritoryId = null;
                    }
                    dbUser.UpdatedDate = GetDateTime.getDate();
                    dbUser.UpdatedBy = activeStatus.UpdatedBy;
                    var response = _userRepository.Update(dbUser);
                    return response;

                }

                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.ManageUserStatus" + ex);
            }
            return result;
        }

        public ActionStatus ClearUserFileName(long userId, long updatedBy)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                var dbUser = _userRepository.FindByNoTracking(x => x.UserId == userId).FirstOrDefault();
                if (dbUser != null)
                {
                    dbUser.UserFileName = null;
                    dbUser.UpdatedDate = GetDateTime.getDate();
                    dbUser.UpdatedBy = updatedBy;
                    var response = _userRepository.Update(dbUser);
                    return response;
                }

                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.ClearUserFileName" + ex);
            }
            return result;
        }

        public ActionStatus DeleteUser(ENTPatchRequest activeStatus)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                var dbUser = _userRepository.FindBy(x => x.UserId == activeStatus.Id).FirstOrDefault();
                if (dbUser != null)
                {
                    dbUser.IsDeleted = activeStatus.status;
                    if (dbUser.IsDeleted)
                    {
                        dbUser.Pin = null;
                        dbUser.TerritoryId = string.Empty;
                        dbUser.DefTerritoryId = null;
                    }
                    dbUser.UpdatedDate = GetDateTime.getDate();
                    dbUser.UpdatedBy = activeStatus.UpdatedBy;
                    var response = _userRepository.Update(dbUser);
                    return response;
                }

                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.DeleteUser" + ex);
            }
            return result;
        }

        public List<ENTTerriotyUsers> GetAllUserByTerritoryId(Int32 TerritoryId)
        {
            List<ENTTerriotyUsers> result = new List<ENTTerriotyUsers>();
            string connString = _configuration.GetConnectionString("DefaultConnection"); ;
            try
            {
                result = SqlDBHelper.RawSqlQuery("EXEC [sp_DSD_GetUserListByTerritoryId] " + TerritoryId, x => new ENTTerriotyUsers
                {
                    UserName = x["UserName"].ToString(),
                    Name = x["Name"].ToString(),
                    HoneyPin = x["PIN"].ToString(),
                    ReportsTo = x["ManagerName"].ToString(),
                    Role = x["RoleName"].ToString(),
                    IsActive = !Convert.ToBoolean(Convert.ToString(x["IsInActive"])),
                    CreatedDate = Convert.ToDateTime(x["CreatedDate"]),
                    UserId = Convert.ToInt32(x["UserID"]),
                    IsTerritoryUser = Convert.ToBoolean(x["IsTerritoryUser"])
                }, connString).OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.GetAllUserByTerritoryId", ex);
            }
            return result;

        }

        public ActionStatus UpdateUserTerritory(Int32 UserId, Int32 TerritoryId, long UpdatedBy)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                var dbUser = _userRepository.FindBy(x => x.UserId == UserId).FirstOrDefault();

                if (dbUser != null)
                {
                    if (string.IsNullOrWhiteSpace(dbUser.TerritoryId) || !(("," + dbUser.TerritoryId + ",").IndexOf("," + TerritoryId + ",") > -1))
                    {
                        dbUser.TerritoryId = string.IsNullOrWhiteSpace(dbUser.TerritoryId) ? Convert.ToString(TerritoryId) : (dbUser.TerritoryId + "," + TerritoryId);
                    }
                    else
                    {
                        return new ActionStatus
                        {
                            Success = false,
                            Message = "Already Exists"
                        };
                    }
                    dbUser.UpdatedBy = UpdatedBy;
                    dbUser.UpdatedDate = GetDateTime.getDate();
                    var response = _userRepository.Update(dbUser);
                    return response;

                }

                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.UpdateUserTerritory" + ex);
            }
            return result;
        }

        public ActionStatus DeleteUserTerritory(Int32 UserId, Int32 TerritoryId, long UpdatedBy)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                var dbUser = _userRepository.FindBy(x => x.UserId == UserId).FirstOrDefault();

                if (dbUser != null && !string.IsNullOrWhiteSpace(dbUser.TerritoryId) && (("," + dbUser.TerritoryId + ",").IndexOf("," + TerritoryId + ",") > -1))
                {
                    List<String> Items = dbUser.TerritoryId.Split(",").Select(i => i.Trim()).Where(i => i != string.Empty).ToList(); //Split them all and remove spaces
                    Items.Remove(Convert.ToString(TerritoryId)); //or whichever you want
                    string NewX = String.Join(", ", Items.ToArray());

                    dbUser.TerritoryId = NewX;
                    dbUser.UpdatedBy = UpdatedBy;
                    dbUser.UpdatedDate = GetDateTime.getDate();
                    if (dbUser != null)
                    {
                        var response = _userRepository.Update(dbUser);
                        return response;
                    }
                    return result;
                }

                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.DeleteUserTerritory" + ex);
            }
            return result;
        }

        public List<ENTUser> GetAllUsersByRoleId(Int32 roleId)
        {
            List<ENTUser> result = new List<ENTUser>();
            try
            {
                result = _userRepository.GetAllUsersByRoleId(roleId).Select(p => Configuration.Mapper.Map<ENTUser>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.GetAllUsersByRoleId" + ex);
            }
            return result;
        }

        public List<ENTUser> GetAllUsersByManagerId(Int32 managerId)
        {
            List<ENTUser> result = new List<ENTUser>();
            try
            {
                result = _userRepository.GetAllUsersByManagerId(managerId).Select(p => Configuration.Mapper.Map<ENTUser>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.GetAllUsersByManagerId" + ex);
            }
            return result;
        }

        public List<ENTReassignmentUsers> GetReassignUsers(int? page = 1, int? pageSize = 10, string TerritoryId = "", string userName = "")
        {
            List<ENTReassignmentUsers> result = new List<ENTReassignmentUsers>();
            string connString = _configuration.GetConnectionString("DefaultConnection"); ;
            try
            {
                userName = userName == "NULL" ? "" : userName;
                result = SqlDBHelper.RawSqlQuery($"EXEC [sp_DSD_GetReassignUsers] " +
                    $"{page} ,{pageSize}, '{TerritoryId}','{userName}',''", x => new ENTReassignmentUsers
                    {
                        Name = x["Name"].ToString(),
                        Role = x["Role"].ToString(),
                        Team = x["Team"].ToString(),
                        Territory = x["Territory"].ToString(),
                        Title = x["Territory"].ToString(),
                        UserName = x["UserName"].ToString(),
                        UserId = Convert.ToInt32(x["UserID"]),
                        KeyAccount = x["KeyAccount"].ToString(),
                        InsideSales = x["InsideSales"].ToString(),
                        Broker = x["Broker"].ToString(),
                    }, connString).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.GetReassignUsers", ex);
            }
            return result;
        }

        public ActionStatus ChangeUserDetails(ENTChangeUserDetailsRequest request)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                string connString = _configuration.GetConnectionString("DefaultConnection"); ;
                string UserIds = String.Join(',', request.userIds.Where(x => x > 0).ToList());
                if (!string.IsNullOrWhiteSpace(UserIds))
                {
                    List<SqlParameter> sqlParameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@UserIds", UserIds),
                        new SqlParameter("@UpdateTerritoryId", request.updateTerritoryId),
                        new SqlParameter("@AddTeamId", request.addTeamId),
                        new SqlParameter("@DeleteTeamId", request.deleteTeamId),
                        new SqlParameter("@UpdatedBy", request.UpdatedBy),
                    };

                    int count = SqlDBHelper.ExecuteNonQuery("sp_DSD_ChangeUserDetails", ref sqlParameters, connString);
                    if (count > 0)
                    {
                        return new ActionStatus
                        {
                            Success = true,
                            Message = ""
                        };
                    }
                }

                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.ChangeUserDetails" + ex);
            }
            return result;
        }

        public List<ENTTerriotyUsers> GetUsersByTerritoryIdAndUserId(Int32 TerritoryId, Int32 UserId)
        {
            List<ENTTerriotyUsers> result = new List<ENTTerriotyUsers>();
            string connString = _configuration.GetConnectionString("DefaultConnection"); ;
            try
            {
                result = SqlDBHelper.RawSqlQuery($"EXEC [sp_DSD_GetUserListHierarchy] '{TerritoryId}','{UserId}'", x => new ENTTerriotyUsers
                {
                    UserName = x["UserName"].ToString(),
                    Name = x["Name"].ToString(),
                    HoneyPin = x["PIN"].ToString(),
                    ReportsTo = x["ManagerName"].ToString(),
                    Role = x["RoleName"].ToString(),
                    IsActive = !Convert.ToBoolean(Convert.ToString(x["IsInActive"])),
                    CreatedDate = Convert.ToDateTime(x["CreatedDate"]),
                    UserId = Convert.ToInt32(x["UserID"]),
                    IsTerritoryUser = Convert.ToBoolean(x["IsTerritoryUser"])
                }, connString).OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.GetAllUserByTerritoryId", ex);
            }
            return result;

        }

        public int GetUserIdByUserName(string Username)
        {
            int result = 0;
            string connString = _configuration.GetConnectionString("DefaultConnection"); ;
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@pUsername", Username)
                    };

                var returnValue = SqlDBHelper.ExecuteScalar("[sp_DSD_GetUserIdByUserName]", ref sqlParameters, connString);

                if (returnValue != null)
                {
                    result = Convert.ToInt32(returnValue);
                }
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.GetUserIdByName", ex);
            }
            return result;
        }
    }
}

