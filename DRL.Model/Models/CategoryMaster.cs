using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class CategoryMaster
    {
        public long CategoryId { get; set; }
        public int? ErpcategoryId { get; set; }
        public string CategoryName { get; set; }
        public long ParentCategoryId { get; set; }
        public int CategoryStatus { get; set; }
        public string ImageFilePath { get; set; }
        public int PromoId { get; set; }
        public int LangId { get; set; }
        public int Priority { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
