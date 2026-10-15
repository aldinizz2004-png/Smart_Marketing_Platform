using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

public class Offer : BusinessEntity
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal? OfferPrice { get; set; }

    public string? Currency { get; set; }

    public decimal? DiscountPercent { get; set; }

    public DateTimeOffset StartsAt { get; set; }

    public DateTimeOffset EndsAt { get; set; }

    public OfferStatus Status { get; set; } = OfferStatus.Draft;

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<CampaignOffer> CampaignOffers { get; set; } = new List<CampaignOffer>();
}
