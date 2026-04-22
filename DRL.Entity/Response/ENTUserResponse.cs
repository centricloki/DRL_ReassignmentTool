using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity.Response
{
    public class ENTUserResponse
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PIN { get; set; }
        public string RoleName { get; set; }
        public string ZoneName { get; set; }
        public string RegionName { get; set; }
        public string TerritoryName { get; set; }
        public string ManagerName { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AVPName { get; set; }
        public string BDName { get; set; }
    }
}
