using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Any URL the owner pastes: a competitor page, a professional account in the
// same field, or an inspiration source. The platform is detected from the
// URL host through PlatformDomain ("website" when the host is unknown).
public class TrackedSource : BusinessEntity
{
    // URL exactly as the owner pasted it.
    public string Url { get; set; } = string.Empty;

    // Lower-cased, without tracking parameters - used for uniqueness.
    public string NormalizedUrl { get; set; } = string.Empty;

    public string Host { get; set; } = string.Empty;

    public string PlatformCode { get; set; } = string.Empty;

    public TrackedSourceKind Kind { get; set; }

    public string? DisplayName { get; set; }

    public string? Handle { get; set; }

    public string? Notes { get; set; }

    public TrackingStatus Status { get; set; } = TrackingStatus.Pending;

    public DateTimeOffset? LastScannedAt { get; set; }

    public Guid AddedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Platform Platform { get; set; } = null!;

    public TenantMembership AddedByMember { get; set; } = null!;

    public ICollection<SourcePost> Posts { get; set; } = new List<SourcePost>();

    public ICollection<CompetitorInsight> Insights { get; set; } = new List<CompetitorInsight>();
}
