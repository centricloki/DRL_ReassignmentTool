using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class OrderHistoryEmail
    {
        public long Id { get; set; }
        public string DeviceOrderId { get; set; }
        public string EmailId { get; set; }
        public string MemoField { get; set; }
        public bool? IsEmailSent { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string DeviceOrderEmailId { get; set; }
        public bool? IsExported { get; set; }
        public long? UserId { get; set; }
    }
}
