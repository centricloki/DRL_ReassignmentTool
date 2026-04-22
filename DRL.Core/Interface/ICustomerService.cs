using DRL.Entity;
using DRL.Entity.Response;
using DRL.Library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DRL.Core.Interface
{
    public interface ICustomerService
    {
        List<ENTCustomerList> GetAllCustomer(ENTCustomerRequest request);
        ActionStatus DeleteCustomer(ENTPatchCustomerRequest activeStatus);
        ActionStatus ActivateCustomer(ENTPatchCustomerRequest activeStatus);
        ActionStatus ChangeCustomerDetails(ENTUpdateCustomerRequest request);
        List<ENTActionHistoryResponse> GetActionHistory();
        ActionStatus AddCustomerMaster(CustomerMasterRequest customerMasterRequest);
        List<ENTCustomerMaster> GetCustomer(string customerName, string Address, string AddressCity, string AddressState, string AddressZipCode);
        Task<List<ENTCustomerMaster>> GetCustomerAsync(
           string customerName,
           string address,
           string addressCity,
           string addressState,
           string addressZipCode);
        Task<KendoGridDataResult<ENTActionHistoryResponse>> GetActionHistoriesWithPaginationAsync(KendoGridRequest request);
    }
}
