using Microsoft.AspNetCore.Mvc;
using ValidationProblemDetails = AssetAllocation.Api.Infrastructure.Exceptions.ProblemDetailTypes.ValidationProblemDetails;

namespace AssetAllocation.Api;

public abstract class BaseResult
{
    public Failure? ResultFailure { get; init; }
    public bool IsSuccess => ResultFailure == null;
    public int StatusCode { get; init; }

    //Made static to share between derived instances to lower memory usage
    private static Dictionary<int, string> StatusCodeTitles => new() {
        {StatusCodes.Status200OK, "OK"},
        {StatusCodes.Status201Created, "Created"},
        {StatusCodes.Status204NoContent, "No Content"},
        {StatusCodes.Status400BadRequest, "Bad Request"},
        {StatusCodes.Status401Unauthorized, "Unauthorized"},
        {StatusCodes.Status403Forbidden, "Forbidden"},
        {StatusCodes.Status404NotFound, "Not found"}
    };

    protected ProblemDetails SetProblemDetails()
    {
        if (ResultFailure!.IsValidationFailure)
        {
            return new ValidationProblemDetails(ResultFailure.Errors);
        }

        return new()
        {
            Title = StatusCodeTitles[StatusCode] ?? String.Empty,
            Detail = ResultFailure.Errors[0],
            Status = StatusCode,
            Type = "https://example.com/probs/unknown"
        };
    }
}