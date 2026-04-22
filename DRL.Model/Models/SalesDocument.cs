using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class SalesDocument
    {
        public long SalesDocumentId { get; set; }
        public string OriginalFileName { get; set; }
        public string SalesDocumentPath { get; set; }
        public int ImportedFrom { get; set; }
        public DateTime UpdateDate { get; set; }
        public long? CategoryId { get; set; }
        public long? BrandId { get; set; }
        public long? StyleId { get; set; }
        public long? ProductId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
    }
}
