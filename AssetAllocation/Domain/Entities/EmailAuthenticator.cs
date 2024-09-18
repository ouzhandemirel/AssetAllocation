namespace AssetAllocation.Api;

public class EmailAuthenticator : Entity<Guid>
{
    public Guid PersonId { get; set; }
    public string? ActivationKey { get; set; }
    public bool IsVerified { get; set; }

    public Person Person { get; set; } = null!;

    public EmailAuthenticator() { }

    public EmailAuthenticator(Guid personId, bool isVerified)
    {
        PersonId = personId;
        IsVerified = isVerified;
    }

    public EmailAuthenticator(Guid id, Guid personId, bool isVerified) : base(id)
    {
        PersonId = personId;
        IsVerified = isVerified;
    }

}
