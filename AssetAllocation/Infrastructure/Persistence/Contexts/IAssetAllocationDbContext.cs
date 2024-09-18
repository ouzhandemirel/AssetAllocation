using System;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api.Infrastructure.Persistence.Contexts;

public interface IAssetAllocationDbContext
{
    DbSet<CarAllocation> CarAllocations { get; set; }
    DbSet<PhoneAllocation> PhoneAllocations { get; set; }
    DbSet<Car> Cars { get; set; }
    DbSet<CarMake> CarMakes { get; set; }
    DbSet<CarModel> CarModels { get; set; }
    DbSet<Mileage> Mileages { get; set; }
    DbSet<Person> Persons { get; set; }
    DbSet<Phone> Phones { get; set; }
    DbSet<PhoneBrand> PhoneBrands { get; set; }
    DbSet<PhoneModel> PhoneModels { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }
    DbSet<OperationClaim> OperationClaims { get; set; }
    DbSet<PersonOperationClaim> PersonOperationClaims { get; set; }
    DbSet<EmailAuthenticator> EmailAuthenticators { get; set; }
    DbSet<OtpAuthenticator> OtpAuthenticators { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task SoftRemoveAsync(IEntityTimestamps entity);
    Task SoftRemoveRangeAsync(IEnumerable<IEntityTimestamps> entities);

}
