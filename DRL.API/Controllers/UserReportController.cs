using DRL.API.Extensions;
using DRL.Core.Interface;
using DRL.Entity;
using DRL.Library;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using System;
using System.Collections.Generic;

namespace DRL.API.Controllers
{
    [CustomAuthorizeAttribute]
    [Route("api/UserReport")]
    [ApiController]
    public class UserReportController : BaseController
    {
        private readonly CommonHelper _commonHelper = new CommonHelper();
        private readonly IUserReportService _userReportService;
        private readonly ICacheService _cacheService;

        public UserReportController(IUserReportService userReportService, ICacheService cacheService)
        {
            _userReportService = userReportService;
            _cacheService = cacheService;
        }

        [HttpGet("GetUserReport")]
        public BaseResponse<List<ENTUserReportList>> GetUserReportData()
        {
            var cacheKey = $"{LookupCacheKeys.USER_REPORT_CACHE_PREFIX}all";

            var userReports = _cacheService.GetOrCreate(
                cacheKey,
                () => _userReportService.GetUserReport(),
                TimeSpan.FromMinutes(LookupCacheKeys.CACHE_MINUTES)
            );

            _cacheService.RegisterKey(cacheKey, LookupCacheKeys.USER_REPORT_CACHE_PREFIX);

            return new BaseResponse<List<ENTUserReportList>>(true) { Data = userReports };


        }

        [HttpGet("GetUserReportByRoleId/{roleId}/{entityId}")]
        public BaseResponse<List<ENTUserReportList>> GetUserReportData(int roleId, int entityId)
        {
            var cacheKey = $"{LookupCacheKeys.USER_REPORT_CACHE_PREFIX}{roleId}_{entityId}";
            var userReports = _cacheService.GetOrCreate(
                cacheKey,
                () => _userReportService.GetUserReport(roleId, entityId),
                TimeSpan.FromMinutes(LookupCacheKeys.CACHE_MINUTES)
            );
            _cacheService.RegisterKey(cacheKey, LookupCacheKeys.USER_REPORT_CACHE_PREFIX);

            return new BaseResponse<List<ENTUserReportList>>(true) { Data = userReports };
        }

        [HttpGet("GetUserReportHierarchy")]
        public BaseResponse<List<ENTUserReportHierarchyNode>> GetUserReportHierarchy()
        {
            var cacheKey = $"{LookupCacheKeys.USER_REPORT_HIERARCHY_CACHE_PREFIX}full";

            var hierarchy = _cacheService.GetOrCreate(
                cacheKey,
                () => _userReportService.GetUserReportHierarchy(),
                TimeSpan.FromMinutes(LookupCacheKeys.CACHE_MINUTES)
            );

            _cacheService.RegisterKey(cacheKey, LookupCacheKeys.USER_REPORT_HIERARCHY_CACHE_PREFIX);

            return new BaseResponse<List<ENTUserReportHierarchyNode>>(true) { Data = hierarchy };
        }
    }
}
