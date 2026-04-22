using System.Collections.Generic;

namespace DRL.Entity
{
    /// <summary>
    /// Hierarchical user report node for V3 tree-table UI (single API payload).
    /// </summary>
    public class ENTUserReportHierarchyNode
    {
        public string NodeId { get; set; }
        public string ParentNodeId { get; set; }

        public int UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string ZoneName { get; set; }
        public string ZoneId { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string EntityId { get; set; }
        public string RegionName { get; set; }
        public string RegionId { get; set; }
        public string BDId { get; set; }
        public string BDName { get; set; }
        public string AvpName { get; set; }
        public string AvpId { get; set; }

        /// <summary>Comma-separated source from SQL (optional diagnostics).</summary>
        public string TerritoryId { get; set; }
        public string TerritoryName { get; set; }

        public string[] TerritoryIds { get; set; }
        public string[] TerritoryNames { get; set; }

        /// <summary>Normalized text for client-side territory search.</summary>
        public string TerritoryFilterText { get; set; }

        public string AvpFilter { get; set; }
        public string ZoneFilter { get; set; }
        public string RegionFilter { get; set; }
        public string BdFilter { get; set; }

        public string AvpDetail { get; set; }
        public string ZoneDetail { get; set; }
        public string RegionDetail { get; set; }
        public string BdDetail { get; set; }
        public string[] TerritoryDetailIds { get; set; }

        // Display properties for UI binding
        public string DisplayFullName => FullName ?? string.Empty;
        public string DisplayRoleName => RoleName ?? string.Empty;
        public string DisplayZoneName => ZoneName ?? string.Empty;
        public string DisplayRegionName => RegionName ?? string.Empty;
        public string DisplayBDName => BDName ?? string.Empty;
        public string DisplayAvpName => AvpName ?? string.Empty;
        public string DisplayTerritoryName => TerritoryName ?? string.Empty;
        public string DisplayManagerFullName => ManagerFullName ?? string.Empty;
        public string DisplayManagerZoneName => ManagerZoneName ?? string.Empty;
        public string DisplayManagerRegionName => ManagerRegionName ?? string.Empty;
        public string DisplayManagerTerritoryName => ManagerTerritoryName ?? string.Empty;

        // Territories property for backward compatibility
        public string[] Territories => TerritoryNames ?? new string[0];
        public string DisplayTerritories => TerritoryFilterText ?? string.Empty;

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
        
        // Manager ID for hierarchy building
        public int ManagerID { get; set; }

        public List<ENTUserReportHierarchyNode> Children { get; set; } = new List<ENTUserReportHierarchyNode>();
    }
}