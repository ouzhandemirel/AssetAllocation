namespace AssetAllocation.Api;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}
