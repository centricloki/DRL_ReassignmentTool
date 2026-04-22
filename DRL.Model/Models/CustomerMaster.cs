using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class CustomerMaster
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public int AccountType { get; set; }
        public string DeviceCustomerId { get; set; }
        public string DistributorId { get; set; }
        public string DeviceDistributorCustomerId { get; set; }
        public string AccountResponsibility { get; set; }
        public int? AccountClassification { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string EmailId { get; set; }
        public int? Team { get; set; }
        public int ZoneId { get; set; }
        public int RegionId { get; set; }
        public int TerritoryId { get; set; }
        public string Broker { get; set; }
        public string AssociatedInternalSalesGuy { get; set; }
        public string AssignUserId { get; set; }
        public string Buyer { get; set; }
        public int? StoreCount { get; set; }
        public long? SupplyChainId { get; set; }
        public string ManagerName { get; set; }
        public int ImportedFrom { get; set; }
        public bool IsExported { get; set; }
        public string PhysicalAddress { get; set; }
        public string PhysicalAddressCity { get; set; }
        public int PhysicalAddressStateId { get; set; }
        public string PhysicalAddressZipCode { get; set; }
        public string MailingAddress { get; set; }
        public string MailingAddressCity { get; set; }
        public int MailingAddressStateId { get; set; }
        public string MailingAddressZipCode { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingAddressCity { get; set; }
        public int ShippingAddressStateId { get; set; }
        public string ShippingAddressZipCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public string SugarCustomerId { get; set; }
        public bool IsCreatePermanent { get; set; }
        public string Ranking { get; set; }
        public bool IsDeleted { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public decimal? YtdcurrentYear { get; set; }
        public decimal? YtdlastYear { get; set; }
        public int? YtdcasesCurrentYear { get; set; }
        public int? YtdcasesLastYear { get; set; }
        public decimal? VarianceYtd { get; set; }
        public decimal? VarianceMtd { get; set; }
        public string StateTobaccoLicense { get; set; }
        public string RetailerLicense { get; set; }
        public string RetailerSalesTaxCertificate { get; set; }
        public string TaxStatement { get; set; }
        public bool IsUpdated { get; set; }
        public bool? IsLocatgionUpdated { get; set; }
        public string StoreType { get; set; }
        public string BuyType { get; set; }
        public string TradeType { get; set; }
    }
}
