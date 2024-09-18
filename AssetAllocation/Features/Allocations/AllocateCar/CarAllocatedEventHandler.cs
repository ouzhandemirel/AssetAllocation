using AssetAllocation.Api.Infrastructure.Mail;
using MediatR;

namespace AssetAllocation.Api;

public sealed class CarAllocatedEventHandler : INotificationHandler<DomainEventNotification<CarAllocatedEvent>>
{
    private readonly IMailService _mailService;
    private readonly EmailPublisher _emailPublisher;
    private readonly ILogger<CarAllocatedEventHandler> _logger;

    public CarAllocatedEventHandler(ILogger<CarAllocatedEventHandler> logger,
    IMailService mailService,
    EmailPublisher emailPublisher)
    {
        _logger = logger;
        _mailService = mailService;
        _emailPublisher = emailPublisher;
    }

    public Task Handle(DomainEventNotification<CarAllocatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        //EmailMessage emailMessage = new(domainEvent.CarAllocation.Person.EmailAddress);
        
        EmailMessage emailMessage = new(notification.DomainEvent.CarAllocation.Person.Email, "Allocation Alert", "A car allocated to you", false);
        _emailPublisher.PublishEmail(emailMessage);

        _logger.LogInformation(
            "Asset car {CarId} allocated to person {personId} with id {id}", 
            [domainEvent.CarAllocation.CarId, domainEvent.CarAllocation.PersonId, domainEvent.CarAllocation.Id]);

        return Task.CompletedTask;
    }
}
