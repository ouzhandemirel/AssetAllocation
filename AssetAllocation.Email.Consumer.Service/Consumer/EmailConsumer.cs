using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AssetAllocation.Email.Consumer.Service;

public class EmailConsumer : BackgroundService
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private const string ExchangeName = "email_fanout_exchange";
    private const string QueueName = "email_fanout_queue";
    private readonly IMailService _mailService;

    public EmailConsumer(IConnection connection, IMailService mailService)
    {
        _connection = connection;
        _channel = connection.CreateModel();
        _mailService = mailService;
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

            Console.WriteLine($"Email message consumed.");

            if (email != null)
            {
                await _mailService.SendMailAsync(email, cancellationToken);
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
