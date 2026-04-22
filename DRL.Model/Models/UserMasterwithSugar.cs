using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class UserMasterwithSugar
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string ContactNo { get; set; }
        public string Location { get; set; }
        public string UserName { get; set; }
        public string Pin { get; set; }
        public int RoleId { get; set; }
        public int ZoneId { get; set; }
        public int RegionId { get; set; }
        public string TerritoryId { get; set; }
        public string SellerRepresentativeTobaccoPermitNo { get; set; }
        public long ManagerId { get; set; }
        public int ImportedFrom { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public string SugarUserId { get; set; }
    }
}
