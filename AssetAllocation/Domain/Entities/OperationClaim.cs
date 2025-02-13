﻿namespace AssetAllocation.Api;

public class OperationClaim : Entity<int>
{
    public string Name { get; set; }

    public ICollection<PersonOperationClaim> PersonOperationClaims { get; set; } = null!;

    public OperationClaim()
    {
        Name = string.Empty;
    }

    public OperationClaim(string name)
    {
        Name = name;
    }

    public OperationClaim(int id, string name) : base(id)
    {
        Name = name;
    }
}
