namespace AssetAllocation.Api;

public class PersonOperationClaim : Entity<int>
{
    public Guid PersonId { get; set; }
    public int OperationClaimId { get; set; }

    public Person Person { get; set; } = null!;
    public OperationClaim OperationClaim { get; set; } = null!;

    public PersonOperationClaim(Guid personId, int operationClaimId)
    {
        PersonId = personId;
        OperationClaimId = operationClaimId;
    }

    public PersonOperationClaim(int id, Guid personId, int operationClaimId) : base(id)
    {
        PersonId = personId;
        OperationClaimId = operationClaimId;
    }
}
