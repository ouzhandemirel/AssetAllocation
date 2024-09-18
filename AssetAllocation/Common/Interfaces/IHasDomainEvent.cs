namespace AssetAllocation.Api;

public interface IHasDomainEvent
{
    public List<DomainEvent> DomainEvents { get; }
}
