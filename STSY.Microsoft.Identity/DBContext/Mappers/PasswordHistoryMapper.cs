using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STSY.Identity.Models;

namespace STSY.Microsoft.Identity.DBContext.Mappers
{
    internal class PasswordHistoryMapper : IEntityTypeConfiguration<MicrosoftIdentityPasswordHistory>
    {
        public void Configure(EntityTypeBuilder<MicrosoftIdentityPasswordHistory> builder)
        {
            builder.ToTable("Identity_password_history");
            builder.HasKey(m => m.Id);
            builder.HasOne(s => s.User).WithMany(s => s.PasswordHistories).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
