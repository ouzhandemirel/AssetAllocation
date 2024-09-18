namespace AssetAllocation.Api;

public class InternalServerErrorException(string message) : Exception(message)
{
}
