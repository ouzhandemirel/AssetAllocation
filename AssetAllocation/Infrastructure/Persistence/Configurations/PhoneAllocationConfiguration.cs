using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetAllocation.Api;

public class PhoneAllocationConfiguration : IEntityTypeConfiguration<PhoneAllocation>
{
    public void Configure(EntityTypeBuilder<PhoneAllocation> builder)
    {
        builder.ToTable("PhoneAllocations").HasKey(a => a.Id);

        builder.Property(a => a.Id).ValueGeneratedNever().IsRequired();
        builder.Property(a => a.PhoneId).IsRequired();
        builder.Property(a => a.PersonId).IsRequired();
        builder.Property(a => a.AllocationDate).IsRequired();
        builder.Property(a => a.ReturnDate);
        builder.Property(a => a.CreatedDate).IsRequired();
        builder.Property(a => a.UpdatedDate);
        builder.Property(a => a.DeletedDate);

        builder.HasIndex(a => a.PhoneId);
        builder.HasIndex(a => a.PersonId);

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasOne(a => a.Phone).WithMany(a => a.Allocations).HasForeignKey(a => a.PhoneId);
        builder.HasOne(a => a.Person).WithMany(a => a.PhoneAllocations).HasForeignKey(a => a.PersonId);
    }
}
