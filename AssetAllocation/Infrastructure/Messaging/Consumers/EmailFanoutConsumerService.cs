using System;
using System.Text;
using System.Text.Json;
using AssetAllocation.Api.Infrastructure.Mail;
using AssetAllocation.Api.Infrastructure.Messaging.Constants;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AssetAllocation.Api.Infrastructure.Messaging.Consumers;

public class EmailFanoutConsumerService : BackgroundService
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private const string ExchangeName = ExchangeNames.EmailExhange;
    private const string QueueName = "email_fanout_queue";
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EmailFanoutConsumerService(IConnection connection, IServiceScopeFactory serviceScopeFactory)
    {
        _connection = connection;
        _channel = connection.CreateModel();

        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _channel.QueueDeclare(
            queue: QueueName,
            durable: false,
            exclusive: true,
            autoDelete: true);

        _channel.QueueBind(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: "");
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var serializedMessage = Encoding.UTF8.GetString(body);
            var email = JsonSerializer.Deserialize<EmailMessage>(serializedMessage);

            Console.WriteLine($"Email message consumed by {QueueName}");

            if (email != null)
            {
                using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
                var mailService = serviceScope.ServiceProvider.GetRequiredService<IMailService>();

                await mailService.SendMailAsync(email, cancellationToken);
            }
        };

        _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
