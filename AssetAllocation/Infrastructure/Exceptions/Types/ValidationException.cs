namespace AssetAllocation.Api;

public class ValidationException(string[] errors) : Exception
{
    public string[] Errors { get; } = errors;
}
