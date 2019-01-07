using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models
{
    [Table("log_pullorder")]
    public partial class LogPullOrder
    {
        public int PullOrderBgplogId { get; set; }
        public DateTime ProcessStartTime { get; set; }
        public DateTime ProcessEndTime { get; set; }
        public int ProcessStatus { get; set; }
        public string ProcessMessage { get; set; }
        public long ApiUserPlatformTokenId { get; set; }
    }
}
