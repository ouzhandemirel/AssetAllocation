namespace AssetAllocation.Api;

public interface ICachableRequest
{
    string CacheKey { get; }
    bool BypassCache { get; }
    string? GroupKey { get; }
    TimeSpan? SlidingExpiration { get; }
}
