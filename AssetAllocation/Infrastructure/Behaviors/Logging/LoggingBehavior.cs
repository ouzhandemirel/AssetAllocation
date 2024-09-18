using System.Text.Json;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http.Extensions;

namespace AssetAllocation.Api;

public class LoggingBehavior<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ILoggableRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LoggerServiceBase _logger;

    public LoggingBehavior(IHttpContextAccessor httpContextAccessor, LoggerServiceBase logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        List<LogParameter> logParameters =
        [
            new LogParameter{ Type = request.GetType().Name, Value = request}
        ];

        LogDetail logDetail = new()
        {
            PathAndQuery = _httpContextAccessor.HttpContext?.Request.GetEncodedPathAndQuery() ?? string.Empty,
            User = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "?",
            Parameters = logParameters,
        };

        _logger.Information(JsonSerializer.Serialize(logDetail));

        return Task.CompletedTask;
    }
}
