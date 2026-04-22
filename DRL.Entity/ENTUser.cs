using System;
using System.Collections.Generic;

namespace DRL.Entity
{
    public class ENTUser
    {
        public long? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Pin { get; set; }
        public int RoleId { get; set; }
        public string TerritoryId { get; set; }
        public int? DefaultTeamId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public long ManagerId { get; set; }
        public int BDID { get; set; }
        public int AVPID { get; set; }
        //public string RoleName { get; set; }
        //public string ManagerName { get; set; }
        //public string ZoneName { get; set; }
        //public string RegionName { get; set; }
        //public string DefTerritoryName { get; set; }

        public List<ENTTeam> Teams { get; set; }
        public List<ENTZone> Zones { get; set; }
    }

    
}
