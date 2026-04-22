using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class StyleMaster
    {
        public long StyleId { get; set; }
        public string StyleName { get; set; }
        public long ParentId { get; set; }
        public bool IsStyle { get; set; }
        public int ImportedFrom { get; set; }
        public int Status { get; set; }
        public string ImageFilePath { get; set; }
        public int PromoId { get; set; }
        public int LangId { get; set; }
        public int Priority { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
