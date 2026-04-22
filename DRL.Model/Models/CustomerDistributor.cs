using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class CustomerDistributor
    {
        public long CustomerDistributorId { get; set; }
        public long DistributorId { get; set; }
        public bool IsExported { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long UpdatedBy { get; set; }
        public string DeviceCustomerId { get; set; }
    }
}
