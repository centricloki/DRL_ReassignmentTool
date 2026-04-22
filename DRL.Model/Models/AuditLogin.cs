using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class AuditLogin
    {
        public long AuditLoginId { get; set; }
        public long UserId { get; set; }
        public DateTime AuditDateTime { get; set; }
        public long? UserMastersCount { get; set; }
        public long? CategoryMastersCount { get; set; }
        public long? BrandMastersCount { get; set; }
        public long? StyleMastersCount { get; set; }
        public long? ProductMastersCount { get; set; }
        public long? ProductAdditionalDocumentsCount { get; set; }
        public long? CategoryProductsCount { get; set; }
        public int? RoleMastersCount { get; set; }
        public int? RecordResourceTypesCount { get; set; }
        public int? StateMastersCount { get; set; }
        public int? CityMastersCount { get; set; }
        public int? ZoneMastersCount { get; set; }
        public int? RegionMastersCount { get; set; }
        public int? TerritoryMastersCount { get; set; }
        public long? CustomerMasterCount { get; set; }
        public long? CustomerDocumentsCount { get; set; }
        public long? OrderMastersCount { get; set; }
        public long? OrderDetailsCount { get; set; }
        public long? CallActivitysCount { get; set; }
        public long? ScheduleRoutesCount { get; set; }
        public long? RouteStationsCount { get; set; }
        public long? CustomerProductsCount { get; set; }
        public long? SupplyChainsCount { get; set; }
        public long? AccountClassificationTypeMastersCount { get; set; }
        public long? DistributerCount { get; set; }
        public long? ContactCount { get; set; }
        public long? CustomerDistributorCount { get; set; }
        public long? UserTaxStatementCount { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string DbfileName { get; set; }
        public DateTime? LastSyncUtcdate { get; set; }
        public string LoginUserName { get; set; }
        public string LoginPassword { get; set; }
        public string ErrorMsg { get; set; }
        public string VersionNumber { get; set; }
    }
}
