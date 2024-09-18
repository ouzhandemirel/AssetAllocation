using System.Reflection;
using System.Threading.RateLimiting;
using AssetAllocation.Api;
using AssetAllocation.Api.Infrastructure.Exceptions.Types;
using AssetAllocation.Api.Infrastructure.Messaging;
using AssetAllocation.Api.Infrastructure.Persistence.Contexts;
using AssetAllocation.Api.Infrastructure.Services.Token;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

//Add exception handler
builder.Services.AddExceptionHandler<HttpExceptionHandler>();

//HttpContextAccessor
builder.Services.AddHttpContextAccessor();

//DbContext
builder.Services.AddDbContext<AssetAllocationDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetSection("ConnectionStrings:asset_allocation_db").Get<string>()));
builder.Services.AddScoped<IAssetAllocationDbContext, AssetAllocationDbContext>();

//Distributed Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "asset_allocation";
});

//Domain Event Service
builder.Services.AddScoped<IDomainEventService, DomainEventService>();

//Mail Service
builder.Services.AddScoped<IMailService, MailService>();

//MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);

    cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
    cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
    cfg.AddOpenBehavior(typeof(CacheRemovingBehavior<,>));

    cfg.AddOpenRequestPostProcessor(typeof(LoggingBehavior<,>));
});

//Logger implementation
//builder.Services.AddSingleton<LoggerServiceBase, FileLogger>();
builder.Services.AddSingleton<LoggerServiceBase, PostgreSqlLogger>();

//Security
builder.Services.AddScoped<ITokenHelper, JwtHelper>();
builder.Services.AddScoped<ITokenService, TokenService>();
//builder.Services.AddScoped<IEmailAuthenticationHelper, EmailAuthenticationHelper>();
builder.Services.AddScoped<IOtpAuthenticationHelper, OtpNetOtpAuthenticationHelper>();

//Validators from the assembly containing the CreateCarCommandValidator
builder.Services.AddValidatorsFromAssemblyContaining<CreateCarCommandValidator>();

//AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory() { HostName = builder.Configuration.GetSection("RabbitMqConfiguration:Host").Get<string>() };
    return factory.CreateConnection();
});

builder.Services.AddSingleton<EmailPublisher>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidIssuer = builder.Configuration["TokenOptions:Issuer"],
            ValidAudience = builder.Configuration["TokenOptions:Audience"],
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(builder.Configuration["TokenOptions:SecurityKey"] ?? throw new ArgumentNullException("Security key is missing in appsettings.json")),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

//builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = (context, _) =>
    {
        Console.WriteLine($"{context.HttpContext.User.Identity?.Name} made too many requests.");
        return new ValueTask();
    };

    options.AddPolicy("fixed", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            //partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            partitionKey: httpContext.User.GetPersonId().ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(1),
                PermitLimit = 100
            }));
});



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler(opt => { });
}

app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var connection = scope.ServiceProvider.GetRequiredService<IConnection>();

    MessagingObjectDeclarations.DeclareEmailExchange(connection);
});

//ip-based ratelimiter
//app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//user-based ratelimiter
app.UseRateLimiter();

app.MapControllers();

app.Run();