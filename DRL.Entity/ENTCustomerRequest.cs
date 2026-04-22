using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DRL.Entity
{
    public class ENTCustomerRequest
    {
        [DefaultValue(1)]
        public int page { get; set; }

        [DefaultValue(10)]
        public int pageSize { get; set; }

        [DefaultValue(0)]
        public int userId { get; set; }

        [DefaultValue(0)]
        public int territoryId { get; set; }

        [DefaultValue("")]
        public string address { get; set; }

        [DefaultValue("")]
        public string city { get; set; }

        [DefaultValue("")]
        public string state { get; set; }

        [DefaultValue("")]
        public string zipCode { get; set; }

        [DefaultValue("")]
        public string customerName { get; set; }

        [DefaultValue(true)]
        public bool customerMatch { get; set; }

        [DefaultValue(1)]
        public int accountType { get; set; }

        [DefaultValue(false)]
        public bool includeDeleted { get; set; }

        [DefaultValue("CustomerName")]
        public string orderBy { get; set; }

        [DefaultValue("ASC")]
        public string sortOrder { get; set; }
        public string parentNumber { get; set; }
        [DefaultValue(0)]
        public int teamId { get; set; }
    }
}
