using System;

namespace DRL.Entity.Response
{
    public class ENTRegionResponse
    {
        public int RegionId { get; set; }
        public string Regioname { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
