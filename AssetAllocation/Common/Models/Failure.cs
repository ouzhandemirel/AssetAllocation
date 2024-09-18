namespace AssetAllocation.Api;

public class Failure
{
    public string[] Errors { get; set; } = null!;
    public bool IsValidationFailure { get; set; }
}
