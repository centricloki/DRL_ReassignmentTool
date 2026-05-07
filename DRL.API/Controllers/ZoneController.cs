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
    [Route("api/Zone")]
    public class ZoneController : BaseController
    {
        private static readonly HttpClient _HttpClient = new HttpClient();
        private readonly IZoneService _ZoneService;
        private readonly CommonHelper _commonHelper = new CommonHelper();
        private IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        public ZoneController(IZoneService ZoneService, IConfiguration configuration, ICacheService cacheService)
        {
            _ZoneService = ZoneService;
            _configuration = configuration;
            _cacheService = cacheService;
        }

        /// <summary>
        ///     Get All Zone List
        /// </summary>
        /// <returns>
        ///     ENTZoneResponse Model
        /// </returns>
        [HttpGet("GetAllZones")]
        public BaseResponse<List<ENTZoneResponse>> GetAllZones()
        {
            var response = new BaseResponse<List<ENTZoneResponse>>(true);
            response.Data = _ZoneService.GetZoneList();
            return response;
        }

        /// <summary>
        ///     Get Zone details by ZoneId
        /// </summary>
        /// <returns>
        ///     ENTZone Model
        /// </returns>
        [HttpGet("GetZone/{ZoneId}")]
        public BaseResponse<ENTZone> GetZone(long ZoneId)
        {
            var response = new BaseResponse<ENTZone>(true);
            response.Data = _ZoneService.GetZone((int)ZoneId);
            return response;
        }

        /// <summary>
        ///     Add/Update Zone Details
        /// </summary>
        /// <returns>
        ///     ENTZone Model
        /// </returns>
        [HttpPost("ManageZone")]
        public BaseResponse<ENTZone> ManageZone([FromBody] ENTZone Zone)
        {
            BaseResponse<ENTZone> response = new BaseResponse<ENTZone>();
            var serviceResponse = new ActionStatus();
            Zone.ImportedFrom = 0;
            serviceResponse = _ZoneService.CheckZoneNameExists(Zone.ZoneName, Zone.ZoneId);
            if (!serviceResponse.Success)
            {
                if (Zone.ZoneId <= 0 || Zone.ZoneId == null)
                {
                    serviceResponse = _ZoneService.Insert(Zone);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "Zone added successfully";
                        response.Data = serviceResponse.Result as ENTZone;
                        ClearZoneCaches();
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = serviceResponse.Message ?? "Failed to add zone. Please ensure all required fields are filled correctly.";
                        response.Data = null;
                    }
                }
                else
                {
                    serviceResponse = _ZoneService.Update(Zone);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "Zone updated successfully";
                        response.Data = serviceResponse.Result as ENTZone;
                        ClearZoneCaches();
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = serviceResponse.Message ?? "Failed to update zone. Please ensure all required fields are filled correctly.";
                        response.Data = null;
                    }
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "Zone already exists with same zone name.";
                response.Data = null;
            }
            return response;
        }

        /// <summary>
        ///     Delete zone by zone id
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPatch("DeleteZonebyZoneId")]
        public BaseResponse<ActionStatus> DeleteZonebyZoneId([FromBody] ENTPatchRequest activeStatus)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                activeStatus.UpdatedBy = CurrentUserId;
                var serviceResponse = _ZoneService.DeleteZone(activeStatus);
                if (serviceResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Message = "Zone deleted successfully";
                    ClearZoneCaches();
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
        ///     Active/InActive Zone
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPatch("ManageZoneStatus")]
        public BaseResponse<ActionStatus> ManageZoneStatus([FromBody] ENTPatchRequest activeStatus)
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
                var serviceResponse = _ZoneService.ManageZoneStatus(activeStatus);
                if (serviceResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Message = "Zone updated successfully";
                    ClearZoneCaches();
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

        private void ClearZoneCaches()
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