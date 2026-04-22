using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class TerritoryMaster
    {
        public int TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public int RegionId { get; set; }
        public string SugarTerritoryId { get; set; }
        public int? ImportedFrom { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? BDID { get; set; }

    }
}
