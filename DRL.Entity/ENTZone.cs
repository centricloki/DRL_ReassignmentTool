using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity
{
    public class ENTZone
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public DateTime UpdateDate { get; set; }
        public string SugarZoneId { get; set; }
        public int ImportedFrom { get; set; }
        public int? AVPID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
