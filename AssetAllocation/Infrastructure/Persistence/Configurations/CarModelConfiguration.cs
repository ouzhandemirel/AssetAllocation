using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetAllocation.Api;

public class CarModelConfiguration : IEntityTypeConfiguration<CarModel>
{
    public void Configure(EntityTypeBuilder<CarModel> builder)
    {
        builder.ToTable("CarModels").HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.MakeId).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.UpdatedDate);
        builder.Property(x => x.DeletedDate);

        builder.HasIndex(x => x.MakeId);

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasOne(x => x.Make).WithMany(x => x.Models).HasForeignKey(x => x.MakeId);
        builder.HasMany(x => x.Cars).WithOne(x => x.Model).HasForeignKey(x => x.ModelId);
    }
}
