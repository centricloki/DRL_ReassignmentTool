using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class ScheduleRoutes
    {
        public long RouteId { get; set; }
        public string RouteName { get; set; }
        public long UserId { get; set; }
        public long? AssignTsmid { get; set; }
        public string RouteDescription { get; set; }
        public DateTime PlannedDate { get; set; }
        public string HouseNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string AddressName { get; set; }
        public string RouteType { get; set; }
        public DateTime UpdateDate { get; set; }
        public string SugarIds { get; set; }
        public string DeviceRouteId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsExported { get; set; }
    }
}
