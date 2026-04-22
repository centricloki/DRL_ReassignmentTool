using System;

namespace DRL.Entity
{
    public class CustomerMasterRequest
    {
        public string CustomerName { get; set; }
        public string EmailId { get; set; }
        public string Address { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressZipCode { get; set; }
        public string TerritoryCode { get; set; }
        public Boolean IsParent { get; set; }
        public string Parent { get; set; }
        public string AccountType { get; set; }
        public string AccountClassification { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
