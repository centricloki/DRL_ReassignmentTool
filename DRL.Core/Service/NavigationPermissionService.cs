using DRL.Core.Interface;
using DRL.Core.Mapper;
using DRL.Entity;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using DRL.Model.Models;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;

using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DRL.Core.Service
{
    public class NavigationPermissionService : INavigationPermissionService
    {
        private readonly string _connectionString;
        private readonly ICacheService _cacheService;

        public NavigationPermissionService(IConfiguration config, ICacheService cacheService)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            _cacheService = cacheService;
        }

        public async Task<List<ENTLinkPermission>> GetPermissionsForGroupAsync(string groupName)
        {
            var permissions = new List<ENTLinkPermission>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var sql = @"
                SELECT nl.LinkCode, lgp.IsVisible
                FROM RT_NavigationLinks nl
                INNER JOIN RT_LinkGroupPermissions lgp ON nl.LinkId = lgp.LinkId
                INNER JOIN RT_UserGroups ug ON lgp.GroupId = ug.GroupId
                WHERE ug.GroupName = @GroupName 
                  AND nl.IsActive = 1 
                  AND ug.IsActive = 1
                ORDER BY nl.DisplayOrder";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@GroupName", groupName);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            permissions.Add(new ENTLinkPermission
                            {
                                LinkCode = reader["LinkCode"].ToString(),
                                IsVisible = Convert.ToBoolean(reader["IsVisible"]),
                            });
                        }
                    }
                }
            }
            return permissions;
        }

        public async Task<Dictionary<int, string>> GetActiveUserGroupsAsync()
        {
            const string cacheKey = "ActiveUserGroups";
            if (_cacheService.TryGetValue(cacheKey, out Dictionary<int, string> cachedGroups))
            {
                return cachedGroups;
            }

            var groups = new Dictionary<int, string>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var sql = "SELECT GroupId, GroupName FROM RT_UserGroups WHERE IsActive = 1";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = Convert.ToInt32(reader["GroupId"]);
                            string name = reader["GroupName"].ToString();
                            groups[id] = name;
                        }
                    }
                }
            }

            _cacheService.Set(cacheKey, groups, TimeSpan.FromHours(1));
            return groups;
        }

        public void ClearUserGroupCache()
        {
            _cacheService.Remove("ActiveUserGroups");
        }
    }
}
