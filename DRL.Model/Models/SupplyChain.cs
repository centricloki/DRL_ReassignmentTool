using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class SupplyChain
    {
        public long SupplyChainId { get; set; }
        public long CustomerId { get; set; }
        public long CustomerParentId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsExport { get; set; }
    }
}
