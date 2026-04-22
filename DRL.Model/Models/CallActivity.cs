using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class CallActivity
    {
        public long CallActivityId { get; set; }
        public string DeviceCallActivityId { get; set; }
        public string DeviceCustomerId { get; set; }
        public long CustomerId { get; set; }
        public long UserId { get; set; }
        public DateTime CallDate { get; set; }
        public string ActivityType { get; set; }
        public bool? IsThisFromYourList { get; set; }
        public string GratisProductUsed { get; set; }
        public string CarSalesSold { get; set; }
        public decimal? Amount { get; set; }
        public string Objective { get; set; }
        public string Result { get; set; }
        public string Comments { get; set; }
        public int ImportedFrom { get; set; }
        public bool IsExported { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string SugarCallActivityId { get; set; }
        public long? OrderId { get; set; }
        public bool IsDeleted { get; set; }
        public int Hours { get; set; }
        public string WholesaleInvoiceFilePath { get; set; }
        public bool IsVoided { get; set; }
        public bool IsSecure { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public string Status { get; set; }
    }
}
