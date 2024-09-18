using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetAllocation.Api;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens").HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.PersonId).IsRequired();
        builder.Property(x => x.Token).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.AbsoluteExpiration).IsRequired();
        builder.Property(x => x.CreatedByIp).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.RevokedAt);
        builder.Property(x => x.RevokedByIp);
        builder.Property(x => x.ReplacedByToken);
        builder.Property(x => x.ReasonRevoked);
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.UpdatedDate);
        builder.Property(x => x.DeletedDate);

        builder.HasIndex(x => x.PersonId);
        builder.HasIndex(x => x.Token);

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasOne(x => x.Person).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.PersonId);
    }
}
