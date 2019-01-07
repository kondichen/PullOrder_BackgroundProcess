using System;
using System.Collections.Generic;

namespace DataBase.Models
{
    public partial class ApiUserPlatformToken
    {
        public long ApiUserPlatformTokenId { get; set; }
        public string ApiId { get; set; }
        public string ApiUserId { get; set; }
        public string PlatformCode { get; set; }
        public string PlatformUsername { get; set; }
        public string PlatformUserKey { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpirationTime { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime ModifyOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
