using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetAllocation.Api;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Persons").HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Surname).IsRequired();
        builder.Property(x => x.DateOfBirth).IsRequired();
        builder.Property(x => x.RegistrationNumber).IsRequired();
        builder.Property(x => x.Department).IsRequired();
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.AuthenticatorType).IsRequired();
        builder.Property(x => x.PasswordSalt).IsRequired();
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
        builder.Property(x => x.UpdatedDate);
        builder.Property(x => x.DeletedDate);

        builder.HasIndex(x => x.RegistrationNumber).IsUnique();

        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);

        builder.HasMany(x => x.PhoneAllocations).WithOne(x => x.Person).HasForeignKey(x => x.PersonId);
        builder.HasMany(x => x.CarAllocations).WithOne(x => x.Person).HasForeignKey(x => x.PersonId);
        builder.HasMany(x => x.RefreshTokens).WithOne(x => x.Person).HasForeignKey(x => x.PersonId);
        builder.HasMany(x => x.PersonOperationClaims).WithOne(x => x.Person).HasForeignKey(x => x.PersonId);
        builder.HasMany(x => x.OtpAuthenticators).WithOne(x => x.Person).HasForeignKey(x => x.PersonId);
        builder.HasMany(x => x.EmailAuthenticators).WithOne(x => x.Person).HasForeignKey(x => x.PersonId);
    }
}
