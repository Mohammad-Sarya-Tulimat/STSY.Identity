using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using STSY.Identity.Models;
using STSY.Microsoft.Identity.DBContext.Mappers;
using STSY.Microsoft.Identity.Models;

namespace STSY.Microsoft.Identity.DBContext
{
    public class STSYIdentityDbContext : IdentityDbContext<MicrosoftIdentityUser, MicrosoftIdentityRole, string>
    {
        public STSYIdentityDbContext(DbContextOptions<STSYIdentityDbContext> options)
    : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new SessionEFCoreMapper());
            builder.ApplyConfiguration(new UserExternalLoginEfCoreMapper());
            builder.ApplyConfiguration(new PasswordHistoryMapper());
            builder.ApplyConfiguration(new PassKeyEFMapper());
        }
        public DbSet<MicrosoftIdentityPasswordHistory> PasswordHistories { get; set; }
        public DbSet<MicrosoftIdentityUserSession> UserSessions { get; set; }
        public DbSet<MicrosoftIdentityUserExternalLogin> UserExternalLogins { get; set; }
    }
}
