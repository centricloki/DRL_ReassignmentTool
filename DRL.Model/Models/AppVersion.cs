using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class AppVersion
    {
        public int AppVersionId { get; set; }
        public string Version { get; set; }
        public string DownloadUrl { get; set; }
        public bool? IsProduction { get; set; }
        public bool? IsActive { get; set; }
        public string WhatsNew { get; set; }
    }
}
