using DRL.API.Extensions;
using DRL.Core.Interface;
using DRL.Entity;
using DRL.Entity.Response;
using DRL.Library;
using DRL.Model.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace DRL.API.Controllers
{
    [CustomAuthorizeAttribute]
    [Route("api/User")]
    public class UserController : BaseController
    {
        private static readonly HttpClient _HttpClient = new HttpClient();
        private readonly IUserService _userService;
        private readonly ITerritoryService _territoryService;
        private readonly IZoneService _zoneService;
        private readonly IRoleService _roleService;
        private readonly CommonHelper _commonHelper = new CommonHelper();
        private IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        public UserController(
            IUserService userService,
            IConfiguration configuration,
            ITerritoryService territoryService,
            IZoneService zoneService,
            IRoleService roleService,
            ICacheService cacheService)
        {
            _userService = userService;
            _configuration = configuration;
            _territoryService = territoryService;
            _zoneService = zoneService;
            _roleService = roleService;
            _cacheService = cacheService;
        }

        /// <summary>
        ///     Get All Users List
        /// </summary>
        /// <returns>
        ///     ENTUser  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetUserList")]
        public BaseResponse<List<ENTUserResponse>> GetUserList()
        {
            var response = new BaseResponse<List<ENTUserResponse>>(true);
            response.Data = _userService.GetUserList();
            return response;
        }

        /// <summary>
        ///     Get All Users List
        /// </summary>
        /// <returns>
        ///     ENTUser  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetAllUsers")]
        public BaseResponse<List<ENTUser>> GetAllUsers()
        {
            var response = new BaseResponse<List<ENTUser>>(true);
            response.Data = _userService.GetAllUsers();
            return response;
        }


        /// <summary>
        ///     Get user details by userId
        /// </summary>
        /// <returns>
        ///     ENTUser  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetUser/{userId}")]
        public BaseResponse<ENTUser> GetUser(long userId)
        {
            var response = new BaseResponse<ENTUser>(true);
            response.Data = _userService.GetUser(userId);
            return response;
        }

        /// <summary>
        ///     Add/Update User Details
        /// </summary>
        /// <returns>
        ///     ENTUser Model
        /// </returns>
        [HttpPost("ManageUser")]
        public BaseResponse<ENTUser> ManageUser([FromBody] ENTUser user)
        {
            BaseResponse<ENTUser> response = new BaseResponse<ENTUser>();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return new BaseResponse<ENTUser> { Message = "Invalid Model: " + string.Join(", ", errors) };
            }
            else
            {
                var serviceResponse = new ActionStatus();

                serviceResponse = _userService.CheckUserNameExists(user.UserName, user.UserId ?? 0);
                if (!serviceResponse.Success)
                {
                    string TerriotoryId = "";
                    ENTRole avpRole = _roleService.GetRole("AVP");
                    ENTRole bdRole = _roleService.GetRole("BD Manager");
                    if (user.RoleId == avpRole?.RoleId) // Handle zones for AVP 
                    {
                        var zoneIds = user.Zones.Select(x => x.ZoneId)
                                             .ToList();

                        _zoneService.SyncAVPZones(user.AVPID, zoneIds);
                    }
                    else if (user.RoleId == bdRole?.RoleId) // Handle Teams for BD
                    {
                        var newTerritoryIds = user.Teams.Where(x => x.TeamId.HasValue)
                                             .Select(x => x.TeamId.Value)
                                             .ToList();

                        _territoryService.SyncBDTerritories(user.BDID, newTerritoryIds, CurrentUserId);
                    }

                    if (user.RoleId != avpRole?.RoleId && user.Teams != null && user.Teams.Count > 0) //Update TerritoryId column
                    {
                        for (int i = 0; i < user.Teams.Count; i++)
                        {
                            if (i != user.Teams.Count - 1)
                            {
                                TerriotoryId = TerriotoryId + user.Teams[i].TeamId + ",";
                            }
                            else
                            {
                                TerriotoryId = TerriotoryId + user.Teams[i].TeamId;
                            }
                        }
                        user.TerritoryId = TerriotoryId;

                    }

                    if (user.UserId <= 0 || user.UserId == null)
                    {
                        // Set default value of 1 if CurrentUserId is null or invalid
                        user.CreatedBy = CurrentUserId > 0 ? CurrentUserId : 1;
                        serviceResponse = _userService.Insert(user);
                        if (serviceResponse.Success)
                        {
                            response.IsSuccess = true;
                            response.Message = "User added successfully";
                            response.Data = serviceResponse.Result as ENTUser;
                            ClearRoleCaches();
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = serviceResponse.Message;
                            response.Data = serviceResponse.Result as ENTUser;
                        }
                    }
                    else
                    {
                        // Set default value of 1 if CurrentUserId is null or invalid
                        user.UpdatedBy = CurrentUserId > 0 ? CurrentUserId : 1;
                        serviceResponse = _userService.Update(user);
                        if (serviceResponse.Success)
                        {
                            response.IsSuccess = true;
                            response.Message = "User updated successfully";
                            response.Data = serviceResponse.Result as ENTUser;
                            ClearRoleCaches();
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "";
                            response.Data = serviceResponse.Result as ENTUser;
                        }
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "User is already exists with same user name.";
                    response.Data = null;
                }
                return response;
            }
        }

        /// <summary>
        ///     Get Def Territory by userId
        /// </summary>
        /// <returns>
        ///     Int32
        /// </returns>
        // GET api/values
        [HttpGet("{userId}/DefTerritoryId")]
        public BaseResponse<Int32> GetDefTerritoryIdByUserId(long userId)
        {
            var response = new BaseResponse<Int32>(true);
            response.Data = _userService.GetDefTerritoryIdByUserId(userId);
            return response;
        }

        /// <summary>
        ///     Active/InActive User
        /// </summary>
        /// <returns>
        /// </returns>
        // PATCH api/User/ActiveUser
        [HttpPatch("ManageUserStatus")]
        public BaseResponse<ActionStatus> ManageUserStatus([FromBody] ENTPatchRequest activeStatus)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                var msg = "";
                if (activeStatus.status)
                    msg = "activated";
                else
                    msg = "inactivated";


                activeStatus.UpdatedBy = CurrentUserId;
                var serviceResponse = _userService.ManageUserStatus(activeStatus);
                if (serviceResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Message = "User updated successfully";
                    ClearRoleCaches();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Some error occurred updating user status";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
            return response;
        }

        /// <summary>
        ///     Delete user by user id
        /// </summary>
        /// <returns>
        /// </returns>
        // PATCH api/User/DeleteUserbyUserId
        [HttpPatch("DeleteUserbyUserId")]
        public BaseResponse<ActionStatus> DeleteUserbyUserId([FromBody] ENTPatchRequest activeStatus)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                activeStatus.UpdatedBy = CurrentUserId;
                var serviceResponse = _userService.DeleteUser(activeStatus);
                if (serviceResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Message = "User deleted successfully";
                    ClearRoleCaches();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Some error occurred deleting user";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
            return response;
        }

        /// <summary>
        ///     Get All User List by territory id
        /// </summary>
        /// <returns>
        ///     ENTTerriotyUsers  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetAllUsersByTerritoryId/{territoryId}")]
        public BaseResponse<List<ENTTerriotyUsers>> GetAllUsersByTerritoryId(Int32 territoryId)
        {
            var response = new BaseResponse<List<ENTTerriotyUsers>>(true);
            response.Data = _userService.GetAllUserByTerritoryId(territoryId);
            return response;
        }

        /// <summary>
        ///     Get All User List by role id
        /// </summary>
        /// <returns>
        ///     ENTUser  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetAllUsersByRoleId/{roleId}")]
        public BaseResponse<List<ENTUser>> GetAllUsersByRoleId(Int32 roleId)
        {
            var response = new BaseResponse<List<ENTUser>>(true);
            response.Data = _userService.GetAllUsersByRoleId(roleId);
            return response;
        }

        /// <summary>
        ///    Update User Territory
        /// </summary>
        /// <returns>
        ///     
        /// </returns>
        // PATCH UpdateUserTerritory/{userId}/{territoryId}
        [HttpPatch("UpdateUserTerritory/{userId}/{territoryId}")]
        public BaseResponse<ActionStatus> UpdateUserTerritory(Int32 userId, Int32 territoryId)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {

                var serviceResponse = _userService.UpdateUserTerritory(userId, territoryId, CurrentUserId);
                if (serviceResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Message = "Team updated successfully";
                    ClearRoleCaches();
                }
                else if (!serviceResponse.Success && !string.IsNullOrWhiteSpace(serviceResponse.Message))
                {
                    response.IsSuccess = false;
                    response.Message = serviceResponse.Message;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Some error occurred updating user status";
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
            return response;
        }

        /// <summary>
        ///    Delete User Territory
        /// </summary>
        /// <returns>
        ///     
        /// </returns>
        // Delete DeleteUserTerritory/{userId}/{territoryId}
        [HttpPatch("DeleteUserTerritory/{userId}/{territoryId}")]
        public BaseResponse<ActionStatus> DeleteUserTerritory(Int32 userId, Int32 territoryId)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                var serviceResponse = _userService.DeleteUserTerritory(userId, territoryId, CurrentUserId);
                if (serviceResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Message = "User removed successfully from team";
                    ClearRoleCaches();
                }
                else if (!serviceResponse.Success && !string.IsNullOrWhiteSpace(serviceResponse.Message))
                {
                    response.IsSuccess = false;
                    response.Message = serviceResponse.Message;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Some error occurred updating user status";
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
            return response;
        }

        /// <summary>
        ///     Get All User List by reports to id
        /// </summary>
        /// <returns>
        ///     ENTUser  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetAllUsersByManagerId/{managerId}")]
        public BaseResponse<List<ENTUser>> GetAllUsersByManagerId(Int32 managerId)
        {
            var response = new BaseResponse<List<ENTUser>>(true);
            response.Data = _userService.GetAllUsersByManagerId(managerId);
            return response;
        }


        /// <summary>
        ///     Get All User List by territory id and user id
        /// </summary>
        /// <returns>
        ///     ENTTerriotyUsers  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetUsersByTerritoryIdAndUserId/{territoryId}/{userId}")]
        public BaseResponse<List<ENTTerriotyUsers>> GetUsersByTerritoryIdAndUserId(Int32 territoryId, Int32 userId)
        {
            var response = new BaseResponse<List<ENTTerriotyUsers>>(true);
            response.Data = _userService.GetUsersByTerritoryIdAndUserId(territoryId, userId);
            return response;
        }

        /// <summary>
        ///     Get All Territory List for BD
        /// </summary>
        /// <returns>
        ///     ENTTeam  Model
        /// </returns>
        // GET api/values
        [HttpGet("BD/{bdId}/Territories")]
        public BaseResponse<List<ENTTeam>> GetTerritoriesByBD(int bdId)
        {
            var response = new BaseResponse<List<ENTTeam>>(true);
            response.Data = _territoryService.GetAllBDTerritories(bdId);
            return response;
        }

        /// <summary>
        ///     Get All Territory List for user
        /// </summary>
        /// <returns>
        ///     ENTTeam  Model
        /// </returns>
        // GET api/values
        [HttpGet("{userId}/Territories")]
        public BaseResponse<List<ENTTeam>> GetTerritoriesByUser(long userId)
        {
            var response = new BaseResponse<List<ENTTeam>>(true);
            response.Data = _territoryService.GetAllUserTerritories(userId);
            return response;
        }

        /// <summary>
        ///     Get All Zone List for AVP
        /// </summary>
        /// <returns>
        ///     ENTZone  Model
        /// </returns>
        // GET api/values
        [HttpGet("AVP/{avpId}/Zones")]
        public BaseResponse<List<ENTZone>> GetZonesByAVP(int avpId)
        {
            var response = new BaseResponse<List<ENTZone>>(true);
            response.Data = _zoneService.GetAllZoneByAVP(avpId);
            return response;
        }

        /// <summary>
        ///     Get All Territory List
        /// </summary>
        /// <returns>
        ///     ENTTeam  Model
        /// </returns>
        // GET api/values
        [HttpGet("Territories")]
        public BaseResponse<List<ENTTeam>> GetTerritories()
        {
            var response = new BaseResponse<List<ENTTeam>>(true);
            response.Data = _territoryService.GetAllTerritories();
            return response;
        }

        /// <summary>
        ///     Get All Zone List
        /// </summary>
        /// <returns>
        ///     ENTZone  Model
        /// </returns>
        // GET api/values
        [HttpGet("Zones")]
        public BaseResponse<List<ENTZone>> GetZones()
        {
            var response = new BaseResponse<List<ENTZone>>(true);
            response.Data = _zoneService.GetAllZones().OrderBy(x => x.ZoneName).ToList();
            return response;
        }

        private void ClearRoleCaches()
        {
            // 🔑 MUST MATCH EXACT KEYS USED IN LookupController
            _cacheService.Remove(LookupCacheKeys.ROLES_KEY);
            _cacheService.Remove(LookupCacheKeys.REGIONS_KEY);
            _cacheService.Remove(LookupCacheKeys.ZONES_KEY);
            _cacheService.Remove(LookupCacheKeys.TERRITORIES_KEY);
            _cacheService.Remove(LookupCacheKeys.STATES_KEY);
            _cacheService.Remove(LookupCacheKeys.CUST_REASSIGN_ROLES_KEY);
            _cacheService.Remove(LookupCacheKeys.AVPS_KEY);
            _cacheService.Remove(LookupCacheKeys.BDS_KEY);
            _cacheService.Remove(LookupCacheKeys.CITIES_KEY_PREFIX);
            _cacheService.RemoveByPrefix(LookupCacheKeys.USER_REPORT_CACHE_PREFIX);
            _cacheService.RemoveByPrefix(LookupCacheKeys.USER_REPORT_HIERARCHY_CACHE_PREFIX);
        }
    }
}