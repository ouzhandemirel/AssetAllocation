namespace AssetAllocation.Api;

public class PhoneBrand : Entity<Guid>
{
    public string Name { get; set; }
    public string Country { get; set; }
    public ICollection<PhoneModel> Models { get; set; } = null!;

    public PhoneBrand(string name, string country)
    {
        Id = Guid.NewGuid();
        Name = name;
        Country = country;
    }

    public PhoneBrand() : this(string.Empty, string.Empty)
    {
    }
}
