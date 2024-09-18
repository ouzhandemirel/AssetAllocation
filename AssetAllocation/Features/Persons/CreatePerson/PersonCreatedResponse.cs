namespace AssetAllocation.Api;

public class PersonCreatedResponse
{
    public Guid Id { get; set; }
    public int RegistrationNumber { get; set; }
    public DateTime CreatedDate { get; set; }
}
