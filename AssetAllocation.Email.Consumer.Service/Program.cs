using AssetAllocation.Email.Consumer.Service;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory() { HostName = builder.Configuration.GetSection("RabbitMqConfiguration:Host").Get<string>() };
    return factory.CreateConnection();
});

builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddHostedService<EmailConsumer>();

var host = builder.Build();
host.Run();