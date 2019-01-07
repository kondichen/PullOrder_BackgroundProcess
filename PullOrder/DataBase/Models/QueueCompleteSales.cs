using System;
using System.Collections.Generic;

namespace DataBase.Models
{
    public partial class QueueCompleteSales
    {
        public int QueueCompleteSalesId { get; set; }
        public string ApiUserId { get; set; }
        public string CompleteSalesRequestId { get; set; }
    }
}
