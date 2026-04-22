using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class RecordResourceType
    {
        public RecordResourceType()
        {
            ProductMaster = new HashSet<ProductMaster>();
        }

        public int ResourceTypeId { get; set; }
        public string ResourceName { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<ProductMaster> ProductMaster { get; set; }
    }
}
