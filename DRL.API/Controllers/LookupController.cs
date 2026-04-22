using DRL.API.Extensions;
using DRL.Core.Interface;
using DRL.Core.Service;
using DRL.Entity;
using DRL.Library;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // Added for IMemoryCache
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DRL.API.Controllers
{
    [CustomAuthorizeAttribute]
    [Route("api/Lookup")]
    public class LookupController : BaseController
    {
        private readonly ICacheService _cacheService;
        private readonly IRoleService _roleService;
        private readonly IRegionService _regionService;
        private readonly ITerritoryService _territoryService;
        private readonly IZoneService _zoneService;
        private readonly ICityService _cityService;
        private readonly IStateService _stateService;
        private readonly IUserService _userService;
        private readonly IAVPService _avpService;
        private readonly IBDService _bdService;
        private readonly IConfiguration _configuration;
        private readonly CommonHelper _commonHelper = new CommonHelper();
        public LookupController(
            ICacheService cacheService,
            IRoleService roleService,
            IConfiguration configuration,
            IZoneService zoneService,
            IRegionService regionService,
            ITerritoryService territoryService,
            ICityService cityService,
            IStateService stateService,
            IUserService userService,
            IAVPService avpService,
            IBDService bdService)
        {
            _cacheService = cacheService;
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _regionService = regionService ?? throw new ArgumentNullException(nameof(regionService));
            _territoryService = territoryService ?? throw new ArgumentNullException(nameof(territoryService));
            _zoneService = zoneService ?? throw new ArgumentNullException(nameof(zoneService));
            _cityService = cityService ?? throw new ArgumentNullException(nameof(cityService));
            _stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _avpService = avpService ?? throw new ArgumentNullException(nameof(avpService));
            _bdService = bdService ?? throw new ArgumentNullException(nameof(bdService));
        }

        [HttpGet("GetUsers")]
        public BaseResponse<List<ENTLookUpItem>> GetUsers()
        {
            // Not cached: User list is dynamic and potentially large
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            response.Data = (from u in _userService.GetAllUsers()
                             select new ENTLookUpItem
                             {
                                 RecordId = u.UserId.ToString(),
                                 Value = $"{u.FirstName} {u.LastName}"
                             }).OrderBy(u => u.Value).ToList();
            return response;
        }

        [HttpGet("GetRoles")]
        public BaseResponse<List<ENTLookUpItem>> GetRoles()
        {
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            response.Data = _cacheService.GetOrCreate(
   LookupCacheKeys.ROLES_KEY,
   () => _roleService.GetActiveRoles(),
   TimeSpan.FromMinutes(LookupCacheKeys.CACHE_MINUTES)
);
            return response;
        }

        [HttpGet("GetRegions")]
        public BaseResponse<List<ENTLookUpItem>> GetRegions()
        {
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            response.Data = _cacheService.GetOrCreate(
    LookupCacheKeys.REGIONS_KEY,
    () => _regionService.GetAllRegionLookup(),
    TimeSpan.FromMinutes(LookupCacheKeys.CACHE_MINUTES)
);
            return response;
        }

        [HttpGet("GetZones")]
        public BaseResponse<List<ENTLookUpItem>> GetZones()
        {
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            response.Data = _cacheService.GetOrCreate(
      LookupCacheKeys.ZONES_KEY,
      () => _zoneService.GetAllZoneLookup(),
      TimeSpan.FromMinutes(LookupCacheKeys.CACHE_MINUTES)
  );
            return response;
        }

        [HttpGet("Zones/{userId}")]
        public BaseResponse<List<ENTLookUpItem>> GetZonesByUserId(long userId)
        {
            // Not cached: User-specific data with high variability
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            response.Data = _zoneService.GetAllZoneLookupByAVP(userId);
            return response;
        }

        [HttpGet("GetTerritories")]
        public BaseResponse<List<ENTLookUpItem>> GetTerritories()
        {
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            response.Data = _cacheService.GetOrCreate(
                LookupCacheKeys.TERRITORIES_KEY,
                () => _territoryService.GetAllTerritoryLookup(),
                TimeSpan.FromMinutes(LookupCacheKeys.CACHE_MINUTES)
            );
            return response;
        }

        [HttpGet("GetTerritories/{userId}")]
        public BaseResponse<List<ENTLookUpItem>> GetTerritoriesByUserId(long userId)
        {
            // Not cached: User-specific data
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            response.Data = _territoryService.GetAllTerritoryLookupByUserId(userId);
            return response;
        }

        [HttpGet("Cities/{state}")]
        public BaseResponse<List<ENTLookUpItem>> GetCities(string state)
        {
            // Cache with normalized key (case-insensitive state handling)
            var normalizedState = state?.Trim().ToUpperInvariant() ?? string.Empty;
            var cacheKey = $"{LookupCacheKeys.CITIES_KEY_PREFIX}{normalizedState}";
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            List<ENTLookUpItem> cities = null;
            response.Data = _cacheService.GetOrCreate(cacheKey,
            () =>
                {
                    cities = _cityService.GetCitiesLookup(state);
                    return cities ?? new List<ENTLookUpItem>();
                },
            TimeSpan.FromMinutes(LookupCacheKeys.CACHE_MINUTES)
            );
            return response;
        }

        [HttpGet("States")]
        public BaseResponse<List<ENTLookUpItem>> GetStates()
        {
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            response.Data = _cacheService.GetOrCreate(
               LookupCacheKeys.STATES_KEY,
               () => _stateService.GetStatesLookup(),
               TimeSpan.FromMinutes(LookupCacheKeys.CACHE_MINUTES)
           );
            return response;
        }

        [HttpGet("CustomerReassignment/Roles")]
        public BaseResponse<List<ENTLookUpItem>> GetCustomerReassignmentRoles()
        {
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            response.Data = _cacheService.GetOrCreate(
                           LookupCacheKeys.CUST_REASSIGN_ROLES_KEY,
                           () => _roleService.GetCustomerReassignmentRoles(),
                           TimeSpan.FromMinutes(LookupCacheKeys.CACHE_MINUTES)
                       );
            return response;
        }

        [HttpGet("CustomerReassignment/Territories/{roleId}")]
        public BaseResponse<List<ENTLookUpItem>> GetCustReassignTerritoriesByRoleIds(string[] roleId)
        {
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            
            response.Data = _territoryService.GetCustReassignTerritoriesByRoleIds(roleId);
            return response;
        }

        [HttpGet("CustomerReassignment/Team/{roleId}")]
        public BaseResponse<List<ENTLookUpItem>> GetCustomerReassignmentTeam(string[] roleId)
        {
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            response.Data = _territoryService.GetCustReassignTeamsByRoleIds(roleId);
            return response;
        }

        [HttpGet("AVPs")]
        public BaseResponse<List<ENTLookUpItem>> GetAVPs()
        {
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            response.Data = _cacheService.GetOrCreate(
                           LookupCacheKeys.AVPS_KEY,
                           () => _avpService.GetAVPsLookup(),
                           TimeSpan.FromMinutes(LookupCacheKeys.CACHE_MINUTES)
                       );
            return response;
        }

        [HttpGet("BDs")]
        public BaseResponse<List<ENTLookUpItem>> GetBDs()
        {
            var response = new BaseResponse<List<ENTLookUpItem>>(true);
            response.Data = _cacheService.GetOrCreate(
                           LookupCacheKeys.BDS_KEY,
                           () => _bdService.GetBDsLookup(),
                           TimeSpan.FromMinutes(LookupCacheKeys.CACHE_MINUTES)
                       );
            return response;
        }
    }
}