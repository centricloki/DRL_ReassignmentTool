using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class ContactMaster
    {
        public long ContactId { get; set; }
        public string DeviceContactId { get; set; }
        public long CustomerId { get; set; }
        public long? RankId { get; set; }
        public long? PositionId { get; set; }
        public string ContactName { get; set; }
        public string ContactRole { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactCellPhone { get; set; }
        public string ContactFax { get; set; }
        public string ContactNote { get; set; }
        public string SugarContactId { get; set; }
        public int ImportedFrom { get; set; }
        public bool IsExported { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long UpdatedBy { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? ContactTypeId { get; set; }
    }
}
