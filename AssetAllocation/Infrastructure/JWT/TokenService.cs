using System;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api.Infrastructure.Services.Token;

public class TokenService : ITokenService
{
    private const int _maxActiveRefreshTokens = 5;
    private readonly AssetAllocationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenHelper _tokenHelper;

    public TokenService(AssetAllocationDbContext context, IHttpContextAccessor httpContextAccessor, ITokenHelper tokenHelper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _tokenHelper = tokenHelper;
    }

    public async Task<RefreshToken> RotateRefreshToken(string usedToken)
    {
        var token = await _context.RefreshTokens
            .Include(x => x.Person)
            .FirstOrDefaultAsync(x => x.Token == usedToken);

        if (token == null)
        {
            throw new AuthorizationException("Invalid token");
        }

        var clientIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        if (token.RevokedAt != null)
        {
            var activeRefreshTokens = await _context.RefreshTokens.Where(x => x.PersonId == token.PersonId && x.RevokedAt == null).ToListAsync();
            activeRefreshTokens.ForEach(x => x.Revoke("Token revoked due to the use of a previously revoked token", clientIp));
            _context.RefreshTokens.UpdateRange(activeRefreshTokens);
            await _context.SaveChangesAsync();

            throw new AuthorizationException("Invalid token");
        }

        if(token.ExpiresAt < DateTime.UtcNow)
        {
            token.Revoke("Token revoked due to inactivity", clientIp);
            _context.RefreshTokens.UpdateRange(token);
            await _context.SaveChangesAsync();

            throw new AuthorizationException("Invalid token");
        }

        if(token.AbsoluteExpiration < DateTime.UtcNow)
        {
            token.Revoke("Token expired", clientIp);
            _context.RefreshTokens.UpdateRange(token);
            await _context.SaveChangesAsync();

            throw new AuthorizationException("Invalid token");
        }
        
        var newRefreshToken = _tokenHelper.GenerateRefreshToken(token.Person, token.AbsoluteExpiration, clientIp);
        await _context.RefreshTokens.AddAsync(newRefreshToken);

        token.Revoke("Token is used and revoked due to token rotation", clientIp, newRefreshToken.Token);
        _context.RefreshTokens.Update(token);

        await RemoveOldTokensIfLimitExceeded(token.Person.Id, clientIp);

        await _context.SaveChangesAsync();

        return newRefreshToken;
    }

    public async Task RemoveOldTokensIfLimitExceeded(Guid userId, string? revokedByIp = null)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(x => x.PersonId == userId && x.RevokedAt == null)
            .OrderBy(t => t.CreatedDate)
            .ToListAsync();

        if (activeTokens.Count > _maxActiveRefreshTokens)
        {
            var tokensToRemove = activeTokens.Take(activeTokens.Count - _maxActiveRefreshTokens);
            foreach (var token in tokensToRemove)
            {
                token.Revoke("Token revoked because the maximum number of active refresh tokens limit exceeded", revokedByIp);
            }
            _context.RefreshTokens.UpdateRange(tokensToRemove);
            await _context.SaveChangesAsync();
        }
    }
}
