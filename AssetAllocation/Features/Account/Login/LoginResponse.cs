namespace AssetAllocation.Api.Features.Account;

public class LoginResponse
{
    public AccessToken AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
