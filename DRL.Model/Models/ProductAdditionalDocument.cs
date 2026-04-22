using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class ProductAdditionalDocument
    {
        public long ProductAdditionalDocumentId { get; set; }
        public long ProductId { get; set; }
        public string DocumentFilePath { get; set; }
        public int ImportedFrom { get; set; }
        public int? DocumentType { get; set; }
        public int LangId { get; set; }
        public int Priority { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int Status { get; set; }
        public bool IsNew { get; set; }

        public virtual ProductMaster Product { get; set; }
    }
}
