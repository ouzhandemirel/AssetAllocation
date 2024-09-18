
using AssetAllocation.Api.Infrastructure.Exceptions.ProblemDetailTypes;

namespace AssetAllocation.Api;

public class ExceptionTypeResolver
{
    private HttpResponse? _response;
    public HttpResponse Response
    {
        get => _response ?? throw new ArgumentNullException(nameof(_response));
        set => _response = value;
    }

    public Task HandleExceptionAsync(Exception exception) =>
        exception switch
        {
            ValidationException validationException => HandleException(validationException),
            InternalServerErrorException internalServerErrorException => HandleException(internalServerErrorException),
            AuthorizationException authorizationException => HandleException(authorizationException),
            _ => HandleException(exception)
        };
    
    private Task HandleException(ValidationException exception)
    {
        Response.StatusCode = StatusCodes.Status400BadRequest;
        var details = new ValidationProblemDetails(exception.Errors);
        return Response.WriteAsJsonAsync(details);
    }

    private Task HandleException(InternalServerErrorException exception)
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        var details = new InternalServerErrorProblemDetails(exception.Message);
        return Response.WriteAsJsonAsync(details);
    }

    private Task HandleException(AuthorizationException exception)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        var details = new AuthorizationProblemDetails(exception.Message);
        return Response.WriteAsJsonAsync(details);
    }

    private Task HandleException(Exception exception)
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        var details = new InternalServerErrorProblemDetails(exception.Message);
        return Response.WriteAsJsonAsync(details);
    }
}
