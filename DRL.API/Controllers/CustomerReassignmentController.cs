using DRL.Core.Interface;
using DRL.Entity;
using DRL.Library;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DRL.Entity.Response;
using Microsoft.AspNetCore.Authorization;
using DRL.API.Extensions;

namespace DRL.API.Controllers
{
    [CustomAuthorizeAttribute]
    [Route("api/CustomerReassignment")]
    //[ApiController]
    public class CustomerReassignmentController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly CommonHelper _commonHelper = new CommonHelper();
        private IConfiguration _configuration;

        public CustomerReassignmentController(IUserService userService, IConfiguration configuration, ICustomerService customerService)
        {
            _userService = userService;
            _customerService = customerService;
            _configuration = configuration;
        }
        /// <summary>
        ///     Get Customer Reassignment Customers
        /// </summary>
        /// <returns>
        ///     ENTTeam  Model
        /// </returns>
        // GET api/values
        [HttpPost("Customers")]
        public BaseResponse<List<ENTCustomerList>> GetCustomer([FromBody] ENTCustomerRequest request)
        {
            var response = new BaseResponse<List<ENTCustomerList>>(true);
            response.Data = _customerService.GetAllCustomer(request);
            return response;
        }

        /// <summary>
        ///       Delete Customer Reassignment Customers
        /// </summary>
        /// <returns>
        /// </returns>
        // PATCH api/CustomerReassign/DeleteCustomers
        [HttpPatch("DeleteCustomers")]
        public BaseResponse<ActionStatus> DeleteCustomer([FromBody] ENTPatchCustomerRequest request)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                if (request != null && request.Id?.Any() == true)
                {
                    request.UpdatedBy = CurrentUserId;
                    var serviceResponse = _customerService.DeleteCustomer(request);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "Customer deleted successfully";
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Some error occurred deleting Customer";
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Please enter some values";
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
        ///       Activate Customer Reassignment Customers
        /// </summary>
        /// <returns>
        /// </returns>
        // PATCH api/CustomerReassign/ActivateCustomers
        [HttpPatch("ActivateCustomers")]
        public BaseResponse<ActionStatus> ActivateCustomer([FromBody] ENTPatchCustomerRequest request)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                if (request != null && request.Id?.Any() == true)
                {
                    request.UpdatedBy = CurrentUserId;
                    var serviceResponse = _customerService.ActivateCustomer(request);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "Customer activated successfully";
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Some error occurred activating Customer";
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Please enter some values";
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
        ///     Get Customer Reassignment Users
        /// </summary>
        /// <returns>
        ///     ENTUser  Model
        /// </returns>
        // GET api/values
        [HttpGet("Users/{page}/{pageSize}/{territory}/{userName}")]
        public BaseResponse<List<ENTReassignmentUsers>> GetReassignmentUser(int? page, int? pageSize, int? territory, string userName = "")
        {
            var response = new BaseResponse<List<ENTReassignmentUsers>>(true);
            response.Data = _userService.GetReassignUsers(Convert.ToInt32(page), Convert.ToInt32(pageSize), territory == 0 ? "" : territory.ToString(), userName == "null" || userName == null ? "" : userName);
            return response;
        }

        /// <summary>
        ///    Changes Customer Reassignment Customer
        /// </summary>
        /// <returns>
        ///     
        /// </returns>
        // PUT ChangeCustomerDetails
        [HttpPost("ChangeCustomerDetails")]
        public BaseResponse<ActionStatus> ChangeCustomerDetails([FromBody] ENTUpdateCustomerRequest request)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                if (request.customerIds?.Any() == true)
                {
                    request.UpdatedBy = CurrentUserId;
                    var serviceResponse = _customerService.ChangeCustomerDetails(request);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "Customer(s) changed successfully";
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
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Customer list required";
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
        ///   Changes Customer Reassignment User
        /// </summary>
        /// <returns>
        ///     
        /// </returns>
        // PUT ChangeUserDetails
        [HttpPost("ChangeUserDetails")]
        public BaseResponse<ActionStatus> ChangeUserDetails([FromBody] ENTChangeUserDetailsRequest request)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                if (request.userIds?.Any() == true)
                {
                    request.UpdatedBy = CurrentUserId;
                    var serviceResponse = _userService.ChangeUserDetails(request);
                    if (serviceResponse.Success)
                    {
                        response.IsSuccess = true;
                        response.Message = "User(s) changed successfully";
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
                else
                {
                    response.IsSuccess = false;
                    response.Message = "User list required";
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
        ///     Get Customer Reassignment Action History
        /// </summary>
        /// <returns>
        ///     ENTTeam  Model
        /// </returns>
        // GET api/values
        //[HttpGet("ActionHistory")]
        //public BaseResponse<List<ENTActionHistoryResponse>> GetActionHistory()
        //{
        //    var response = new BaseResponse<List<ENTActionHistoryResponse>>(true);
        //    response.Data = _customerService.GetActionHistory();
        //    return response;
        //}
        [HttpPost("ActionHistory")]
        public async Task<BaseResponse<KendoGridDataResult<ENTActionHistoryResponse>>> GetActionHistory([FromBody] KendoGridRequest request)
        {
            try
            {
               KendoGridDataResult<ENTActionHistoryResponse> result = await _customerService.GetActionHistoriesWithPaginationAsync(request);
                return new BaseResponse<KendoGridDataResult<ENTActionHistoryResponse>> { Data = result, IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new BaseResponse<KendoGridDataResult<ENTActionHistoryResponse>> { Message = ex.Message, IsSuccess = false };
            }
        }

        /// <summary>
        ///    Add Customer Master
        /// </summary>
        /// <returns>
        ///     
        /// </returns>
        // POST AddCustomers
        [HttpPost("AddCustomers")]
        public async Task<BaseResponse<ActionStatus>> AddCustomerMasterDetails([FromBody] List<CustomerMasterRequest> requests)
        {
            BaseResponse<ActionStatus> response = new BaseResponse<ActionStatus>();
            try
            {
                if (requests != null && requests.Any())
                {
                    List<string> existingRecords = new List<string>(); // Collect existing records for error message
                    List<string> collectErrors = new List<string>(); // Collect error messages
                    int rowCount = 1;
                    foreach (var request in requests)
                    {
                        rowCount += 1;
                        if (!string.IsNullOrWhiteSpace(request.CustomerName))
                        {
                            // Check if the customer already exists based on CustomerName, Latitude, and Longitude
                            var existingCustomer = await _customerService.GetCustomerAsync(SanitizeForSql(request.CustomerName), SanitizeForSql(request.Address)
                                , SanitizeForSql(request.AddressCity), SanitizeForSql(request.AddressState), SanitizeForSql(request.AddressZipCode));
                            if (existingCustomer != null && existingCustomer.Count > 0)
                            {
                                // Add to existing records list to avoid insertion
                                existingRecords.Add($"Row {rowCount}: {request.CustomerName} (Address: {request.Address})");
                            }
                            else
                            {
                                // Proceed to add customer if not found in the database
                                var serviceResponse = _customerService.AddCustomerMaster(request);
                                if (serviceResponse.Success)
                                {
                                    response.IsSuccess = true;
                                    response.Message = "Customer(s) added successfully";
                                }
                                else if (!serviceResponse.Success && !string.IsNullOrWhiteSpace(serviceResponse.Message))
                                {
                                    //response.IsSuccess = false;
                                    //response.Message = serviceResponse.Message;
                                    //break; // Optionally break the loop if any request fails
                                    // Add to existing records list to avoid insertion
                                    collectErrors.Add($"Row {rowCount}: {request.CustomerName} (Error: {serviceResponse.Message})");
                                }
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "Invalid customer details provided.";
                            break;
                        }
                    }

                    // If any existing records were found, return a specific error message
                    if (existingRecords.Any())
                    {
                        response.IsSuccess = false;
                        response.Message = $"The following customer(s) already exist: {string.Join(", ", existingRecords)}";
                    }
                    if (collectErrors.Any())
                    {
                        response.IsSuccess = false;
                        if (existingRecords.Any())
                            response.Message += $"The following customer(s) could not be saved: {string.Join(", ", collectErrors)}";
                        else
                            response.Message = $"The following customer(s) could not be saved: {string.Join(", ", collectErrors)}";
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Customer list required";
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
        /// Sanitizes string for SQL storage - escapes special characters and trims spaces
        /// </summary>
        private string SanitizeForSql(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Trim leading and trailing spaces
            string trimmed = input.Trim();

            // Optional: Remove extra internal spaces (multiple spaces to single space)
            trimmed = System.Text.RegularExpressions.Regex.Replace(trimmed, @"\s+", " ");

            return trimmed;
        }
    }
}
