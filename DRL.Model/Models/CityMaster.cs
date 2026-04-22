using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class CityMaster
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int StateId { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
