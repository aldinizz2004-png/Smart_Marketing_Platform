using SmartMarketing.Domain.Common;

namespace SmartMarketing.Domain.Entities;

public class Product : BusinessEntity, IHasUpdatedAt
{
    public string? Sku { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string Currency { get; set; } = "USD";

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<MediaAsset> MediaAssets { get; set; } = new List<MediaAsset>();

    public ICollection<CampaignProduct> CampaignProducts { get; set; } = new List<CampaignProduct>();
}
