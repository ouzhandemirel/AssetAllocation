namespace AssetAllocation.Api;

public class TokenOptions
{
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int AccessTokenExpiration { get; set; }
    public string SecurityKey { get; set; }
    public int RefreshTokenExpiration { get; set; }
    public int RefreshTokenAbsoluteExpiration { get; set; }

    public TokenOptions()
    {
        Audience = string.Empty;
        Issuer = string.Empty;
        SecurityKey = string.Empty;
    }

    public TokenOptions(
        string audience, string issuer, int accessTokenExpiration, 
        string securityKey, int refreshTokenExpiration, int refreshTokenAbsoluteExpiration)
    {
        Audience = audience;
        Issuer = issuer;
        AccessTokenExpiration = accessTokenExpiration;
        SecurityKey = securityKey;
        RefreshTokenExpiration = refreshTokenExpiration;
        RefreshTokenAbsoluteExpiration = refreshTokenAbsoluteExpiration;
    }
}