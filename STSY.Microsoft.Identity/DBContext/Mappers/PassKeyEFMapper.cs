using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace STSY.Microsoft.Identity.DBContext.Mappers
{
    internal class PassKeyEFMapper : IEntityTypeConfiguration<IdentityUserPasskey<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserPasskey<string>> builder)
        {
            builder.Ignore(c => c.Data);
            builder.OwnsOne(x => x.Data, passkey =>
            {
                passkey.Property(m => m.Transports)
                .HasConversion(v => JsonSerializer.Serialize(v),
                    v => JsonSerializer.Deserialize<string[]>(v)).IsRequired(false);

            });

        }
    }
}
