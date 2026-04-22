using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class RankMaster
    {
        public long RankId { get; set; }
        public string Rank { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long UpdatedBy { get; set; }
    }
}
