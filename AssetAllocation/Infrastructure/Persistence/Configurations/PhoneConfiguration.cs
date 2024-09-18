using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetAllocation.Api;

public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
{
    public void Configure(EntityTypeBuilder<Phone> builder)
    {
        builder.ToTable("Phones").HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.ModelId).IsRequired();
        builder.Property(x => x.Memory).IsRequired();
        builder.Property(x => x.Storage).IsRequired();
        builder.Property(x => x.Color).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.UpdatedDate);
        builder.Property(x => x.DeletedDate);

        builder.HasIndex(x => x.ModelId);

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasOne(x => x.Model).WithMany(x => x.Phones).HasForeignKey(x => x.ModelId);
        builder.HasMany(x => x.Allocations).WithOne(x => x.Phone).HasForeignKey(x => x.PhoneId);
    }
}
