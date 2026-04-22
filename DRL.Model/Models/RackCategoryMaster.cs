using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class RackCategoryMaster
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
