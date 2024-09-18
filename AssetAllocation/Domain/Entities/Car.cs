namespace AssetAllocation.Api;

public class Car : Entity<Guid>
{
    public Guid ModelId { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public string Plate { get; set; }
    public int CurrentMileage { get; set; }

    public CarModel Model { get; set; } = null!;
    public ICollection<Mileage> Mileages { get; set; } = null!;
    public ICollection<CarAllocation> Allocations { get; set; } = null!;

    public Car(int year, string color, string plate, int mileage, Guid modelId)
    {
        Id = Guid.NewGuid();
        ModelId = modelId;
        Year = year;
        Color = color;
        Plate = plate;
        CurrentMileage = mileage;
    }

    public Car() : this(0, string.Empty, string.Empty, 0, Guid.Empty) 
    {
    }

    public void SetMileage(int mileage)
    {
        CurrentMileage = mileage;
        Mileages.Add(new Mileage(Id, mileage, DateTime.UtcNow));    
    }
}