using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AssetAllocation.Api;

public static class SecurityKeyHelper
{
    public static SecurityKey CreateSecurityKey(string securityKey) => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
}
