using System;

namespace DRL.Entity
{
    public class ENTUserGroup
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string LandingPage { get; set; }
        public bool IsAdmin { get; set; }
        //public DateTime CreatedDate { get; set; }
    }

    //public class ENTUserGroupConstants
    //{
    //    public string SalesGroup { get; set; }
    //    public string DRLITGroup { get; set; }
    //}
}
