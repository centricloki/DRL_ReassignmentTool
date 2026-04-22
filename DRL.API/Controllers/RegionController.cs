using DRL.API.Extensions;
using DRL.Core.Interface;
using DRL.Entity;
using DRL.Entity.Response;
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
    [Route("api/Region")]
    public class RegionController : BaseController
    {
        private static readonly HttpClient _HttpClient = new HttpClient();
        private readonly IRegionService _RegionService;
        private readonly CommonHelper _commonHelper = new CommonHelper();
        private IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        public RegionController(IRegionService RegionService, IConfiguration configuration
            ,ICacheService cacheService)
        {
            _RegionService = RegionService;
            _configuration = configuration;
            _cacheService = cacheService;
        }

        /// <summary>
        ///     Get All Region List
        /// </summary>
        /// <returns>
        ///     ENTRegion  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetAllRegions")]
        public BaseResponse<List<ENTRegionResponse>> GetAllRegions()
        {
            var response = new BaseResponse<List<ENTRegionResponse>>(true);
            response.Data = _RegionService.GetRegionList();
            return response;
        }

        /// <summary>
        ///     Get Region details by RegionId
        /// </summary>
        /// <returns>
        ///     ENTRegion  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetRegion/{RegionId}")]
        public BaseResponse<ENTRegion> GetRegion(long RegionId)
        {
            var response = new BaseResponse<ENTRegion>(true);
            response.Data = _RegionService.GetRegion(RegionId);
            return response;
        }

        /// <summary>
        ///     Add/Update Region Details
        /// </summary>
        /// <returns>
        ///     ENTRegion Model
        /// </returns>
        [HttpPost("ManageRegion")]
        public BaseResponse<ENTRegion> ManageRegion([FromBody] ENTRegion Region)
        {
            BaseResponse<ENTRegion> response = new BaseResponse<ENTRegion>();
            var serviceResponse = new ActionStatus();
            Region.ImportedFrom = 0;
            serviceResponse = _RegionService.CheckRegionNameExists(Region.Regioname, Region.RegionId ?? 0);
            if (!serviceResponse.Success)
            {
                if (Region.RegionId <= 0 || Region.RegionId == null)
                {
                    Region.CreatedBy = CurrentUserId;
                    serviceResponse = _RegionService.Insert(Region);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "Region added successfully";
                        response.Data = serviceResponse.Result as ENTRegion;
                        ClearRegionCaches();
                    }
                }
                else
                {
                    Region.UpdatedBy = CurrentUserId;
                    serviceResponse = _RegionService.Update(Region);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "Region updated successfully";
                        response.Data = serviceResponse.Result as ENTRegion;
                        ClearRegionCaches();
                    }
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "Region already exists with same region name.";
                response.Data = null;
            }
            return response;
        }


        /// <summary>
        ///     Delete region by region id
        /// </summary>
        /// <returns>
        /// </returns>
        // PATCH api/Region/DeleteRegionbyRegionId
        [HttpPatch("DeleteRegionbyRegionId")]
        public BaseResponse<ActionStatus> DeleteRegionbyRegionId([FromBody] ENTPatchRequest activeStatus)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                activeStatus.UpdatedBy = CurrentUserId;
                var serviceResponse = _RegionService.DeleteRegion(activeStatus);
                if (serviceResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Message = "Region deleted successfully";
                    ClearRegionCaches();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = serviceResponse.Message;
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
        ///     Active/InActive Region
        /// </summary>
        /// <returns>
        /// </returns>
        // PATCH api/Region/ManageRegionStatus
        [HttpPatch("ManageRegionStatus")]
        public BaseResponse<ActionStatus> ManageRegionStatus([FromBody] ENTPatchRequest activeStatus)
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
                var serviceResponse = _RegionService.ManageRegionStatus(activeStatus);
                if (serviceResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Message = "Region updated successfully";
                    ClearRegionCaches();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = serviceResponse.Message;
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

        private void ClearRegionCaches()
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