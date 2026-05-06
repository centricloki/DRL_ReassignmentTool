using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity
{
    public class ENTZone
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public DateTime UpdateDate { get; set; } = System.DateTime.UtcNow;
        public string SugarZoneId { get; set; }
        public int ImportedFrom { get; set; } = 1;
        public int? AVPID { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; } = 1;
        public long? UpdatedBy { get; set; } = 1;
        public DateTime CreatedDate { get; set; } = System.DateTime.UtcNow;
    }
}
