using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity
{
    public class ENTTeam
    {
        public int? TeamId { get; set; }
        public string Name { get; set; }
        public int? RegionId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedBy { get; set; }
        public int? BDID { get; set; }
    }
}
