using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity
{
    public class ENTBD
    {
        public int BDID { get; set; }
        public string BDName { get; set; }
        public DateTime UpdateDate { get; set; } = System.DateTime.UtcNow;
        public DateTime CreatedDate { get; set; } = System.DateTime.UtcNow;
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public bool Approver { get; set; }
        public List<ENTTeam> Teams { get; set; }
    }
}
