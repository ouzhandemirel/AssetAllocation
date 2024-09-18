namespace AssetAllocation.Api;

public class PaginatedList<T>
{
    public int Index { get; set; }
    public int Size { get; set; }
    public int Count { get; set; }
    public int Pages { get; set; }
    public IList<T> Items { get; set; }
    public bool HasPrevious => Index > 0;
    public bool HasNext => Index + 1 < Pages;
    
    public PaginatedList()
    {
        Items = Array.Empty<T>();
    }
}
