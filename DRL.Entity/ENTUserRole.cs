using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity
{
    public class ENTRole
    {
        public ENTRole()
        {
            //Users = new List<ENTUser>();
        }

        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }

        //public ICollection<ENTUser> Users { get; set; }
    }
}
