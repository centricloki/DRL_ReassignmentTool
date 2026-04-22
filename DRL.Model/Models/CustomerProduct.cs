using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class CustomerProduct
    {
        public long CustomerProductId { get; set; }
        public long CustomerId { get; set; }
        public long ProductId { get; set; }
        public DateTime LastDistributionRecordDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
