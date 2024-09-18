using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetAllocation.Api;

public class PhoneModelConfiguration : IEntityTypeConfiguration<PhoneModel>
{
    public void Configure(EntityTypeBuilder<PhoneModel> builder)
    {
        builder.ToTable("PhoneModels").HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.BrandId).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.UpdatedDate);
        builder.Property(x => x.DeletedDate);

        builder.HasIndex(x => x.BrandId);

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasOne(x => x.Brand).WithMany(x => x.Models).HasForeignKey(x => x.BrandId);
        builder.HasMany(x => x.Phones).WithOne(x => x.Model).HasForeignKey(x => x.ModelId);
    }
}
