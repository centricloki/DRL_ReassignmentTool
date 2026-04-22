using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class UserTaxStatement
    {
        public long UserTaxStatementId { get; set; }
        public string DeviceUserTaxStatementId { get; set; }
        public long UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsExported { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
    }
}
