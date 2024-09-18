using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace AssetAllocation.Api;

public class CacheRemovingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheRemoverRequest
{
    private readonly IDistributedCache _cache;

    public CacheRemovingBehavior(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.BypassCache)
        {
            return await next();
        }

        TResponse response = await next();

        if (request.GroupKey != null)
        {
            byte[]? cachedGroup = await _cache.GetAsync(request.GroupKey, cancellationToken);
            if (cachedGroup != null)
            {
                HashSet<string> keysInGroup = JsonSerializer.Deserialize<HashSet<string>>(Encoding.Default.GetString(cachedGroup))!;
                foreach (string key in keysInGroup)
                {
                    await _cache.RemoveAsync(key, cancellationToken);
                }

                await _cache.RemoveAsync(request.GroupKey, cancellationToken);
                await _cache.RemoveAsync(key: $"{request.GroupKey}SlidingExpiration", cancellationToken);
            }
        }

        if (request.CacheKey != null)
        {
            await _cache.RemoveAsync(request.CacheKey,cancellationToken);
        }
        return response;
    }
}
