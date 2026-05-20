using System;

namespace DRL.Entity.Response
{
    public class ENTBDResponse
    {
        public int BDID { get; set; }
        public string BDName { get; set; }
        public bool IsActive { get; set; }
        public bool Approver { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
