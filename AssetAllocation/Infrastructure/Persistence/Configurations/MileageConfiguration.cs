using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetAllocation.Api;

public class MileageConfiguration : IEntityTypeConfiguration<Mileage>
{
    public void Configure(EntityTypeBuilder<Mileage> builder)
    {
        builder.ToTable("CarMileages").HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.CarId).IsRequired();
        builder.Property(x => x.Counter).IsRequired();
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.UpdatedDate);
        builder.Property(x => x.DeletedDate);

        builder.HasIndex(x => x.CarId);

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasOne(x => x.Car).WithMany(x => x.Mileages).HasForeignKey(x => x.CarId);
    }
}
