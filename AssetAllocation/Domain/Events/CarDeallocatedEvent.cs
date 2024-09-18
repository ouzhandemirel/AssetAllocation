namespace AssetAllocation.Api;

public sealed class CarDeallocatedEvent : DomainEvent
{
    public CarAllocation CarAllocation { get; }

    public CarDeallocatedEvent(CarAllocation carAllocation)
    {
        CarAllocation = carAllocation;
    }
}
