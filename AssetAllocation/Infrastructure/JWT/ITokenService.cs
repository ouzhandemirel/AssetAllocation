namespace AssetAllocation.Api.Infrastructure.Services.Token;

public interface ITokenService
{
    Task<RefreshToken> RotateRefreshToken(string usedToken);
    Task RemoveOldTokensIfLimitExceeded(Guid personId, string? revokedByIp = null);
}
