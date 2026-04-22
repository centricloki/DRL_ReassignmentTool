using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class CategoryProduct
    {
        public long CategoryProductId { get; set; }
        public long ProductId { get; set; }
        public long BrandIstyleId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
