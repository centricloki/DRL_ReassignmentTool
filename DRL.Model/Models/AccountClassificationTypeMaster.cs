using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class AccountClassificationTypeMaster
    {
        public long AccountClassificationId { get; set; }
        public string AccountClassificationName { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CustomerType { get; set; }
    }
}
