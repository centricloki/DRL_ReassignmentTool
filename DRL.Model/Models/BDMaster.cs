using System;

namespace DRL.Model.Models
{
    public partial class BDMaster
    {
        public int BDID { get; set; }
        public string BDName { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool Approver { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
    }
}
