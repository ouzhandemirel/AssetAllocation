using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetAllocation.Api;

public class PersonOperationClaimConfiguration : IEntityTypeConfiguration<PersonOperationClaim>
{
    public void Configure(EntityTypeBuilder<PersonOperationClaim> builder)
    {
        builder.ToTable("PersonOperationClaims").HasKey(x => x.Id);

        builder.Property(x => x.Id).UseIdentityByDefaultColumn().IsRequired();
        builder.Property(x => x.PersonId).IsRequired();
        builder.Property(x => x.OperationClaimId).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.UpdatedDate);
        builder.Property(x => x.DeletedDate);

        builder.HasIndex(x => x.PersonId);

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasOne(x => x.Person).WithMany(x => x.PersonOperationClaims).HasForeignKey(x => x.PersonId);
        builder.HasOne(x => x.OperationClaim).WithMany(x => x.PersonOperationClaims).HasForeignKey(x => x.OperationClaimId);
    }
}
