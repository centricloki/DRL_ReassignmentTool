using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class ProductMaster
    {
        public ProductMaster()
        {
            ProductAdditionalDocument = new HashSet<ProductAdditionalDocument>();
        }

        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsPromotional { get; set; }
        public int ImportedFrom { get; set; }
        public DateTime DistributionRecordedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int Status { get; set; }
        public int LangId { get; set; }
        public string Uom { get; set; }
        public long CatId { get; set; }
        public long BrandId { get; set; }
        public long StyleId { get; set; }
        public bool? IsTobbacoProduct { get; set; }
        public bool IsDeleted { get; set; }
        public int? ItemType { get; set; }

        public virtual RecordResourceType ImportedFromNavigation { get; set; }
        public virtual ICollection<ProductAdditionalDocument> ProductAdditionalDocument { get; set; }
    }
}
