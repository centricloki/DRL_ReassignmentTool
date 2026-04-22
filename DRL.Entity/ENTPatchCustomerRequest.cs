using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Entity
{
    public class ENTPatchCustomerRequest
    {
        public long[] Id { get; set; }
        public bool status { get; set; }
        public long UpdatedBy { get; set; }
    }
}
