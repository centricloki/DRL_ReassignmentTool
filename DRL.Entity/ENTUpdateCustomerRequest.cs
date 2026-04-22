using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity
{
    public class ENTUpdateCustomerRequest
    {
        public Int32 updateTerritoryId { get; set; }
        public Int32 addTeamId { get; set; }
        public Int32 deleteTeamId { get; set; }
        public long[] customerIds { get; set; }
        public long UpdatedBy { get; set; }
    }
}
