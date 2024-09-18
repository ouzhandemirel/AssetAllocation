using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace AssetAllocation.Api;

public class JwtHelper : ITokenHelper
{
    private readonly TokenOptions _tokenOptions;
    private DateTime _accessTokenExpiration;

    public JwtHelper(IConfiguration configuration)
    {
        const string configurationSection = "TokenOptions";
        _tokenOptions = configuration.GetSection(configurationSection).Get<TokenOptions>() 
            ?? throw new NullReferenceException($"{configurationSection}\" section cannot be found in configuration");
    }

    public AccessToken GenerateToken(Person person, IList<OperationClaim> operationClaims)
    {
        _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
        SecurityKey securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
        SigningCredentials signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
        JwtSecurityToken jwt = CreateJwtSecurityToken(_tokenOptions, person, signingCredentials, operationClaims);
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        string? token = jwtSecurityTokenHandler.WriteToken(jwt);

        return new AccessToken() { Token = token, Expiration = _accessTokenExpiration };
    }

    public RefreshToken GenerateRefreshToken(Person person, DateTime? absoluteExpiration = null, string? ipAddress = null)
    {
        RefreshToken refreshToken = new()
        {
            Id = Guid.NewGuid(),
            PersonId = person.Id,
            Token = RandomRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenExpiration),
            AbsoluteExpiration = absoluteExpiration ?? DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenAbsoluteExpiration),
            CreatedDate = DateTime.UtcNow,
            CreatedByIp = ipAddress ?? string.Empty
        };

        return refreshToken;
    }

    public JwtSecurityToken CreateJwtSecurityToken(
        TokenOptions tokenOptions,
        Person person,
        SigningCredentials signingCredentials,
        IList<OperationClaim> operationClaims)
    {
        JwtSecurityToken jwt = new
        (
            issuer: tokenOptions.Issuer,
            audience: tokenOptions.Audience,
            claims: SetClaims(person, operationClaims),
            notBefore: DateTime.Now,
            expires: _accessTokenExpiration,
            signingCredentials: signingCredentials
        );

        return jwt;
    }

    private static List<Claim> SetClaims(Person person, IList<OperationClaim> operationClaims)
    {
        List<Claim> claims = [];
        claims.AddNameIdentifier(person.Id.ToString());
        claims.AddEmail(person.Email);
        claims.AddName($"{person.Name} {person.Surname}");
        claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());

        return claims;
    }

    private static string RandomRefreshToken()
    {
        byte[] numberByte = new byte[32];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(numberByte);
        return Convert.ToBase64String(numberByte);
    }
}
