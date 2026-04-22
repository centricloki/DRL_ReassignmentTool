using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class RegionMaster
    {
        public int RegionId { get; set; }
        public string Regioname { get; set; }
        public int ZoneId { get; set; }
        public DateTime UpdateDate { get; set; }
        public string SugarRegionId { get; set; }
        public int ImportedFrom { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
