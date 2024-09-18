using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace AssetAllocation.Api;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICachableRequest
{
    private readonly CacheSettings _cacheSettings;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(
        IDistributedCache cache,
        IConfiguration configuration,
        ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>() ?? throw new InvalidOperationException(nameof(CacheSettings));
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.BypassCache)
        {
            return await next();
        }

        TResponse response;
        byte[]? cachedResponse = await _cache.GetAsync(request.CacheKey, cancellationToken);

        if (cachedResponse != null)
        {
            var decodedString = Encoding.Default.GetString(cachedResponse);
            response = JsonSerializer.Deserialize<TResponse>(decodedString)!;
            _logger.LogInformation($"Cache hit for {request.CacheKey}");

            if (request.GroupKey != null)
            {
                TimeSpan slidingExpiration = request.SlidingExpiration ?? TimeSpan.FromMinutes(_cacheSettings.SlidingExpiration);
                await AddCacheKeyToGroupAndRefreshExpiration(request, slidingExpiration, cancellationToken);
            }
        }
        else
        {
            response = await GetResponseAndCache(request, next, cancellationToken);
        }

        return response;
    }

    private async Task<TResponse> GetResponseAndCache(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        TResponse response = await next();

        byte[] serializedData = JsonSerializer.SerializeToUtf8Bytes(response);
        TimeSpan slidingExpiration = request.SlidingExpiration ?? TimeSpan.FromMinutes(_cacheSettings.SlidingExpiration);
        DistributedCacheEntryOptions cacheOptions = new()
        {
            SlidingExpiration = slidingExpiration
        };

        await _cache.SetAsync(request.CacheKey, serializedData, cacheOptions, cancellationToken);
        _logger.LogInformation($"Cache miss for {request.CacheKey}, cached for {slidingExpiration} minutes");

        if (request.GroupKey != null)
        {
            await AddCacheKeyToGroupAndRefreshExpiration(request, slidingExpiration, cancellationToken);
        }

        return response;
    }

    private async Task AddCacheKeyToGroupAndRefreshExpiration(TRequest request, TimeSpan slidingExpiration, CancellationToken cancellationToken)
    {
        byte[]? cacheGroupCache = await _cache.GetAsync(key: request.GroupKey!, cancellationToken);
        HashSet<string> cacheKeysInGroup;

        if (cacheGroupCache != null)
        {
            cacheKeysInGroup = JsonSerializer.Deserialize<HashSet<string>>(Encoding.Default.GetString(cacheGroupCache))!;

            if (!cacheKeysInGroup.Contains(request.CacheKey!))
            {
                cacheKeysInGroup.Add(request.CacheKey!);
            }
        }
        else
        {
            cacheKeysInGroup = new HashSet<string>([request.CacheKey!]);
        }

        byte[] newCacheGroupCache = JsonSerializer.SerializeToUtf8Bytes(cacheKeysInGroup);

        byte[]? cacheGroupCacheSlidingExpirationCache = await _cache.GetAsync(
            key: $"{request.GroupKey}SlidingExpiration",
            cancellationToken);

        int? cacheGroupCacheSlidingExpirationValue = null;

        if (cacheGroupCacheSlidingExpirationCache != null)
        {
            cacheGroupCacheSlidingExpirationValue = Convert.ToInt32(Encoding.Default.GetString(cacheGroupCacheSlidingExpirationCache));
        }

        if (cacheGroupCacheSlidingExpirationValue == null || slidingExpiration.TotalSeconds > cacheGroupCacheSlidingExpirationValue)
        {
            cacheGroupCacheSlidingExpirationValue = Convert.ToInt32(slidingExpiration.TotalSeconds);
        }

        byte[] serializeCachedGroupSlidingExpirationData = JsonSerializer.SerializeToUtf8Bytes(cacheGroupCacheSlidingExpirationValue);

        DistributedCacheEntryOptions cacheOptions =
            new() { SlidingExpiration = TimeSpan.FromSeconds(Convert.ToDouble(cacheGroupCacheSlidingExpirationValue)) };

        await _cache.SetAsync(key: request.GroupKey!, newCacheGroupCache, cacheOptions, cancellationToken);
        _logger.LogInformation($"Added to Cache -> {request.GroupKey}");

        await _cache.SetAsync(
            key: $"{request.GroupKey}SlidingExpiration",
            serializeCachedGroupSlidingExpirationData,
            cacheOptions,
            cancellationToken
        );
        _logger.LogInformation($"Added to Cache -> {request.GroupKey}SlidingExpiration");
    }
}