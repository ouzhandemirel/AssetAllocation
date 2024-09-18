using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetAllocation.Api;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars").HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.ModelId).IsRequired();
        builder.Property(x => x.Year).IsRequired();
        builder.Property(x => x.Color).IsRequired();
        builder.Property(x => x.Plate).IsRequired();
        builder.Property(x => x.CurrentMileage).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.UpdatedDate);
        builder.Property(x => x.DeletedDate);

        builder.HasIndex(x => x.ModelId);

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasOne(x => x.Model).WithMany(x => x.Cars).HasForeignKey(x => x.ModelId);
        builder.HasMany(x => x.Mileages).WithOne(x => x.Car).HasForeignKey(x => x.CarId);
        builder.HasMany(x => x.Allocations).WithOne(x => x.Car).HasForeignKey(x => x.CarId);
    }
}
