namespace AssetAllocation.Api;

public class RefreshToken : Entity<Guid>
{
    public Guid PersonId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime AbsoluteExpiration { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }

    public Person Person { get; set; } = null!;

    public RefreshToken()
    {
        Token = string.Empty;
        CreatedByIp = string.Empty;
    }

    public RefreshToken(Guid personId, string token, DateTime expiresAt, string createdByIp)
    {
        Id = Guid.NewGuid();
        PersonId = personId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedByIp = createdByIp;
    }

    public RefreshToken(Guid id, Guid personId, string token, DateTime expiresAt, string createdByIp) : base(id)
    {
        PersonId = personId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedByIp = createdByIp;
    }

    public void Revoke(string reason, string? revokedByIp = null, string? replacedByToken = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReasonRevoked = reason;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
    }
}
