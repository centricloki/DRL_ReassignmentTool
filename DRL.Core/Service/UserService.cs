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
            string connString = _configuration.GetConnectionString("DefaultConnection");;
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
                    BDName = Convert.ToString(x[13])
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

        public ActionStatus Insert(ENTUser user)
        {
            try
            {
                var dbUser = Configuration.Mapper.Map<ENTUser, EF.UserMaster>(user);
                dbUser.CreatedDate = GetDateTime.getDate();
                dbUser.IsInActive = !user.IsActive;
                var terr = _teamRepository.GetByWhere(t => t.TerritoryId == user.DefaultTeamId).FirstOrDefault();
                if (terr != null && terr.RegionId > 0)
                {
                    dbUser.BDID = terr.BDID ?? 0;
                    dbUser.RegionId = terr.RegionId;

                    var region = _regionRepository.GetByWhere(t => t.RegionId == terr.RegionId).FirstOrDefault();
                    if (region != null && region.ZoneId > 0)
                    {
                        dbUser.ZoneId = region.ZoneId;
                        var zone = _zoneRepository.GetByWhere(t => t.ZoneId == region.ZoneId).FirstOrDefault();
                        if (zone != null && zone.AVPID > 0)
                        {
                            dbUser.AVPID = zone.AVPID ?? 0;
                        }
                    }
                }
                var resp = _userRepository.Insert(dbUser);
                resp.Result = Configuration.Mapper.Map(resp.Result, user);
                return resp;
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.Insert" + ex);
                return new ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public ActionStatus Update(ENTUser user)
        {
            try
            {
                var dbUser = _userRepository.GetUser(user.UserId ?? 0);
                if (dbUser == null)
                    return new ActionStatus
                    {
                        Success = false,
                        Message = "User not exist!"
                    };

                dbUser = Configuration.Mapper.Map(user, dbUser);
                if (user.IsActive == false)
                {
                    dbUser.Pin = null;
                    dbUser.TerritoryId = string.Empty;
                    dbUser.DefTerritoryId = null;
                }
                dbUser.IsInActive = !user.IsActive;
                var terr = _teamRepository.GetByWhere(t => t.TerritoryId == user.DefaultTeamId).FirstOrDefault();
                if (terr != null && terr.RegionId > 0)
                {
                    dbUser.BDID = terr.BDID ?? 0;
                    dbUser.RegionId = terr.RegionId;

                    var region = _regionRepository.GetByWhere(t => t.RegionId == terr.RegionId).FirstOrDefault();
                    if (region != null && region.ZoneId > 0)
                    {
                        dbUser.ZoneId = region.ZoneId;
                        var zone = _zoneRepository.GetByWhere(t => t.ZoneId == region.ZoneId).FirstOrDefault();
                        if (zone != null && zone.AVPID > 0)
                        {
                            dbUser.AVPID = zone.AVPID ?? 0;
                        }
                    }
                }
                dbUser.UpdatedDate = GetDateTime.getDate();
                var result = _userRepository.Update(dbUser);
                result.Result = Configuration.Mapper.Map(result.Result, user);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.Update" + ex);
                return new ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
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
            string connString = _configuration.GetConnectionString("DefaultConnection");;
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
            string connString = _configuration.GetConnectionString("DefaultConnection");;
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
                string connString = _configuration.GetConnectionString("DefaultConnection");;
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
            string connString = _configuration.GetConnectionString("DefaultConnection");;
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
            string connString = _configuration.GetConnectionString("DefaultConnection");;
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

