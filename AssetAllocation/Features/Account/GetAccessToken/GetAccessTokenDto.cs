namespace AssetAllocation.Api.Features.Account.GetAccessToken;

public class GetAccessTokenDto
{
    public AccessToken AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public GetAccessTokenDto()
    {
        AccessToken = new();
        RefreshToken = string.Empty;
    }

    public GetAccessTokenDto(AccessToken accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}