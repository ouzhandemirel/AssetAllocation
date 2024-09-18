using Microsoft.IdentityModel.Tokens;

namespace AssetAllocation.Api;

public static class SigningCredentialsHelper
{
    public static SigningCredentials CreateSigningCredentials(SecurityKey securityKey) => new(securityKey, SecurityAlgorithms.HmacSha512Signature);
}