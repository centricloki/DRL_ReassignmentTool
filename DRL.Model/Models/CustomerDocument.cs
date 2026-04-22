using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class CustomerDocument
    {
        public long CustomerDocumentId { get; set; }
        public long CustomerId { get; set; }
        public string OriginalFileName { get; set; }
        public string CustomerDocumentPath { get; set; }
        public int ImportedFrom { get; set; }
        public bool IsExported { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string SugarCustomerDocId { get; set; }
        public string DocumentType { get; set; }
        public bool IsDelete { get; set; }
        public long? UserId { get; set; }
        public string DeviceDocumentId { get; set; }
        public string Description { get; set; }
    }
}
