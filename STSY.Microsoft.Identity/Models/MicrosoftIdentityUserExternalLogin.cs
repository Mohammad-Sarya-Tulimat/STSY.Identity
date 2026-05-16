using STSY.Identity.Models;
using System;

namespace STSY.Microsoft.Identity.Models
{
    public class MicrosoftIdentityUserExternalLogin
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public MicrosoftIdentityUser User { get; set; }
        public string Provider { get; set; }
        public string ProviderUserId { get; set; }
        public DateTimeOffset LinkedAt { get; set; }
    }
}
