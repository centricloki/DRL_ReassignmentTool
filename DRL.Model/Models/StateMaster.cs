using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class StateMaster
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public DateTime UpdateDate { get; set; }
        public string TaxStatement { get; set; }
    }
}
