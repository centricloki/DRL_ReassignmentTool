using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class RouteStations
    {
        public long RouteStationId { get; set; }
        public long RouteId { get; set; }
        public long UserId { get; set; }
        public long CustomerId { get; set; }
    }
}
