using System;

namespace DRL.Entity.Response
{
    public class ENTZoneResponse
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? AVPID { get; set; }
        public string AVPName { get; set; }
    }
}