using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models
{
    [Table("queue_pullorder")]
    public partial class QueuePullOrder
    {
        public int PullOrderQueueId { get; set; }
        public long ApiUserPlatformTokenId { get; set; }
        public Guid PullOrderFromID { get; set; }
        public string OrderId { get; set; }
        public string SiteCode { get; set; }
        public bool IsUsed { get; set; }
        
    }
}
