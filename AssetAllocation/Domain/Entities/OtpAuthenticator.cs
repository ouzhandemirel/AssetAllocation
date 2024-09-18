namespace AssetAllocation.Api;

public class OtpAuthenticator : Entity<Guid>
{
    public Guid PersonId { get; set; }
    public byte[] SecretKey { get; set; }
    public bool IsVerified { get; set; }

    public Person Person { get; set; } = null!;

    public OtpAuthenticator()
    {
        SecretKey = [];
    }

    public OtpAuthenticator(Guid personId, byte[] secretKey, bool isVerified)
    {
        PersonId = personId;
        SecretKey = secretKey;
        IsVerified = isVerified;
    }

    public OtpAuthenticator(Guid id, Guid personId, byte[] secretKey, bool isVerified) : base(id)
    {
        PersonId = personId;
        SecretKey = secretKey;
        IsVerified = isVerified;
    }
}
