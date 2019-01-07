using System;
using System.Collections.Generic;
using System.Text;

namespace PullOrderTransaction.Models
{
    public class PullOrdersPayload
    {
        public long apiUserPlatformTokenId { get; set; }
        public int numberOfDays { get; set; }
    }
}
