using System;
using AssetAllocation.Api.Infrastructure.Services.Token;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api.Features.Account;

public class LoginQuery : IRequest<Result<LoginResponse>>
{
    public int RegistrationNumber { get; set; }
    public string? Password { get; set; }

    public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<LoginResponse>>
    {
        private readonly AssetAllocationDbContext _assetAllocationDbContext;
        private readonly ITokenHelper _tokenHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;

        public LoginQueryHandler(AssetAllocationDbContext assetAllocationDbContext,
        ITokenHelper tokenHelper,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService)
        {
            _assetAllocationDbContext = assetAllocationDbContext;
            _tokenHelper = tokenHelper;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }

        public async Task<Result<LoginResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var person = await _assetAllocationDbContext.Persons
                .Include(p => p.PersonOperationClaims).ThenInclude(p => p.OperationClaim)
                .Include(p => p.RefreshTokens.Where(x => x.RevokedAt == null))
                .FirstOrDefaultAsync(p => p.RegistrationNumber == request.RegistrationNumber, cancellationToken);

            if(person == null)
            {
                return Result<LoginResponse>.BadRequest("Person not found");
            }

            bool isPasswordValid = HashingHelper.VerifyPasswordHash(request.Password!, person.PasswordHash, person.PasswordSalt);

            if(!isPasswordValid)
            {
                return Result<LoginResponse>.Unauthorized("Wrong password");
            }

            var clientIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            var refreshToken = _tokenHelper.GenerateRefreshToken(person: person, ipAddress: clientIp);
            await _assetAllocationDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await _assetAllocationDbContext.SaveChangesAsync(cancellationToken);

            await _tokenService.RemoveOldTokensIfLimitExceeded(person.Id, clientIp);

            var personOperationClaims = person.PersonOperationClaims.Select(x => x.OperationClaim).ToList();
            var accessToken = _tokenHelper.GenerateToken(person, personOperationClaims);

            LoginResponse response = new() { AccessToken = accessToken, RefreshToken = refreshToken.Token }; ;

            return Result<LoginResponse>.Ok(response);
        }
    }
}
