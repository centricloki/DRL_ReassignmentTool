using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity.Response
{
    public class ENTActionHistoryResponse
    {
        public string Module { get; set; }
        public string Operation { get; set; }
        public string AccountName { get; set; }
        public string CustomerNumber { get; set; }
        public string Team { get; set; }
        public bool IsUpdateTeam { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
