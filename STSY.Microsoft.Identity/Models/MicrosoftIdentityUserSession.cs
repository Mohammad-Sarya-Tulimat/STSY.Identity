using STSY.Identity.Models;
using System;
using System.Collections.Generic;

namespace STSY.Microsoft.Identity.Models
{
    public class MicrosoftIdentityUserSession
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string SessionType { get; set; }
        public MicrosoftIdentityUser User { get; set; }
        public string? IpAddress { get; set; }
        public string? Location { get; set; }
        public IDictionary<string, object> ProtectedData { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset ExpiredAt { get; set; } = DateTimeOffset.Now;
    }
}
