using DRL.Core.Interface;
using DRL.Entity;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DRL.Model.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using DRL.Entity.Response;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Reflection.Emit;

namespace DRL.Core.Service
{
    public class CustomerService : ICustomerService
    {

        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper CommonHelper;
        private readonly IConfiguration _configuration;

        public CustomerService(IUnitOfWork unitofwork, ICustomerRepository customerRepository, ILogManager logManager, IConfiguration configuration)
        {
            _customerRepository = customerRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            CommonHelper = new CommonHelper();
            _configuration = configuration;
        }


        public List<ENTCustomerList> GetAllCustomer(ENTCustomerRequest request)
        {
            List<ENTCustomerList> result = new List<ENTCustomerList>();
            string connString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                #region query 
                string strQuery = $@"EXEC [sp_DSD_GetCustomerList] @UserId={request.userId}, @TerritoryId={request.territoryId}
                    , @Address='{request.address}', @CustomerNameOrNo='{request.customerName}', @CustomerMatch={(request.customerMatch ? 1 : 0)} 
                    , @AccountType={request.accountType}, @IncludeDeleted={request.includeDeleted}, @OrderBY='{request.orderBy}', @SortOrder='{request.sortOrder}'
                    , @Page={request.page}, @PageSize={request.pageSize}, @City='{request.city}', @State='{request.state}', @ZipCode='{request.zipCode}'
                    , @ParentNumber='{request.parentNumber}', @TeamId={request.teamId}";


                result = SqlDBHelper.RawSqlQuery(strQuery, x => new ENTCustomerList
                {
                    AccountType = x["AccountType"].ToString() == "1" ? "Direct" : "Indirect",
                    CustomerId = x["CustomerId"].ToString(),
                    CustomerName = x["CustomerName"].ToString(),
                    CustomerNumber = x["CustomerNumber"].ToString(),
                    IsDeleted = x["IsDeleted"].ToString() == "True" ? "Yes" : "No",
                    Team = x["Team"].ToString(),
                    TerritoryId = x["TerritoryId"].ToString(),
                    Territory = x["TerritoryName"].ToString(),
                    KeyAccount = x["KeyAccount"].ToString(),
                    InsideSales = x["InsideSales"].ToString(),
                    Broker = x["Broker"].ToString(),
                    PhysicalAddress = x["PhysicalAddress"].ToString(),
                    PhysicalAddressState = x["PhysicalAddressState"].ToString(),
                    PhysicalAddressCity = x["PhysicalAddressCity"].ToString(),
                    PhysicalAddressZipCode = x["PhysicalAddressZipCode"].ToString(),
                    ShippingAddress = x["ShippingAddress"].ToString(),
                    ShippingAddressState = x["ShippingAddressState"].ToString(),
                    ShippingAddressCity = x["ShippingAddressCity"].ToString(),
                    ShippingAddressZipCode = x["ShippingAddressZipCode"].ToString(),
                }, connString).ToList();
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "CustomerService.GetAllCustomer", ex);
            }
            return result;
        }

        public ActionStatus DeleteCustomer(ENTPatchCustomerRequest activeStatus)
        {
            ActionStatus result = new ActionStatus();
            string connString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                string customerIds = String.Join(',', activeStatus.Id.Where(x => x > 0).ToList());
                if (!string.IsNullOrWhiteSpace(customerIds))
                {
                    List<SqlParameter> sqlParameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@CustomerIds", customerIds),
                        new SqlParameter("@Status", activeStatus.status),
                        new SqlParameter("@UpdatedBy", activeStatus.UpdatedBy),
                        new SqlParameter("@UpdatedDate", GetDateTime.getDate())
                    };

                    int count = SqlDBHelper.ExecuteNonQuery("sp_DSD_DeleteCustomers", ref sqlParameters, connString);

                    if (count > 0)
                    {
                        return new ActionStatus
                        {
                            Success = true,
                            Message = ""
                        };
                    }
                }
                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "CustomerService.DeleteCustomer" + ex);
            }
            return result;
        }

        public ActionStatus ActivateCustomer(ENTPatchCustomerRequest activeStatus)
        {
            ActionStatus result = new ActionStatus();
            string connString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                string customerIds = String.Join(',', activeStatus.Id.Where(x => x > 0).ToList());
                if (!string.IsNullOrWhiteSpace(customerIds))
                {
                    List<SqlParameter> sqlParameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@CustomerIds", customerIds),
                        new SqlParameter("@UpdatedBy", activeStatus.UpdatedBy),
                        new SqlParameter("@UpdatedDate", GetDateTime.getDate())
                    };

                    int count = SqlDBHelper.ExecuteNonQuery("sp_DSD_ActivateCustomers", ref sqlParameters, connString);

                    if (count > 0)
                    {
                        return new ActionStatus
                        {
                            Success = true,
                            Message = ""
                        };
                    }
                }
                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "CustomerService.ActivateCustomer" + ex);
            }
            return result;
        }
        public ActionStatus ChangeCustomerDetails(ENTUpdateCustomerRequest request)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                string connString = _configuration.GetConnectionString("DefaultConnection");
                string customerIds = String.Join(',', request.customerIds.Where(x => x > 0).ToList());
                if (!string.IsNullOrWhiteSpace(customerIds))
                {
                    List<SqlParameter> sqlParameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@CustomerIds", customerIds),
                        new SqlParameter("@UpdateTerritoryId", request.updateTerritoryId),
                        new SqlParameter("@AddTeamId", request.addTeamId),
                        new SqlParameter("@DeleteTeamId", request.deleteTeamId),
                        new SqlParameter("@UpdatedBy", request.UpdatedBy),
                    };

                    int count = SqlDBHelper.ExecuteNonQuery("sp_DSD_ChangeCustomerDetails", ref sqlParameters, connString);
                    if (count > 0)
                    {
                        return new ActionStatus
                        {
                            Success = true,
                            Message = ""
                        };
                    }
                }
                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "CustomerService.DeleteCustomer" + ex);
            }
            return result;
        }

        public List<ENTActionHistoryResponse> GetActionHistory()
        {
            List<ENTActionHistoryResponse> result = new List<ENTActionHistoryResponse>();
            string connString = _configuration.GetConnectionString("DefaultConnection");
            try
            {

                #region query
                string strQuery = string.Format("EXEC [sp_DSD_GetActionHistoryList]");

                result = SqlDBHelper.RawSqlQuery(strQuery, x => new ENTActionHistoryResponse
                {
                    Module = Convert.ToString(x["Module"]),
                    Operation = Convert.ToString(x["Operation"]),
                    AccountName = Convert.ToString(x["AccountName"]),
                    CustomerNumber = Convert.ToString(x["CustomerNumber"]),
                    Team = Convert.ToString(x["Team"]),
                    IsUpdateTeam = Convert.ToInt32(x["IsUpdateTeam"]) == 1 ? true : false,
                    UpdatedBy = Convert.ToString(x["UpdatedBy"]),
                    UpdatedDate = Convert.ToDateTime(x["UpdatedDate"]),
                }, connString).ToList();
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "CustomerService.GetActionHistory", ex);
            }
            return result;
        }

        //public async Task<KendoGridDataResult<ENTActionHistoryResponse>> GetActionHistoriesWithPaginationAsync(int page, int pageSize)
        //{
        //    KendoGridDataResult<ENTActionHistoryResponse> result = new KendoGridDataResult<ENTActionHistoryResponse>();
        //    string connString = _unitofwork.DbContext.Database.GetDbConnection().ConnectionString;
        //    try
        //    {
        //        #region query
        //        result = await SqlDBHelper.SqlQueryWithPaginationAsync(page, pageSize
        //            , x => new ENTActionHistoryResponse
        //        {
        //            Module = Convert.ToString(x["Module"]),
        //            Operation = Convert.ToString(x["Operation"]),
        //            AccountName = Convert.ToString(x["AccountName"]),
        //            CustomerNumber = Convert.ToString(x["CustomerNumber"]),
        //            Team = Convert.ToString(x["Team"]),
        //            IsUpdateTeam = Convert.ToInt32(x["IsUpdateTeam"]) == 1 ? true : false,
        //            UpdatedBy = Convert.ToString(x["UpdatedBy"]),
        //            UpdatedDate = Convert.ToDateTime(x["UpdatedDate"]),
        //        }, "sp_DSD_GetActionHistoriesWithPagination", connString);
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(Constants.ACTION_EXCEPTION, "CustomerService.GetActionHistoriesWithPaginationAsync", ex);
        //    }
        //    return result;
        //}
        public async Task<KendoGridDataResult<ENTActionHistoryResponse>> GetActionHistoriesWithPaginationAsync(KendoGridRequest request)
        {
            KendoGridDataResult<ENTActionHistoryResponse> result = new KendoGridDataResult<ENTActionHistoryResponse>();
            string connString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                string customFilter = null;
                Func<string, string> funcParam = null;
                #region query
                if (request.Filters.ContainsKey("updatedDate"))
                {
                    customFilter = $" CAST(UpdatedDate AS DATE) = '{request.Filters["updatedDate"]}' ";
                    request.Filters.Remove("updatedDate");

                    funcParam = (paramFilter) =>
                    {
                        if (!string.IsNullOrWhiteSpace(paramFilter))
                        {
                            paramFilter += $" AND  {customFilter}";
                        }
                        else paramFilter = customFilter;
                        return paramFilter;
                    };
                }



                result = await SqlDBHelper.SqlQueryWithPaginationAsync(
                    connString, "sp_DSD_GetActionHistoriesWithPagination", request
                    , x => new ENTActionHistoryResponse
                    {
                        Module = Convert.ToString(x["Module"]),
                        Operation = Convert.ToString(x["Operation"]),
                        AccountName = Convert.ToString(x["AccountName"]),
                        CustomerNumber = Convert.ToString(x["CustomerNumber"]),
                        Team = Convert.ToString(x["Team"]),
                        IsUpdateTeam = Convert.ToInt32(x["IsUpdateTeam"]) == 1 ? true : false,
                        UpdatedBy = Convert.ToString(x["UpdatedBy"]),
                        UpdatedDate = Convert.ToDateTime(x["UpdatedDate"]),
                    }, funcParam);
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "CustomerService.GetActionHistoriesWithPaginationAsync", ex);
            }
            return result;
        }
        public ActionStatus AddCustomerMaster(CustomerMasterRequest customerMasterRequest)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                string connString = _configuration.GetConnectionString("DefaultConnection");
                if (customerMasterRequest.CustomerName.Any())
                {
                    List<SqlParameter> sqlParameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@CustomerName", customerMasterRequest.CustomerName.Trim()),
                        new SqlParameter("@EmailId", customerMasterRequest.EmailId.Trim()),
                        new SqlParameter("@Address", customerMasterRequest.Address.Trim()),
                        new SqlParameter("@AddressCity", customerMasterRequest.AddressCity.Trim()),
                        new SqlParameter("@AddressState", customerMasterRequest.AddressState.Trim()),
                        new SqlParameter("@AddressZipCode", customerMasterRequest.AddressZipCode.Trim()),
                        new SqlParameter("@TerritoryCode", customerMasterRequest.TerritoryCode.Trim()),
                        new SqlParameter("@IsParent", customerMasterRequest.IsParent),
                        new SqlParameter("@Parent", customerMasterRequest.Parent.Trim()),
                        new SqlParameter("@AccountType", customerMasterRequest.AccountType.Trim()),
                        new SqlParameter("@AccountClassification", customerMasterRequest.AccountClassification.Trim()),
                        new SqlParameter("@Latitude", customerMasterRequest.Latitude),
                        new SqlParameter("@Longitude", customerMasterRequest.Longitude)
                    };

                    int count = SqlDBHelper.ExecuteNonQuery("sp_DSD_AddCustomerMasterDetails", ref sqlParameters, connString);
                    if (count > 0)
                    {
                        return new ActionStatus
                        {
                            Success = true,
                            Message = ""
                        };
                    }
                }
                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("zoneid") && ex.Message.ToLower().Contains("cannot insert the value null")
             || (ex.Message.ToLower().Contains("regionid") && ex.Message.ToLower().Contains("cannot insert the value null"))
            || (ex.Message.ToLower().Contains("territoryid") && ex.Message.ToLower().Contains("cannot insert the value null")))
                {
                    result = new ActionStatus
                    {
                        Success = false,
                        Message = "Territory must be valid and correctly mapped to a Region and Zone in the system."
                    };
                }
                else if (ex.Message.ToLower().Contains("physicaladdressstateid") && ex.Message.ToLower().Contains("cannot insert the value null"))
                {
                    result = new ActionStatus
                    {
                        Success = false,
                        Message = "State must be valid state code."
                    };
                }
                else
                {
                    result = new ActionStatus
                    {
                        Success = false,
                        Message = "An error occurred while adding customer master details."
                    };
                }
                logger.Error(Constants.ACTION_EXCEPTION, "CustomerService.AddCustomerMaster" + ex);
            }
            return result;
        }

        public List<ENTCustomerMaster> GetCustomer(string customerName, string Address, string AddressCity, string AddressState, string AddressZipCode)
        {
            List<ENTCustomerMaster> result = new List<ENTCustomerMaster>();
            string connString = _configuration.GetConnectionString("DefaultConnection");
            try
            {

                #region query
                string strQuery = string.Format("EXEC [sp_DSD_ValidateCustomerMasterDetails] @CustomerName='{0}', @Address='{1}', @AddressCity='{2}'" +
                    ", @AddressState='{3}', @AddressZipCode='{4}'", customerName, Address, AddressCity, AddressState, AddressZipCode);

                result = SqlDBHelper.RawSqlQuery(strQuery, x => new ENTCustomerMaster
                {
                    CustomerName = x["CustomerName"].ToString(),
                    Latitude = (double)x["Latitude"],
                    Longitude = (double)x["Longitude"],
                }, connString).ToList();
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "CustomerService.GetCustomer", ex);
                throw new Exception(ex.Message);
            }
            return result;
        }

        public async Task<List<ENTCustomerMaster>> GetCustomerAsync(
            string customerName,
            string address,
            string addressCity,
            string addressState,
            string addressZipCode)
        {
            List<ENTCustomerMaster> result = new List<ENTCustomerMaster>();
            string connString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                SqlParameter[] sqlParameters = new[]
                {
                    new SqlParameter("@CustomerName", customerName ?? (object)DBNull.Value),
                    new SqlParameter("@Address", address ?? (object)DBNull.Value),
                    new SqlParameter("@AddressCity", addressCity ?? (object)DBNull.Value),
                    new SqlParameter("@AddressState", addressState ?? (object)DBNull.Value),
                    new SqlParameter("@AddressZipCode", addressZipCode ?? (object)DBNull.Value)
                };

                result = (await SqlDBHelper.ExecuteReaderAsync(
                    "sp_DSD_ValidateCustomerMasterDetails",
                    sqlParameters,
                    connString,
                    x => new ENTCustomerMaster
                    {
                        CustomerName = x["CustomerName"].ToString(),
                        Latitude = x["Latitude"] != DBNull.Value ? Convert.ToDouble(x["Latitude"]) : 0.0,
                        Longitude = x["Longitude"] != DBNull.Value ? Convert.ToDouble(x["Longitude"]) : 0.0,
                    })).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "CustomerService.GetCustomer", ex);
                throw new Exception(ex.Message);
            }

            return result;
        }



    }
}
