using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class ProductRoleLink
    {
        public long ProductRoleLinkId { get; set; }
        public long CatBrandProductId { get; set; }
        public int Type { get; set; }
        public string TerritoryId { get; set; }
        public int RoleId { get; set; }
        public bool Status { get; set; }
        public int ImportedFrom { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
