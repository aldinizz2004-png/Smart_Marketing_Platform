using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Human decision on one exact revision.
public class ContentReview : BusinessEntity
{
    public Guid ContentItemId { get; set; }

    public Guid RevisionId { get; set; }

    public Guid ReviewerId { get; set; }

    public ReviewDecision Decision { get; set; }

    public string? Feedback { get; set; }

    public DateTimeOffset DecidedAt { get; set; }

    public ContentRevision Revision { get; set; } = null!;

    public TenantMembership Reviewer { get; set; } = null!;
}
