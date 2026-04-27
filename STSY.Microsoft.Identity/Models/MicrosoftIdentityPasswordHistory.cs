using System;

namespace STSY.Identity.Models
{
    public class MicrosoftIdentityPasswordHistory
    {
        public Guid Id { get; set; }
        public string HashedPassword { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
        public string UserId { get; set; }
        public MicrosoftIdentityUser User { get; set; }
    }
}
