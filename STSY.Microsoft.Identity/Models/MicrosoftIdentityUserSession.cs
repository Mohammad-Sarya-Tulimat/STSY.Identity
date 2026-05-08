using STSY.Identity.Models;
using System;

namespace STSY.Microsoft.Identity.Models
{
    public class MicrosoftIdentityUserSession
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public MicrosoftIdentityUser User { get; set; }
        public string? IpAddress { get; set; }
        public string? Location { get; set; }
        public string HashedRefreshToken { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
