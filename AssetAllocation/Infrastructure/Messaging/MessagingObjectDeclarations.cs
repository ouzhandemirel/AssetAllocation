using System;
using AssetAllocation.Api.Infrastructure.Messaging.Constants;
using RabbitMQ.Client;

namespace AssetAllocation.Api.Infrastructure.Messaging;

public static class MessagingObjectDeclarations
{
    public static void DeclareEmailExchange(IConnection connection)
    {
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: ExchangeNames.EmailExhange,
            type: ExchangeType.Fanout,
            durable: false,
            autoDelete: true);
    }
}
