using System;

namespace STSY.Identity.Models
{
    internal class PasswordHistory
    {
        public Guid Id { get; set; }
        public string HashedPassword { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
