namespace SmartMarketing.Domain.Entities;

// Join entity: Campaign <-> Product
public class CampaignProduct
{
    public Guid BusinessId { get; set; }

    public Guid CampaignId { get; set; }

    public Guid ProductId { get; set; }

    public Guid TenantId { get; set; }

    public Campaign Campaign { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
