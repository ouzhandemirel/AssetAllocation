using System.Security.Claims;

namespace AssetAllocation.Api;

public static class ClaimPrincipalExtensions
{
    public static List<string>? Claims(this ClaimsPrincipal claimsPrincipal, string claimType)
    {
        var result = claimsPrincipal?.FindAll(claimType)?.Select(x => x.Value).ToList();
        return result;
    }

    public static List<string>? ClaimRoles(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal?.Claims(ClaimTypes.Role);

    public static Guid GetPersonId(this ClaimsPrincipal claimsPrincipal)
    {
        var nameIdentifierClaim = claimsPrincipal?.Claims(ClaimTypes.NameIdentifier)?.FirstOrDefault();
        return nameIdentifierClaim != null ? new Guid(nameIdentifierClaim) : Guid.Empty;
    }
}
