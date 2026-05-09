using Microsoft.AspNetCore.Identity;
using System;

namespace STSY.Identity.Models
{
    public class MicrosoftIdentityUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset SecurityChangeSessionExpiresAt { get; set; }
        public string? SecurityChangeSessionId { get; set; }

        public bool IsStepUpEnabled(string sessionId)
        {
            return SecurityChangeSessionId == sessionId && SecurityChangeSessionExpiresAt > DateTimeOffset.UtcNow;
        }
        public void UpdateStepUp(string sessionId, DateTimeOffset expiresAt)
        {
            SecurityChangeSessionId = sessionId;
            SecurityChangeSessionExpiresAt = expiresAt;
        }
        public void RemoveStepUp()
        {
            SecurityChangeSessionId = null;
            SecurityChangeSessionExpiresAt = DateTimeOffset.MinValue;
        }
    }
}
