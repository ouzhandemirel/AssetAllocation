namespace AssetAllocation.Api;

public sealed class CarAllocatedEvent : DomainEvent
{
    public CarAllocation CarAllocation { get; }
    
    public CarAllocatedEvent(CarAllocation carAllocation)
    {
        CarAllocation = carAllocation;
    }
}
