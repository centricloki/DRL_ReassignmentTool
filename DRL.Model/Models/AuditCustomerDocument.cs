using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class AuditCustomerDocument
    {
        public long AuditCustomerDocumentId { get; set; }
        public long UserId { get; set; }
        public DateTime AuditDateTime { get; set; }
        public long CustomerDocumentId { get; set; }
        public string LoginUserName { get; set; }
        public string LoginPassword { get; set; }
        public string ErrorMsg { get; set; }
    }
}
