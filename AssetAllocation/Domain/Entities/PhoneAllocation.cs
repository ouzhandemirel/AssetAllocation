namespace AssetAllocation.Api;

public class PhoneAllocation : Entity<Guid>
{
    public Guid PhoneId { get; set; }
    public Guid PersonId { get; set; }
    public DateTime AllocationDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public Phone Phone { get; set; } = null!;
    public Person Person { get; set; } = null!;

    public PhoneAllocation(Guid phoneId, Guid personId, DateTime allocationDate, DateTime? returnDate = null)
    {
        Id = Guid.NewGuid();
        PhoneId = phoneId;
        PersonId = personId;
        AllocationDate = allocationDate;
        ReturnDate = returnDate;
    }

    public PhoneAllocation() : this(Guid.Empty, Guid.Empty, DateTime.UtcNow)
    {
    }
}
