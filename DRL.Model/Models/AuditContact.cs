using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class AuditContact
    {
        public long AuditContactId { get; set; }
        public long UserId { get; set; }
        public DateTime AuditDateTime { get; set; }
        public string ContactId { get; set; }
        public string LoginUserName { get; set; }
        public string LoginPassword { get; set; }
        public string ErrorMsg { get; set; }
        public int Mode { get; set; }
    }
}
