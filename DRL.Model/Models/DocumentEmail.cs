using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class DocumentEmail
    {
        public long DocumentEmailId { get; set; }
        public int DocumentType { get; set; }
        public string EmailTo { get; set; }
        public long? UserId { get; set; }
        public bool IsEmailSent { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
