using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class RackCategoryDetail
    {
        public long Id { get; set; }
        public long RackCategoryId { get; set; }
        public int? Quantity { get; set; }
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
