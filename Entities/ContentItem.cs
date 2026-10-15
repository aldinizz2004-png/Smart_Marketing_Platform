using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// One post / story / reel / WhatsApp message slot inside a campaign.
public class ContentItem : BusinessEntity
{
    public Guid CampaignId { get; set; }

    public string PlatformCode { get; set; } = string.Empty;

    public ContentType ContentType { get; set; }

    public ContentStatus Status { get; set; } = ContentStatus.Draft;

    public DateTimeOffset? PlannedPublishAt { get; set; }

    // A/B testing: items in one test share the group id.
    public Guid? VariantGroupId { get; set; }

    public string? VariantLabel { get; set; }

    // Set when this item was created by an AI recommendation.
    public Guid? SourceRecommendationId { get; set; }

    // Set when this item was inspired by a competitor / professional post.
    public Guid? InspiredBySourcePostId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Campaign Campaign { get; set; } = null!;

    public Platform Platform { get; set; } = null!;

    public StrategyRecommendation? SourceRecommendation { get; set; }

    public SourcePost? InspiredBySourcePost { get; set; }

    public ICollection<ContentRevision> Revisions { get; set; } = new List<ContentRevision>();
}
