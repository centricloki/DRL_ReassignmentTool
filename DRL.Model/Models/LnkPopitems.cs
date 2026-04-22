using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class LnkPopitems
    {
        public long Id { get; set; }
        public long BrandIstyleId { get; set; }
        public long ProductId { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public long? Hierarchy1 { get; set; }
        public long? Hierarchy2 { get; set; }
        public long? Hierarchy3 { get; set; }
        public long? Hierarchy4 { get; set; }
        public long? Hierarchy5 { get; set; }
    }
}
