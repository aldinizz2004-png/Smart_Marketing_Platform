using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Immutable version of a content item's text and creative.
public class ContentRevision : BusinessEntity
{
    public Guid ContentItemId { get; set; }

    public int RevisionNo { get; set; }

    public string Body { get; set; } = string.Empty;

    // Explicit attributes used to compare performance (ours vs competitors).
    public ContentFormat Format { get; set; }

    public HookType? HookType { get; set; }

    public CallToAction? CallToAction { get; set; }

    public bool IncludesOffer { get; set; }

    public Guid? WhatsAppTemplateId { get; set; }

    public string CreativeDetailsJson { get; set; } = "{}";

    // Prices / offers used when generating, frozen.
    public string FactsSnapshotJson { get; set; } = "{}";

    // Voice rules used when generating, frozen.
    public string BrandSnapshotJson { get; set; } = "{}";

    public Guid? GeneratedByExecutionId { get; set; }

    public Guid? SourceRecommendationId { get; set; }

    // NULL when written by the AI.
    public Guid? CreatedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public ContentItem ContentItem { get; set; } = null!;

    public WhatsAppTemplate? WhatsAppTemplate { get; set; }

    public AiExecution? GeneratedByExecution { get; set; }

    public StrategyRecommendation? SourceRecommendation { get; set; }

    public TenantMembership? CreatedByMember { get; set; }

    public ICollection<ContentRevisionAsset> Assets { get; set; } = new List<ContentRevisionAsset>();

    public ICollection<ContentReview> Reviews { get; set; } = new List<ContentReview>();
}
