using Microsoft.EntityFrameworkCore;

namespace AssetAllocation.Api;

public static class IQueryablePaginatingExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> source,
        int index,
        int size,
        CancellationToken cancellationToken = default) where T : class
    {
        int count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
        List<T> items = await source.Skip(index * size).Take(size).ToListAsync(cancellationToken).ConfigureAwait(false);

        return new PaginatedList<T>
        {
            Index = index,
            Size = size,
            Count = count,
            Pages = (int)Math.Ceiling(count / (double)size),
            Items = items
        };
    }
}
