namespace AssetAllocation.Api;

public class CarAllocatedResponse
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public Guid PersonId { get; set; }
    public DateTime CreatedDate { get; set; }
}
