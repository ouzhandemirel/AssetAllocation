using System;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;

namespace AssetAllocation.Api.Infrastructure.Exceptions.Types;

public class HttpExceptionHandler : IExceptionHandler
{
    private readonly LoggerServiceBase _loggerService;
    private readonly ExceptionTypeResolver _resolver;

    public HttpExceptionHandler(LoggerServiceBase loggerService)
    {
        _loggerService = loggerService;
        _resolver = new ExceptionTypeResolver();
    }

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        LogException(httpContext, exception);

        _resolver.Response = httpContext.Response;
        _resolver.HandleExceptionAsync(exception);

        return ValueTask.FromResult(true);
    }

    private Task LogException(HttpContext httpContext, Exception ex)
    {
        List<LogParameter> logParameters = [new LogParameter() { Type = httpContext.GetType().Name, Value = ex.ToString() }];
        LogDetailWithException logDetail = new()
        {
            PathAndQuery = httpContext?.Request.GetEncodedPathAndQuery() ?? string.Empty,
            User = httpContext?.User.Identity?.Name ?? "?",
            Parameters = logParameters,
            ExceptionMessage = ex.InnerException?.Message
        };

        _loggerService.Error(JsonSerializer.Serialize(logDetail));

        return Task.CompletedTask;
    }
}
