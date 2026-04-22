using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity
{
    public class ENTBrandStyleMaster
    {
        public int BrandIStyleID { get; set; }
        public string BrandStyleName { get; set; }
        public string Description { get; set; }
        public string ImageFilePath { get; set; }
        public int ParentID { get; set; }
        public int SortOrder { get; set; }
    }
}
