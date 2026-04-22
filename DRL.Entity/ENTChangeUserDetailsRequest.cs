using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity
{
    public class ENTChangeUserDetailsRequest
    {
        public Int32? updateTerritoryId { get; set; }
        public Int32? addTeamId { get; set; }
        public Int32? deleteTeamId { get; set; }
        public long[] userIds { get; set; }
        public long UpdatedBy { get; set; }
    }
}
