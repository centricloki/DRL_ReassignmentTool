using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Model.Models
{
    public class RT_NavigationLink
    {
        public int LinkId { get; set; }
        public string LinkCode { get; set; }
        public string LinkName { get; set; }
        public string RoutePath { get; set; }
        public string IconClass { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class RT_LinkPermission
    {
        public string LinkCode { get; set; }
        public bool IsVisible { get; set; }
        public bool IsEnabled { get; set; }
    }
}
