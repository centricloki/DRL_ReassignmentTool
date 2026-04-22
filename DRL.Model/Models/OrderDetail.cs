using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class OrderDetail
    {
        public long OrderDetailId { get; set; }
        public long OrderId { get; set; }
        public long CategoryId { get; set; }
        public long BrandIstyleId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long StyleId { get; set; }
        public string Units { get; set; }
        public string CreditRequest { get; set; }
    }
}
