using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace AssetAllocation.Api;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        List<string>? userRoleClaims = _httpContextAccessor.HttpContext?.User.ClaimRoles();

        if (userRoleClaims == null)
        {
            throw new AuthorizationException("You are not authenticated");
        }

        var isNotMatchedAUserRoleClaimWithRequestRoles = string.IsNullOrEmpty(userRoleClaims
            .FirstOrDefault(urc => urc == GeneralOperationClaims.Admin || request.Roles.Any(r => r == urc)));

        if (isNotMatchedAUserRoleClaimWithRequestRoles)
        {
            throw new AuthorizationException("You are not authorized");
        }

        TResponse response = await next();
        return response;
    }
}
