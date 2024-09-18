namespace AssetAllocation.Api;

public class CarModel : Entity<Guid>
{
    public string Name { get; set; }
    public Guid MakeId { get; set; }

    public CarMake Make { get; set; } = null!;
    public ICollection<Car> Cars { get; set; } = null!;

    public CarModel(string name, Guid makeId)
    {
        Id = Guid.NewGuid();
        Name = name;
        MakeId = makeId;
    }

    public CarModel() : this(string.Empty, Guid.Empty) 
    {
    }
}
