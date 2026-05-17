using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STSY.Microsoft.Identity.Models;

namespace STSY.Microsoft.Identity.DBContext.Mappers
{
    internal class UserExternalLoginEfCoreMapper : IEntityTypeConfiguration<MicrosoftIdentityUserExternalLogin>
    {
        public void Configure(EntityTypeBuilder<MicrosoftIdentityUserExternalLogin> builder)
        {
            builder.ToTable("user_external_login");
            builder.HasKey(m => m.Id);
            builder.Property(s => s.Provider).HasMaxLength(40);
            builder.Property(s => s.ProviderUserId).HasMaxLength(40);
            builder.HasOne(s => s.User).WithMany(s => s.UserExternalLogins).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(s => new { s.Provider, s.ProviderUserId });
        }
    }
}
