namespace AssetAllocation.Api;

public class DynamicQuery
{
    public Filter? Filter { get; set; }
    public IEnumerable<Sort>? Sort { get; set; }

    public DynamicQuery()
    {
    }

    public DynamicQuery(Filter filter, IEnumerable<Sort> sort)
    {
        Filter = filter;
        Sort = sort;
    }
}
