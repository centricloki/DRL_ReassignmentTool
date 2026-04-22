using DRL.API.Extensions;
using DRL.Core.Interface;
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
    [Route("api/Territory")]
    //[ApiController]
    public class TerritoryController : BaseController
    {
        private readonly ITerritoryService _territoryService;
        private readonly CommonHelper _commonHelper = new CommonHelper();
        private IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        public TerritoryController(ITerritoryService TerritoryService
            , IConfiguration configuration
            , IMemoryCache cache
            , ICacheService cacheService)
        {
            _territoryService = TerritoryService;
            _configuration = configuration;
            _cacheService = cacheService;
        }



        /// <summary>
        ///     Get All Territory List
        /// </summary>
        /// <returns>
        ///     ENTTeam  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetAllTerritories")]
        public BaseResponse<List<ENTTeam>> GetAllTerritories()
        {
            var response = new BaseResponse<List<ENTTeam>>(true);
            response.Data = _territoryService.GetAllTerritories();
            return response;
        }

        /// <summary>
        ///     Get Territory details by TerritoryId
        /// </summary>
        /// <returns>
        ///     ENTTeam  Model
        /// </returns>
        // GET api/values
        [HttpGet("GetTerritory/{TerritoryId}")]
        public BaseResponse<ENTTeam> GetTerritory(long TerritoryId)
        {
            var response = new BaseResponse<ENTTeam>(true);
            response.Data = _territoryService.GetTerritory(TerritoryId);
            return response;
        }

        /// <summary>
        ///     Add/Update Territory Details
        /// </summary>
        /// <returns>
        ///     ENTTeam Model
        /// </returns>
        [HttpPost("ManageTerritory")]
        public BaseResponse<ENTTeam> ManageTerritory([FromBody]ENTTeam Territory)
        {
            BaseResponse<ENTTeam> response = new BaseResponse<ENTTeam>();
            var serviceResponse = new ActionStatus();

            serviceResponse = _territoryService.CheckTerritoryNameExists(Territory.Name, Territory.TeamId ?? 0);
            if (!serviceResponse.Success)
            {
                if (Territory.TeamId <= 0 || Territory.TeamId == null)
                {
                    Territory.CreatedBy = CurrentUserId;
                    serviceResponse = _territoryService.Insert(Territory);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "Team added successfully";
                        response.Data = serviceResponse.Result as ENTTeam;
                        ClearTerritoryCaches();
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = serviceResponse.Message;
                        response.Data = null;
                    }
                }
                else
                {
                    Territory.UpdatedBy = CurrentUserId;
                    serviceResponse = _territoryService.Update(Territory);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "Team updated successfully";
                        response.Data = serviceResponse.Result as ENTTeam;
                        ClearTerritoryCaches();
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = serviceResponse.Message;
                        response.Data = null;
                    }
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "Team already exists with same team name.";
                response.Data = null;
            }
            return response;
        }

        /// <summary>
        ///     Delete territory by territory id
        /// </summary>
        /// <returns>
        /// </returns>
        // PATCH api/User/DeleteTerritorybyterritoryId
        [HttpPatch("DeleteTerritorybyTerritoryId")]
        public BaseResponse<ActionStatus> DeleteTerritorybyTerritoryId([FromBody]ENTPatchRequest activeStatus)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                activeStatus.UpdatedBy = CurrentUserId;
                var serviceResponse = _territoryService.DeleteTerritory(activeStatus);
                if (serviceResponse.Success)
                {
                    response.IsSuccess = true;
                    response.Message = "Territory deleted successfully";
                    ClearTerritoryCaches();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Some error occurred deleting territory";
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

        [HttpGet("GetTeamListFromRegionId/{RegionId}")]
        public BaseResponse<List<ENTTeam>> GetTeamListFromRegionId(long regionId)
        {
            var response = new BaseResponse<List<ENTTeam>>(true);
            try
            {
                response.Data = _territoryService.GetTeamListFromRegionId(regionId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
            return response;
        }
        private void ClearTerritoryCaches()
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