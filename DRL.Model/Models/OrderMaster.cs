using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class OrderMaster
    {
        public long OrderId { get; set; }
        public string DeviceOrderId { get; set; }
        public long CustomerId { get; set; }
        public string DeviceCustomerId { get; set; }
        public long? CustomerDistributorId { get; set; }
        public string CustomerDeviceDistributorId { get; set; }
        public int SalesType { get; set; }
        public int ImportedFrom { get; set; }
        public bool IsExported { get; set; }
        public string CustomerName { get; set; }
        public string SellerName { get; set; }
        public string SellerRepTobaccoPermitNo { get; set; }
        public string OrderAddress { get; set; }
        public string OrderCityId { get; set; }
        public string OrderZipCode { get; set; }
        public string RetailerLicense { get; set; }
        public string RetailerSalesTaxCertificate { get; set; }
        public string RepublicSalesRepository { get; set; }
        public string CustomStatement { get; set; }
        public string CustomerShippingCityId { get; set; }
        public string CustomerShippingZipCode { get; set; }
        public DateTime? PrebookShipDate { get; set; }
        public string CustomerSignatureFilePath { get; set; }
        public string CustomerComment { get; set; }
        public string EmailRecipients { get; set; }
        public bool IsOrderEmailSent { get; set; }
        public bool IsOrderConfirmed { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public int ZoneId { get; set; }
        public int RegionId { get; set; }
        public int TerritoryId { get; set; }
        public string SugarCrmorderId { get; set; }
        public bool? OpenOrderstatus { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderNumber { get; set; }
        public int OrderStateId { get; set; }
        public int CustomerShippingStateId { get; set; }
        public string StateTobaccoLicense { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime OrderDate { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PrintName { get; set; }
        public bool IsVoided { get; set; }
    }
}
