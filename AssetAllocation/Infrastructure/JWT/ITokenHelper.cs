namespace AssetAllocation.Api;

public interface ITokenHelper
{
    AccessToken GenerateToken(Person person, IList<OperationClaim> operationClaims);
    RefreshToken GenerateRefreshToken(Person person, DateTime? absoluteExpiration = null, string? ipAddress = null);
}
