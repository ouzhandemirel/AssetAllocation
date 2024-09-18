namespace AssetAllocation.Api;

public class Entity<T> : IEntityTimestamps
{
    public T Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate  { get; set; }
    public DateTime? DeletedDate  { get; set; }

    public Entity()
    {
        Id = default!;
    }

    public Entity(T id)
    {
        Id = id;
    }
}
