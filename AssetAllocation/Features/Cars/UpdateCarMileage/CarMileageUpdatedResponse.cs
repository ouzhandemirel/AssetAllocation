namespace AssetAllocation.Api;

public class CarMileageUpdatedResponse
{
    public Guid Id { get; set; }
    public int CurrentMileage { get; set; }
    public DateTime UpdatedDate { get; set; }
}
