namespace AssetAllocation.Api;

public abstract class DomainEvent
{
    public bool IsPublished { get; set; }
    public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
}