using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetAllocation.Api;

public class CarAllocationConfiguration : IEntityTypeConfiguration<CarAllocation>
{
    public void Configure(EntityTypeBuilder<CarAllocation> builder)
    {
        builder.ToTable("CarAllocations").HasKey(a => a.Id);

        builder.Property(a => a.Id).ValueGeneratedNever().IsRequired();
        builder.Property(a => a.CarId).IsRequired();
        builder.Property(a => a.PersonId).IsRequired();
        builder.Property(a => a.AllocationDate).IsRequired();
        builder.Property(a => a.ReturnDate);
        builder.Property(a => a.CreatedDate).IsRequired();
        builder.Property(a => a.UpdatedDate);
        builder.Property(a => a.DeletedDate);

        builder.HasIndex(a => a.CarId);
        builder.HasIndex(a => a.PersonId);

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.Ignore(a => a.DomainEvents);

        builder.HasOne(a => a.Car).WithMany(a => a.Allocations).HasForeignKey(a => a.CarId);
        builder.HasOne(a => a.Person).WithMany(a => a.CarAllocations).HasForeignKey(a => a.PersonId);
    }
}
