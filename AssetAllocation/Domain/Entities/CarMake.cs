namespace AssetAllocation.Api;

public class CarMake : Entity<Guid>
{
    public string Name { get; set; }
    public string Country { get; set; }

    public ICollection<CarModel> Models { get; set; } = null!;

    public CarMake(string name, string country)
    {
        Id = Guid.NewGuid();
        Name = name;
        Country = country;
    }

     public CarMake() : this(string.Empty, string.Empty)
    {      
    }
}