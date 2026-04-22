using DRL.API.Extensions;
using DRL.Core.Interface;
using DRL.Core.Manager;
using DRL.Core.Service;
using DRL.Entity;
using DRL.Library;
using DRL.Model.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DRL.API.Controllers
{
    [CustomAuthorizeAttribute]
    [Route("api/Role")]
    //[ApiController]
    public class RoleController : BaseController
    {
        private static readonly HttpClient _HttpClient = new HttpClient();
        private readonly IRoleService _RoleService;
        private readonly CommonHelper _commonHelper = new CommonHelper();
        private IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        public RoleController(
          IRoleService roleService,
          IConfiguration configuration,
           ICacheService cacheService)
        {
            _RoleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cacheService = cacheService;
        }

        /// <summary>
        ///     Get All Roles List
        /// </summary>
        /// <returns>
        ///     ENTRole  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetAllRoles")]
        public BaseResponse<List<ENTRole>> GetAllRoles()
        {
            var response = new BaseResponse<List<ENTRole>>(true);
            response.Data = _RoleService.GetAllRoles();
            return response;
        }

        /// <summary>
        ///     Get Role details by RoleId
        /// </summary>
        /// <returns>
        ///     ENTRole  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetRole/{RoleId}")]
        public BaseResponse<ENTRole> GetRole(long RoleId)
        {
            var response = new BaseResponse<ENTRole>(true);
            response.Data = _RoleService.GetRole(RoleId);
            return response;
        }

        /// <summary>
        ///     Get Role details by RoleName
        /// </summary>
        /// <returns>
        ///     ENTRole  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetRoleByName/{RoleName}")]
        public BaseResponse<ENTRole> GetRoleByName(string RoleName)
        {
            var response = new BaseResponse<ENTRole>(true);
            response.Data = _RoleService.GetRole(RoleName);
            return response;
        }

        /// <summary>
        ///     Add/Update Role Details
        /// </summary>
        /// <returns>
        ///     ENTRole Model
        /// </returns>
        [HttpPost("ManageRole")]
        public BaseResponse<ENTRole> ManageRole([FromBody] ENTRole Role)
        {
            BaseResponse<ENTRole> response = new BaseResponse<ENTRole>();
            var serviceResponse = new ActionStatus();

            serviceResponse = _RoleService.CheckRoleNameExists(Role.RoleName, Role.RoleId ?? 0);
            if (!serviceResponse.Success)
            {
                if (Role.RoleId <= 0 || Role.RoleId == null)
                {
                    Role.IsActive = true;
                    Role.IsDeleted = false;
                    Role.CreatedBy = CurrentUserId;
                    serviceResponse = _RoleService.Insert(Role);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "Role added successfully";
                        response.Data = serviceResponse.Result as ENTRole;
                        ClearRoleCaches();
                    }
                }
                else
                {
                    Role.UpdatedBy = CurrentUserId;
                    serviceResponse = _RoleService.Update(Role);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "Role updated successfully";
                        response.Data = serviceResponse.Result as ENTRole;
                        ClearRoleCaches();
                    }
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "Role already exists with same role name.";
                response.Data = null;
            }
            return response;
        }


        /// <summary>
        ///     Delete role by role id
        /// </summary>
        /// <returns>
        /// </returns>
        // PATCH api/User/DeleteRolebyRoleId
        [HttpPatch("DeleteRolebyRoleId")]
        public BaseResponse<ActionStatus> DeleteRolebyRoleId([FromBody] ENTPatchRequest activeStatus)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                activeStatus.UpdatedBy = CurrentUserId;
                var serviceResponse = _RoleService.DeleteRole(activeStatus);
                if (serviceResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Message = "Role deleted successfully";
                    ClearRoleCaches();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Some error occurred deleting role";
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

        private void ClearRoleCaches()
        {
            // 🔑 MUST MATCH EXACT KEYS USED IN LookupController
            _cacheService.Remove(LookupCacheKeys.ROLES_KEY); // Main roles lookup cache
            _cacheService.Remove(LookupCacheKeys.CUST_REASSIGN_ROLES_KEY); // Customer reassignment roles cache
        }
    }
}