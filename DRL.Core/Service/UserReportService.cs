using DRL.Core.Interface;
using DRL.Entity;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using DRL.Model.DataBase;
using DRL.Model.UnitOfWork.Interface;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRL.Core.Service
{
    public class UserReportService : IUserReportService
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper _commonHelper;

        public UserReportService(IUnitOfWork unitofwork, ILogManager logManager)
        {
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            _commonHelper = new CommonHelper();
        }

        public List<ENTUserReportList> GetUserReport(int roleId = 0, int entityId = 0)
        {
            List<ENTUserReportList> result = new List<ENTUserReportList>();
            string connString = _commonHelper.GetProdConnectionString();
            try
            {
                #region query
                string strQuery = string.Format("EXEC [sp_DSD_GetUsersReport] @RoleId = {0},@EntityId = {1}", roleId, entityId);

                result = SqlDBHelper.RawSqlQuery(strQuery, x => new ENTUserReportList
                {
                    UserId = x["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(x["UserID"]),
                    UserName = Convert.ToString(x["UserName"]),
                    FullName = Convert.ToString(x["FullName"]),
                    ZoneName = Convert.ToString(x["ZoneName"]),
                    ZoneId = Convert.ToString(x["ZoneId"]),
                    RoleName = Convert.ToString(x["RoleName"]),
                    RegionName = Convert.ToString(x["RegionName"]),
                    RegionId = Convert.ToString(x["RegionId"]),
                    TerritoryName = Convert.ToString(x["TerritoryName"]),
                    TerritoryId = Convert.ToString(x["TerritoryId"]),
                    RoleId = Convert.ToString(x["RoleId"]),
                    EntityId = Convert.ToString(x["EntityId"]),
                    BDId = Convert.ToString(x["BDId"]),
                    BDName = Convert.ToString(x["BDName"]),
                    AvpName = x["AVPName"] != DBNull.Value ? Convert.ToString(x["AVPName"]) : string.Empty,
                    AvpId = x["AVPId"] != DBNull.Value ? Convert.ToString(x["AVPId"]) : string.Empty,
                    
                    // Add missing manager field mappings
                    ManagerUserName = Convert.ToString(x["ManagerUserName"]),
                    ManagerRoleName = Convert.ToString(x["ManagerRoleName"]),
                    ManagerRoleId = Convert.ToString(x["ManagerRoleId"]),
                    ManagerAvpName = Convert.ToString(x["ManagerAvpName"]),
                    ManagerAvpId = Convert.ToString(x["ManagerAvpId"]),
                    ManagerFullName = Convert.ToString(x["ManagerFullName"]),
                    ManagerZoneName = Convert.ToString(x["ManagerZoneName"]),
                    ManagerZoneId = Convert.ToString(x["ManagerZoneId"]),
                    ManagerRegionName = Convert.ToString(x["ManagerRegionName"]),
                    ManagerRegionId = Convert.ToString(x["ManagerRegionId"]),
                    ManagerBdName = Convert.ToString(x["ManagerBdName"]),
                    ManagerBdId = Convert.ToString(x["ManagerBdId"]),
                    ManagerTerritoryName = Convert.ToString(x["ManagerTerritoryName"]),
                    ManagerTerritoryId = Convert.ToString(x["ManagerTerritoryId"]),
                    ManagerID = x["ManagerID"] == DBNull.Value ? 0 : Convert.ToInt32(x["ManagerID"])
                }, connString).ToList();

                #endregion

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public List<ENTUserReportHierarchyNode> GetUserReportHierarchy()
        {
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var roots = GetUserReport(17, 0) ?? new List<ENTUserReportList>();
            var nodes = new List<ENTUserReportHierarchyNode>();
            foreach (var row in roots)
            {
                var node = BuildNodeRecursive(row, null, null, null, null, null, visited);
                if (node != null)
                {
                    nodes.Add(node);
                }
            }
            return nodes;
        }

        private ENTUserReportHierarchyNode BuildNodeRecursive(
            ENTUserReportList row,
            string parentNodeId,
            string parentAvpName,
            string parentAvpId,
            string parentBdId,
            string parentBdName,
            HashSet<string> visited)
        {
            var key = VisitKey(row);
            if (visited.Contains(key))
            {
                return null;
            }

            visited.Add(key);

            // Use the AVP values directly from the current row, similar to Zone/Region
            // Only inherit from parent if current row doesn't have AVP values
            string currentAvpName = row.AvpName ?? parentAvpName;
            string currentAvpId = row.AvpId ?? parentAvpId;

            var node = MapToNode(row, parentNodeId, currentAvpName, currentAvpId);

            int r = ParseInt(row.RoleId);
            int e = ParseInt(row.EntityId);
            var childRows = GetUserReport(r, e) ?? new List<ENTUserReportList>();
            foreach (var childRow in childRows)
            {
                var child = BuildNodeRecursive(
                    childRow,
                    node.NodeId,
                    currentAvpName,
                    currentAvpId,
                    node.BDId,
                    node.BDName,
                    visited);
                if (child != null)
                {
                    node.Children.Add(child);
                }
            }

            return node;
        }

        private static string VisitKey(ENTUserReportList row)
        {
            return $"{row.RoleId}_{row.EntityId}_{row.UserId}";
        }

        private static ENTUserReportHierarchyNode MapToNode(ENTUserReportList row, string parentNodeId, string avpName, string avpId)
        {
            var node = new ENTUserReportHierarchyNode
            {
                NodeId = MakeNodeId(row),
                ParentNodeId = parentNodeId,
                UserId = row.UserId,
                FullName = row.FullName ?? string.Empty,
                UserName = row.UserName ?? string.Empty,
                ZoneName = row.ZoneName ?? string.Empty,
                ZoneId = row.ZoneId ?? string.Empty,
                RoleName = row.RoleName ?? string.Empty,
                RoleId = row.RoleId ?? string.Empty,
                EntityId = row.EntityId ?? string.Empty,
                RegionName = row.RegionName ?? string.Empty,
                RegionId = row.RegionId ?? string.Empty,
                BDId = row.BDId ?? string.Empty,
                BDName = row.BDName ?? string.Empty,
                AvpName = row.AvpName ?? string.Empty,
                AvpId = row.AvpId ?? string.Empty,
                TerritoryId = row.TerritoryId ?? string.Empty,
                TerritoryName = row.TerritoryName ?? string.Empty,
                
                // Add missing manager information mapping
                ManagerUserName = row.ManagerUserName ?? string.Empty,
                ManagerRoleName = row.ManagerRoleName ?? string.Empty,
                ManagerRoleId = row.ManagerRoleId ?? string.Empty,
                ManagerAvpName = row.ManagerAvpName ?? string.Empty,
                ManagerAvpId = row.ManagerAvpId ?? string.Empty,
                ManagerFullName = row.ManagerFullName ?? string.Empty,
                ManagerZoneName = row.ManagerZoneName ?? string.Empty,
                ManagerZoneId = row.ManagerZoneId ?? string.Empty,
                ManagerRegionName = row.ManagerRegionName ?? string.Empty,
                ManagerRegionId = row.ManagerRegionId ?? string.Empty,
                ManagerBdName = row.ManagerBdName ?? string.Empty,
                ManagerBdId = row.ManagerBdId ?? string.Empty,
                ManagerTerritoryName = row.ManagerTerritoryName ?? string.Empty,
                ManagerTerritoryId = row.ManagerTerritoryId ?? string.Empty,
                ManagerID = row.ManagerID
            };

            ParseTerritories(row, node);
            DeriveDisplayFields(node);

            return node;
        }

        private static void ParseTerritories(ENTUserReportList row, ENTUserReportHierarchyNode node)
        {
            var idParts = string.IsNullOrWhiteSpace(row.TerritoryId)
                ? Array.Empty<string>()
                : row.TerritoryId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
            var nameParts = string.IsNullOrWhiteSpace(row.TerritoryName)
                ? Array.Empty<string>()
                : row.TerritoryName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
            node.TerritoryIds = idParts;
            node.TerritoryNames = nameParts;
            
            var sb = new StringBuilder();
            for (int i = 0; i < nameParts.Length; i++)
            {
                string name = nameParts[i];
                string id = i < idParts.Length ? idParts[i] : string.Empty;
                sb.Append(name);
                if (!string.IsNullOrEmpty(id))
                {
                    sb.Append(" (");
                    sb.Append(id);
                    sb.Append(")");
                }
                if (i < nameParts.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            node.TerritoryFilterText = sb.ToString();
        }

        private static void DeriveDisplayFields(ENTUserReportHierarchyNode node)
        {
            node.AvpFilter = node.AvpName;
            node.ZoneFilter = node.ZoneName;
            node.RegionFilter = node.RegionName;
            node.BdFilter = node.BDName;

            node.AvpDetail = FormatDetailValue(node.AvpName, node.AvpId);
            node.ZoneDetail = FormatDetailValue(node.ZoneName, node.ZoneId);
            node.RegionDetail = FormatDetailValue(node.RegionName, node.RegionId);
            node.BdDetail = FormatDetailValue(node.BDName, node.BDId);
            node.TerritoryDetailIds = node.TerritoryIds;
        }

        private static string MakeNodeId(ENTUserReportList row)
        {
            return $"u{row.UserId}_r{row.RoleId}_e{row.EntityId}";
        }

        private static string FormatDetailValue(string name, string id)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(id) || id == "0")
            {
                return name;
            }
            return name + " (" + id + ")";
        }

        private static int ParseInt(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return 0;
            }
            return int.TryParse(s.Trim(), out var v) ? v : 0;
        }
    }
}
