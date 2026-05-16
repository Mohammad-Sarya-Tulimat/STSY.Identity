using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using STSY.Identity.Models;
using STSY.Microsoft.Identity.Models;

namespace STSY.Microsoft.Identity.DBContext
{
    public class STSYIdentityDbContext : IdentityDbContext<MicrosoftIdentityUser, MicrosoftIdentityRole, string>
    {
        public DbSet<MicrosoftIdentityPasswordHistory> PasswordHistories { get; set; }
        public DbSet<MicrosoftIdentityUserSession> UserSessions { get; set; }
        public DbSet<MicrosoftIdentityUserExternalLogin> UserExternalLogins { get; set; }
    }
}
