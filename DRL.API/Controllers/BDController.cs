using DRL.API.Extensions;
using DRL.Core.Interface;
using DRL.Entity;
using DRL.Entity.Response;
using DRL.Library;
using DRL.Model.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DRL.API.Controllers
{
    [CustomAuthorizeAttribute]
    [Route("api/BD")]
    public class BDController : BaseController
    {
        private readonly IBDService _bdService;
        private readonly ITerritoryService _territoryService;
        private readonly CommonHelper _commonHelper = new CommonHelper();
        private IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        public BDController(IBDService bdService, IConfiguration configuration, ICacheService cacheService, ITerritoryService territoryService)
        {
            _bdService = bdService;
            _configuration = configuration;
            _cacheService = cacheService;
            _territoryService = territoryService;
        }

        [HttpGet("GetAllBDs")]
        public BaseResponse<List<ENTBDResponse>> GetAllBDs()
        {
            var response = new BaseResponse<List<ENTBDResponse>>(true);
            response.Data = _bdService.GetBDList();
            return response;
        }

        [HttpGet("GetBD/{bdId}")]
        public BaseResponse<ENTBD> GetBD(long bdId)
        {
            var response = new BaseResponse<ENTBD>(true);
            var bd = _bdService.GetBD((int)bdId);
            if (bd != null)
            {
                bd.Teams = _territoryService.GetAllBDTerritories((int)bdId);
            }
            response.Data = bd;
            return response;
        }

        [HttpPost("ManageBD")]
        public BaseResponse<ENTBD> ManageBD([FromBody] ENTBD bd)
        {
            BaseResponse<ENTBD> response = new BaseResponse<ENTBD>();
            var serviceResponse = new ActionStatus();

            serviceResponse = _bdService.CheckBDNameExists(bd.BDName, bd.BDID);
            if (!serviceResponse.Success)
            {
                if (bd.BDID <= 0)
                    bd.CreatedBy = CurrentUserId;
                else
                    bd.UpdatedBy = CurrentUserId;

                serviceResponse = bd.BDID <= 0 ? _bdService.Insert(bd) : _bdService.Update(bd);
                
                if (serviceResponse.Success)
                {
                    var savedBD = serviceResponse.Result as ENTBD;
                    if (savedBD != null)
                    {
                        savedBD.Teams = _territoryService.GetAllBDTerritories(savedBD.BDID);
                    }
                    response.IsSuccess = true;
                    response.Message = (bd.BDID <= 0) ? "BD added successfully" : "BD updated successfully";
                    response.Data = savedBD;
                    ClearBDCaches();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = serviceResponse.Message ?? "Failed to save BD. Please ensure all required fields are filled correctly.";
                    response.Data = null;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "BD already exists with same name.";
                response.Data = null;
            }
            return response;
        }

        [HttpPatch("DeleteBDbyBDId")]
        public BaseResponse<ActionStatus> DeleteBDbyBDId([FromBody] ENTPatchRequest activeStatus)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                activeStatus.UpdatedBy = CurrentUserId;
                var serviceResponse = _bdService.DeleteBD(activeStatus);
                if (serviceResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Message = "BD deleted successfully";
                    ClearBDCaches();
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

        [HttpPatch("ManageBDStatus")]
        public BaseResponse<ActionStatus> ManageBDStatus([FromBody] ENTPatchRequest activeStatus)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                activeStatus.UpdatedBy = CurrentUserId;
                var serviceResponse = _bdService.ManageBDStatus(activeStatus);
                if (serviceResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Message = "BD updated successfully";
                    ClearBDCaches();
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

        private void ClearBDCaches()
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
