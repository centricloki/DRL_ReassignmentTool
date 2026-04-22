using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity
{
    public class ENTTerriotyUsers
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string HoneyPin { get; set; }
        public string ReportsTo { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public bool IsTerritoryUser { get; set; }

        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
    }
}
