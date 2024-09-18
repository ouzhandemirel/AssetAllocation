namespace AssetAllocation.Api;

public class GetCarByIdResponse
{
    public Guid Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public int CurrentMileage { get; set; }
}
