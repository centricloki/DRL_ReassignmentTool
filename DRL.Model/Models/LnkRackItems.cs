using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class LnkRackItems
    {
        public long Id { get; set; }
        public long BrandIstyleId { get; set; }
        public long ProductId { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public int? IsMainRack { get; set; }
        public bool? Status { get; set; }
        public int? SortOrder { get; set; }
    }
}
