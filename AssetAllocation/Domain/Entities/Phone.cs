using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AssetAllocation.Api;

public class Phone : Entity<Guid>
{
    public Guid ModelId { get; set; }
    public int Memory { get; set; }
    public int Storage { get; set; }
    public string Color { get; set; }

    public PhoneModel Model { get; set; } = null!;
    public ICollection<PhoneAllocation> Allocations { get; set; } = null!;

    public Phone(Guid modelId, int memory, int storage, string color)
    {
        Id = Guid.NewGuid();
        ModelId = modelId;
        Memory = memory;
        Storage = storage;
        Color = color;
    }

    public Phone() : this(Guid.Empty, 0, 0, String.Empty)
    {
    }
}
