namespace AssetAllocation.Api;

public interface ISecuredRequest
{
    public string[] Roles { get; }
}
