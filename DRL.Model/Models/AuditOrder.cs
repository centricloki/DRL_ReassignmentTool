using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class AuditOrder
    {
        public long AuditOrderId { get; set; }
        public long UserId { get; set; }
        public DateTime AuditDateTime { get; set; }
        public long OrderId { get; set; }
        public string LoginUserName { get; set; }
        public string LoginPassword { get; set; }
        public string ErrorMsg { get; set; }
    }
}
