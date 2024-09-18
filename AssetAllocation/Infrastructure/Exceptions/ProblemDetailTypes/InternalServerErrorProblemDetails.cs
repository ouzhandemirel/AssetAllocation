using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api.Infrastructure.Exceptions.ProblemDetailTypes;

public class InternalServerErrorProblemDetails : ProblemDetails
{
    public InternalServerErrorProblemDetails(string detail)
    {
        Title = "Internal Server Error";
        Detail = detail;
        Status = StatusCodes.Status500InternalServerError;
        Type = "https://example.com/probs/internal";
    }
}
