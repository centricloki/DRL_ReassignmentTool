using System;

namespace DRL.Model.Models
{
    public partial class AVPMaster
    {
        public int AVPID { get; set; }

        public string AVPName { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
