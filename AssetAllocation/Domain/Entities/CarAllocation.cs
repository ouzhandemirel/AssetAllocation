
namespace AssetAllocation.Api;

public class CarAllocation : Entity<Guid>, IHasDomainEvent
{
    public Guid CarId { get; set; }
    public Guid PersonId { get; set; }
    public DateTime AllocationDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public Car Car { get; set; } = null!;
    public Person Person { get; set; } = null!;
    public List<DomainEvent> DomainEvents { get; } = [];

    public CarAllocation(Guid carId, Guid personId, DateTime allocationDate, DateTime? returnDate = null)
    {
        Id = Guid.NewGuid();
        CarId = carId;
        PersonId = personId;
        AllocationDate = allocationDate;
        ReturnDate = returnDate;
    }

    public CarAllocation() : this(Guid.Empty, Guid.Empty, DateTime.UtcNow)
    {
        
    }
}
