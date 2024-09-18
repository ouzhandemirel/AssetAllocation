using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetAllocation.Api;

public class PhoneBrandConfiguration : IEntityTypeConfiguration<PhoneBrand>
{
    public void Configure(EntityTypeBuilder<PhoneBrand> builder)
    {
        builder.ToTable("PhoneBrands").HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Country).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.UpdatedDate);
        builder.Property(x => x.DeletedDate);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasMany(x => x.Models).WithOne(x => x.Brand).HasForeignKey(x => x.BrandId);
    }
}
