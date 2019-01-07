using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models
{
    [Table("queue_pullorder_user_token")]
    public partial class QueuePullOrderUserToken
    {
        public int LoginUserTokenQueueId { get; set; }
        public long ApiUserPlatformTokenId { get; set; }
        public int NumberOfDays { get; set; }
        public bool IsUsed { get; set; }
        public int OrderTotal { get; set; }
    }
}
