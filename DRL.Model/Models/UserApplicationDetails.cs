using System;
using System.Collections.Generic;

namespace DRL.Model.Models
{
    public partial class UserApplicationDetails
    {
        public long UserApplicationDetailId { get; set; }
        public long UserId { get; set; }
        public DateTime? DateAdded { get; set; }
        public bool IsUpdated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsSendNotification { get; set; }
        public string NotificationCode { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceToken { get; set; }
        public int? CurrentVersion { get; set; }
        public int? AssignedVersion { get; set; }
        public string Iosversion { get; set; }
        public string ApplicationVersion { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
