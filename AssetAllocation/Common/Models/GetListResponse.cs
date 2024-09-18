namespace AssetAllocation.Api;

public class GetListResponse<T> : BasePageable
{
    private List<T>? _items;

    public List<T> Items
    {
        get => _items ??= [];
        set => _items = value;
    }
}
