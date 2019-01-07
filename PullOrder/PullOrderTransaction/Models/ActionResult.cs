using System;
using System.Collections.Generic;
using System.Text;

namespace PullOrderTransaction.Models
{
    public class ActionResult
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string GetMessage { get; set; }
    }
}
