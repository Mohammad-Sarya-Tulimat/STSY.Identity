using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STSY.Microsoft.Identity.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace STSY.Microsoft.Identity.DBContext.Mappers
{
    internal class SessionEFCoreMapper : IEntityTypeConfiguration<MicrosoftIdentityUserSession>
    {
        public void Configure(EntityTypeBuilder<MicrosoftIdentityUserSession> builder)
        {
            builder.ToTable("IdentitySession");
            builder.HasKey(m => m.Id);
            builder.Property(s => s.SessionType).HasMaxLength(40);
            builder.HasOne(s => s.User).WithMany(s => s.UserSessions).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            builder
                .Property(m => m.ProtectedData)
                .HasConversion(
                    v => JsonSerializer.Serialize(v),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v)
                )
                .IsRequired(false);

        }
    }
}
