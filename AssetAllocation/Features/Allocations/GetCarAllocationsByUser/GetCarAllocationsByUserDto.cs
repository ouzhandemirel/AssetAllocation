namespace AssetAllocation.Api;

public class GetCarAllocationsByUserDto
{
    public Guid Id { get; set; }
    public string CarMake { get; set; } = null!;
    public string CarModel { get; set; } = null!;
    public int CarYear { get; set; }
    public string CarColor { get; set; } = null!;
    public string Plate { get; set; } = null!;
    public DateTime AllocationDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}

