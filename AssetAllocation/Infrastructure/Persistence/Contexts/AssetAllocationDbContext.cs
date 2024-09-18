using System.Collections;
using System.IO.Compression;
using System.Reflection;
using AssetAllocation.Api.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AssetAllocation.Api;

public class AssetAllocationDbContext : DbContext, IAssetAllocationDbContext
{
    private readonly IDomainEventService _domainEventService;

    #region Entity Sets
    public virtual DbSet<CarAllocation> CarAllocations { get; set; }
    public virtual DbSet<PhoneAllocation> PhoneAllocations { get; set; }
    public virtual DbSet<Car> Cars { get; set; }
    public virtual DbSet<CarMake> CarMakes { get; set; }
    public virtual DbSet<CarModel> CarModels { get; set; }
    public virtual DbSet<Mileage> Mileages { get; set; }
    public virtual DbSet<Person> Persons { get; set; }
    public virtual DbSet<Phone> Phones { get; set; }
    public virtual DbSet<PhoneBrand> PhoneBrands { get; set; }
    public virtual DbSet<PhoneModel> PhoneModels { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<OperationClaim> OperationClaims { get; set; }
    public virtual DbSet<PersonOperationClaim> PersonOperationClaims { get; set; }
    public virtual DbSet<EmailAuthenticator> EmailAuthenticators { get; set; }
    public virtual DbSet<OtpAuthenticator> OtpAuthenticators { get; set; }
    #endregion

    public AssetAllocationDbContext(
        DbContextOptions<AssetAllocationDbContext> options,
        IDomainEventService domainEventService)
        : base(options)
    {
        _domainEventService = domainEventService;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        TagInsertedEntities();
        TagUpdatedEntities();

        var events = ChangeTracker.Entries<IHasDomainEvent>()
            .Select(x => x.Entity.DomainEvents)
            .SelectMany(x => x)
            .Where(x => !x.IsPublished)
            .ToArray();

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents(events);

        return result;
    }

    private async Task DispatchEvents(DomainEvent[] domainEvents)
    {
        foreach (var @event in domainEvents)
        {
            @event.IsPublished = true;
            await _domainEventService.Publish(@event);
        }
    }

    private void TagInsertedEntities()
    {
        var addedEntities = ChangeTracker.Entries<IEntityTimestamps>()
            .Where(e => e.State == EntityState.Added)
            .ToList();

        foreach (var entity in addedEntities)
        {
            entity.Entity.CreatedDate = DateTime.UtcNow;
        }
    }

    private void TagUpdatedEntities()
    {
        var modifiedEntities = ChangeTracker.Entries<IEntityTimestamps>()
            .Where(e => e.State == EntityState.Modified && e.Entity.DeletedDate is null)
            .ToList();

        foreach (var entity in modifiedEntities)
        {
            entity.Entity.UpdatedDate = DateTime.UtcNow;
        }
    }

    public async Task SoftRemoveAsync(IEntityTimestamps entity)
    {
        CheckEntityForOneToOneRelation(entity);
        await SetEntityAsSoftDeletedAsync(entity);
    }

    public async Task SoftRemoveRangeAsync(IEnumerable<IEntityTimestamps> entities)
    {
        foreach (IEntityTimestamps entity in entities)
        {
            CheckEntityForOneToOneRelation(entity);
        }

        foreach (IEntityTimestamps entity in entities)
        {
            await SetEntityAsSoftDeletedAsync(entity);
        }
    }

    protected void CheckEntityForOneToOneRelation(IEntityTimestamps entity)
    {
        var keys = this.Entry(entity)
            .Metadata.GetForeignKeys();

        bool hasOneToOneRelation =
            this.Entry(entity)
            .Metadata.GetForeignKeys()
            .All(x =>
                x.DependentToPrincipal?.IsCollection == true ||
                x.PrincipalToDependent?.IsCollection == true ||
                x.DependentToPrincipal?.ForeignKey.DeclaringEntityType.ClrType == entity.GetType()
            ) == false;

        if (hasOneToOneRelation)
        {
            throw new InvalidOperationException("Entity has one to one relation. Soft delete causes problems if you try to create a new entity with the same primary key.");
        }
    }

    private async Task SetEntityAsSoftDeletedAsync(IEntityTimestamps entity)
    {
        if (entity.DeletedDate.HasValue)
        {
            return;
        }

        entity.DeletedDate = DateTime.UtcNow;

        var navigations = this
            .Entry(entity)
            .Metadata.GetNavigations()
            .Where(x => x is { IsOnDependent: false, ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade })
            .ToList();

        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
            {
                continue;
            }

            if (navigation.PropertyInfo == null)
            {
                continue;
            }

            object? navValue = navigation.PropertyInfo.GetValue(entity);

            if (navigation.IsCollection)
            {
                if (navValue == null)
                {
                    IQueryable query = this.Entry(entity).Collection(navigation.PropertyInfo.Name).Query();
                    navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).ToListAsync();

                    if (navValue == null)
                    {
                        continue;
                    }
                }

                foreach (IEntityTimestamps navValueItem in (IEnumerable)navValue)
                {
                    await SetEntityAsSoftDeletedAsync(navValueItem);
                }
            }
            else
            {
                if (navValue == null)
                {
                    IQueryable query = this.Entry(entity).Reference(navigation.PropertyInfo.Name).Query();
                    navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType())
                        .FirstOrDefaultAsync();

                    if (navValue == null)
                    {
                        continue;
                    }
                }

                await SetEntityAsSoftDeletedAsync((IEntityTimestamps)navValue);
            }
        }

        this.Update(entity);
    }

    protected IQueryable<object> GetRelationLoaderQuery(IQueryable query, Type navigationPropertyType)
    {
        Type queryProviderType = query.Provider.GetType();

        MethodInfo createQueryMethod =
            queryProviderType
                .GetMethods()
                .First(m => m is { Name: nameof(query.Provider.CreateQuery), IsGenericMethod: true })
                ?.MakeGenericMethod(navigationPropertyType)
            ?? throw new InvalidOperationException("CreateQuery<TElement> method is not found in IQueryProvider.");

        IQueryable<object> queryProviderQuery =
            (IQueryable<object>)createQueryMethod.Invoke(query.Provider, parameters: [query.Expression])!;

        return queryProviderQuery.Where(x => !((IEntityTimestamps)x).DeletedDate.HasValue);
    }
}
