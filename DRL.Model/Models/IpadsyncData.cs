using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class IpadsyncData
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public bool? IsPartialSync { get; set; }
        public DateTime? PartialSyncFromDateTimeSendByIpad { get; set; }
        public DateTime? SyncCurrentDateTime { get; set; }
        public long? LoginCategory { get; set; }
        public long? LoginBrand { get; set; }
        public long? LoginStyle { get; set; }
        public long? LoginProduct { get; set; }
        public long? LoginProductDocument { get; set; }
        public int? LoginRole { get; set; }
        public int? LoginRecordResourceType { get; set; }
        public int? LoginState { get; set; }
        public int? LoginCity { get; set; }
        public int? LoginZone { get; set; }
        public int? LoginRegion { get; set; }
        public int? LoginTerritory { get; set; }
        public long? LoginCustomer { get; set; }
        public long? LoginCustomerDocument { get; set; }
        public long? LoginOrder { get; set; }
        public long? LoginOrderDetails { get; set; }
        public long? LoginCallActivity { get; set; }
        public long? LoginScheduleRoutes { get; set; }
        public long? LoginRouteStations { get; set; }
        public long? LoginCustomerProduct { get; set; }
        public long? LoginSupplyChain { get; set; }
        public long? LoginAccountClassification { get; set; }
        public long? LoginDistributerCount { get; set; }
        public long? LoginContactCount { get; set; }
        public long? LoginCustomerDistributorCount { get; set; }
        public long? LoginUserTaxStatement { get; set; }
        public string DbfileName { get; set; }
        public DateTime? LastSyncUtcdate { get; set; }
        public int? AddEditCustomer { get; set; }
        public int? AddEditContact { get; set; }
        public int? AddOrder { get; set; }
        public int? AddEditCallActivity { get; set; }
        public int? AddEditScheduleRoute { get; set; }
        public int? AddCustomerDocument { get; set; }
        public int? DeleteCustomerDocument { get; set; }
        public int? DeleteCallActivity { get; set; }
        public int? DeleteScheduleRoutes { get; set; }
        public bool? IsForgotPassword { get; set; }
        public string LoginUserName { get; set; }
        public string LoginPassword { get; set; }
        public string ErrorMsg { get; set; }
        public string SuccessMsg { get; set; }
        public string VersionNumber { get; set; }
    }
}
