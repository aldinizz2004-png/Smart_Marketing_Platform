using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Scheduled publishing work. PlatformCode must match BOTH the content item
// and the social account (enforced by composite foreign keys).
public class PublicationJob : BusinessEntity
{
    public Guid ContentItemId { get; set; }

    public Guid RevisionId { get; set; }

    public Guid SocialAccountId { get; set; }

    public string PlatformCode { get; set; } = string.Empty;

    public string IdempotencyKey { get; set; } = string.Empty;

    public DateTimeOffset ScheduledAt { get; set; }

    public Guid? SourceRecommendationId { get; set; }

    public JobStatus Status { get; set; } = JobStatus.Queued;

    public int AttemptCount { get; set; }

    public string? LastError { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public ApprovedContentRevision Approval { get; set; } = null!;

    public ContentItem ContentItem { get; set; } = null!;

    public SocialAccount SocialAccount { get; set; } = null!;

    public StrategyRecommendation? SourceRecommendation { get; set; }

    public Publication? Publication { get; set; }

    public ICollection<MessageDelivery> Deliveries { get; set; } = new List<MessageDelivery>();
}
