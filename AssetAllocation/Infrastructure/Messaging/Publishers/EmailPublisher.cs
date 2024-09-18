using System.Net.Mail;
using System.Text;
using System.Text.Json;
using AssetAllocation.Api.Infrastructure.Mail;
using AssetAllocation.Api.Infrastructure.Messaging;
using AssetAllocation.Api.Infrastructure.Messaging.Constants;
using RabbitMQ.Client;

namespace AssetAllocation.Api;

public class EmailPublisher
{
    private readonly IModel _channel;
    private const string ExchangeName = ExchangeNames.EmailExhange;
    private const string RoutingKey = RoutingKeys.EmailRoutingKey;

    public EmailPublisher(IConnection connection)
    {
        _channel = connection.CreateModel();
    }

    public void PublishEmail(EmailMessage email)
    {
        byte[]? serializedMessage = JsonSerializer.SerializeToUtf8Bytes(email);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: RoutingKey,
            basicProperties: properties,
            body: serializedMessage
        );
    }
}
