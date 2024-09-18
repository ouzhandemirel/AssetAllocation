using Microsoft.AspNetCore.Mvc;

namespace AssetAllocation.Api.Infrastructure.Exceptions.ProblemDetailTypes;

public class AuthorizationProblemDetails : ProblemDetails
{
    public AuthorizationProblemDetails(string detail)
    {
        Title = "Authorization Error";
        Detail = detail;
        Status = StatusCodes.Status401Unauthorized;
        Type = "https://example.com/probs/auth";
    }
}
