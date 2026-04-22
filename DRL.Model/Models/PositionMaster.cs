using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class PositionMaster
    {
        public long PositionId { get; set; }
        public string Position { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long UpdatedBy { get; set; }
        public bool IsSecure { get; set; }
    }
}
