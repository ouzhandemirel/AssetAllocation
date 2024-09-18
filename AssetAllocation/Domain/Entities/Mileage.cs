namespace AssetAllocation.Api;

public class Mileage : Entity<Guid>
{
    public Guid CarId { get; set; }
    public int Counter { get; set; }
    public DateTime Date { get; set; }

    public Car Car { get; set; } = null!;

    public Mileage(Guid carId, int counter, DateTime date)
    {
        Id = Guid.NewGuid();
        CarId = carId;
        Counter = counter;
        Date = date;
    }

    public Mileage() : this(Guid.Empty, 0, DateTime.UtcNow) {}
}
