using MediatR;

namespace AssetAllocation.Api;

public class DomainEventNotification<T> : INotification where T : DomainEvent
{
    public T DomainEvent { get; }

    public DomainEventNotification(T domainEvent)
    {
        DomainEvent = domainEvent;
    }
}
