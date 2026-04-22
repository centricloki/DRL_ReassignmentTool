using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class BrandStyleMaster
    {
        public long BrandIstyleId { get; set; }
        public string BrandStyleName { get; set; }
        public long ParentId { get; set; }
        public bool IsBrand { get; set; }
        public int ImportedFrom { get; set; }
        public int Status { get; set; }
        public string ImageFilePath { get; set; }
        public int PromoId { get; set; }
        public int LangId { get; set; }
        public int Priority { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsPopOrder { get; set; }
        public int? SortOrder { get; set; }
    }
}
