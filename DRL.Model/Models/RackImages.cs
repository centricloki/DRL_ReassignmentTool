using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class RackImages
    {
        public long Id { get; set; }
        public long RackCategoryMasterId { get; set; }
        public long RackCategoryDetailId { get; set; }
        public string Image { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
