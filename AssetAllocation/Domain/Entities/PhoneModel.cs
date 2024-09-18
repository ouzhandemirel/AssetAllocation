using System.Data.SqlTypes;

namespace AssetAllocation.Api;

public class PhoneModel : Entity<Guid>
{
    public string Name { get; set; }
    public Guid BrandId { get; set; }

    public PhoneBrand Brand { get; set; } = null!;
    public ICollection<Phone> Phones { get; set; } = null!;

    public PhoneModel(string name, Guid brandId)
    {
        Id = Guid.NewGuid();
        Name = name;
        BrandId = brandId;
    }

    public PhoneModel() : this(string.Empty, Guid.Empty)
    {
        
    }
}
