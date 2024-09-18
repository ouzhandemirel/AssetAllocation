using AssetAllocation.Api.Infrastructure.Services.Token;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api.Features.Account.GetAccessToken;

public class GetAccessTokenQuery : IRequest<Result<GetAccessTokenDto>>
{
    public string RefreshToken { get; set; }

    public GetAccessTokenQuery(string refreshToken)
    {
        RefreshToken = refreshToken;
    }

    public class GetAccessTokenQueryHandler : IRequestHandler<GetAccessTokenQuery, Result<GetAccessTokenDto>>
    {
        private readonly AssetAllocationDbContext _assetAllocationDbContext;
        private readonly ITokenService _tokenService;
        private readonly ITokenHelper _tokenHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAccessTokenQueryHandler(ITokenService tokenService, IHttpContextAccessor httpContextAccessor, AssetAllocationDbContext assetAllocationDbContext, ITokenHelper tokenHelper)
        {
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _assetAllocationDbContext = assetAllocationDbContext;
            _tokenHelper = tokenHelper;
        }

        public async Task<Result<GetAccessTokenDto>> Handle(GetAccessTokenQuery request, CancellationToken cancellationToken)
        {
            var newRefreshToken = await _tokenService.RotateRefreshToken(request.RefreshToken);
            var clientIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            var user = await _assetAllocationDbContext.Persons
                .Include(x => x.PersonOperationClaims).ThenInclude(x => x.OperationClaim)
                .FirstOrDefaultAsync(x => x.Id == newRefreshToken.PersonId);

            if (user == null)
            {
                return Result<GetAccessTokenDto>.BadRequest("User not found");
            }

            var personOperationClaims = user.PersonOperationClaims.Select(x => x.OperationClaim).ToList();
            var accessToken = _tokenHelper.GenerateToken(user, personOperationClaims);

            var response = new GetAccessTokenDto(accessToken, newRefreshToken.Token);

            return Result<GetAccessTokenDto>.Ok(response);
        }
    }
}
