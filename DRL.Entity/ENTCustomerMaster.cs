using System;

namespace DRL.Entity
{
    public class ENTCustomerMaster
    {
        public string CustomerName { get; set; }
        public string EmailId { get; set; }
        public int AccountType { get; set; }
        public int AccountClassification { get; set; }
        public int ZoneID { get; set; }
        public int RegionID { get; set; }
        public int TerritoryID { get; set; }
        public int ImportedFrom { get; set; }
        public int IsExported { get; set; }
        public string PhysicalAddress { get; set; }
        public string PhysicalAddressCity { get; set; }
        public int PhysicalAddressStateID { get; set; }
        public string PhysicalAddressZipCode { get; set; }
        public string MailingAddress { get; set; }
        public string MailingAddressCity { get; set; }
        public int MailingAddressStateID { get; set; }
        public string MailingAddressZipCode { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingAddressCity { get; set; }
        public int ShippingAddressStateID { get; set; }
        public string ShippingAddressZipCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public int Parent { get; set; }
        public int OldTerritoryId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
