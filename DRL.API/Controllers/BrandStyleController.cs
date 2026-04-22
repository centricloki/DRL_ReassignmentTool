using DRL.Core.Interface;
using DRL.Entity;
using DRL.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using DRL.API.Extensions;

namespace DRL.API.Controllers
{
    [CustomAuthorizeAttribute]
    [Route("api/BrandStyle")]
    [ApiController]
    public class BrandStyleController : BaseController
    {
        private readonly CommonHelper _commonHelper = new CommonHelper();
        private readonly IBrandStyleService _brandStyleService;

        public BrandStyleController(IBrandStyleService brandStyleService)
        {
            _brandStyleService = brandStyleService;
        }

        [HttpGet("BrandStyles")]
        public BaseResponse<List<ENTBrandStyleMaster>> GetBrandStyles()
        {
            var response = new BaseResponse<List<ENTBrandStyleMaster>>(true);
            response.Data = _brandStyleService.GetBrandStyleMaster();
            return response;
        }

        [HttpPost("ChangeSortOrder/{brandStyleId}/{sortOrder}")]
        public BaseResponse<ActionStatus> ChangeSortOrder(int brandStyleId, int sortOrder)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                if (brandStyleId > 0)
                {
                    var serviceResponse = _brandStyleService.UpdateBrandStyleMaster(brandStyleId, sortOrder);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "Sort order changed successfully";
                    }
                    else if (!serviceResponse.Success && !string.IsNullOrWhiteSpace(serviceResponse.Message))
                    {
                        response.IsSuccess = false;
                        response.Message = serviceResponse.Message;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Some error occurred updating sort order";
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "BrandStyleMasterId and SortOrder both are required";
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
    }
}
