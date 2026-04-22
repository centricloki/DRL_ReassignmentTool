using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class RoleMaster
    {
        public RoleMaster()
        {
            UserMaster = new HashSet<UserMaster>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual UserMaster CreatedByNavigation { get; set; }
        public virtual UserMaster UpdatedByNavigation { get; set; }
        public virtual ICollection<UserMaster> UserMaster { get; set; }
    }
}
