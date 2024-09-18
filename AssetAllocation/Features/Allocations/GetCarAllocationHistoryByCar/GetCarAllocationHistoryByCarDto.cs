namespace AssetAllocation.Api;

public class GetCarAllocationHistoryByCarDto
{
    public Guid AllocationId { get; set; }
    public string PersonName { get; set; } = null!;
    public string PersonSurname { get; set; } = null!;
    public int RegistrationNumber { get; set; }
    public string Department { get; set; } = null!;
    public string Title { get; set; } = null!;
    public DateTime AllocationDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}
