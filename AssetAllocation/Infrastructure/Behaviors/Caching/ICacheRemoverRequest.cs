namespace AssetAllocation.Api;

public interface ICacheRemoverRequest
{
    public string? CacheKey { get; }
    public string? GroupKey { get; }
    bool BypassCache { get; }
}
