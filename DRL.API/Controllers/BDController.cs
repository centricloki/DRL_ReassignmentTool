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
                {
                    bd.CreatedBy = CurrentUserId;
                    serviceResponse = _bdService.Insert(bd);
                    if (serviceResponse.Success)
                    {
                        var createdBD = serviceResponse.Result as ENTBD;
                        if (bd.Teams != null && createdBD != null)
                        {
                            var territoryIds = bd.Teams.Where(x => x.TeamId.HasValue).Select(x => x.TeamId.Value).ToList();
                            _territoryService.SyncBDTerritories(createdBD.BDID, territoryIds, CurrentUserId);
                            createdBD.Teams = _territoryService.GetAllBDTerritories(createdBD.BDID);
                        }
                        response.IsSuccess = true;
                        response.Message = "BD added successfully";
                        response.Data = createdBD;
                        ClearBDCaches();
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = serviceResponse.Message ?? "Failed to add BD. Please ensure all required fields are filled correctly.";
                        response.Data = null;
                    }
                }
                else
                {
                    bd.UpdatedBy = CurrentUserId;
                    serviceResponse = _bdService.Update(bd);
                    if (serviceResponse.Success)
                    {
                        if (bd.Teams != null)
                        {
                            var territoryIds = bd.Teams.Where(x => x.TeamId.HasValue).Select(x => x.TeamId.Value).ToList();
                            _territoryService.SyncBDTerritories(bd.BDID, territoryIds, CurrentUserId);
                        }
                        var updatedBD = serviceResponse.Result as ENTBD;
                        if (updatedBD != null)
                        {
                            updatedBD.Teams = _territoryService.GetAllBDTerritories(updatedBD.BDID);
                        }
                        response.IsSuccess = true;
                        response.Message = "BD updated successfully";
                        response.Data = updatedBD;
                        ClearBDCaches();
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = serviceResponse.Message ?? "Failed to update BD. Please ensure all required fields are filled correctly.";
                        response.Data = null;
                    }
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
            _cacheService.Remove(LookupCacheKeys.BDS_KEY);
        }
    }
}
