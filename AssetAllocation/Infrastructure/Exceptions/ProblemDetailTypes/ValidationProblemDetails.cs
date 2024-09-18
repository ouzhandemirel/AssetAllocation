using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api.Infrastructure.Exceptions.ProblemDetailTypes;

public class ValidationProblemDetails : ProblemDetails
{
    public string[] Errors { get; init; }

    public ValidationProblemDetails(string[] errors)
    {
        Title = "Validation Error(s)";
        Detail = "One or more validation errors occured";
        Errors = errors;
        Status = StatusCodes.Status400BadRequest;
        Type = "https://example.com/probs/validation";
    }
}
