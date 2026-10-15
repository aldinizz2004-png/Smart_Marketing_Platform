namespace SmartMarketing.Domain.Entities;

// Join entity: Campaign <-> Offer
public class CampaignOffer
{
    public Guid BusinessId { get; set; }

    public Guid CampaignId { get; set; }

    public Guid OfferId { get; set; }

    public Guid TenantId { get; set; }

    public Campaign Campaign { get; set; } = null!;

    public Offer Offer { get; set; } = null!;
}
