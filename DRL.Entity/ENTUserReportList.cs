namespace DRL.Entity
{
    public class ENTUserReportList
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string ZoneName { get; set; }
        public string ZoneId { get; set; }
        public string RoleName { get; set; }
        public string RegionName { get; set; }
        public string RegionId { get; set; }
        public string TerritoryName { get; set; }
        public string TerritoryId { get; set; }
        public string RoleId { get; set; }
        public string EntityId { get; set; }
        public string BDId { get; set; }
        public string BDName { get; set; }
        public string AvpName { get; set; }
        public string AvpId { get; set; }
        
        // Manager-related properties
        public string ManagerUserName { get; set; }
        public string ManagerRoleName { get; set; }
        public string ManagerRoleId { get; set; }
        public string ManagerAvpName { get; set; }
        public string ManagerAvpId { get; set; }
        public string ManagerFullName { get; set; }
        public string ManagerZoneName { get; set; }
        public string ManagerZoneId { get; set; }
        public string ManagerRegionName { get; set; }
        public string ManagerRegionId { get; set; }
        public string ManagerBdName { get; set; }
        public string ManagerBdId { get; set; }
        public string ManagerTerritoryName { get; set; }
        public string ManagerTerritoryId { get; set; }

        // Critical for hierarchy building
        public int ManagerID { get; set; }
    }
}