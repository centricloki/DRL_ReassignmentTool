using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity
{
    public class ENTAppUser
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string UserGroup { get; set; }
        public bool IsAdmin { get; set; }
        public string Token { get; set; }
        public string WindowGroupName { get; set; }
        public List<ENTLinkPermission> LinkPermissions { get; set; }
    }
    public class ENTLinkPermission
    {
        public string LinkCode { get; set; }
        public bool IsVisible { get; set; }
    }
}
