using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// A public post collected from a tracked source, with its engagement numbers
// at capture time. Same attributes as our own revisions so they can be compared.
public class SourcePost : BusinessEntity
{
    public Guid TrackedSourceId { get; set; }

    public string? ExternalPostId { get; set; }

    public string? PostUrl { get; set; }

    public DateTimeOffset? PostedAt { get; set; }

    public ContentFormat Format { get; set; }

    public HookType? HookType { get; set; }

    public string? CaptionText { get; set; }

    public string HashtagsJson { get; set; } = "[]";

    public int? LikesCount { get; set; }

    public int? CommentsCount { get; set; }

    public int? SharesCount { get; set; }

    public long? ViewsCount { get; set; }

    // (likes + comments + shares) / followers at capture time.
    public decimal? EngagementRate { get; set; }

    public DateTimeOffset CapturedAt { get; set; }

    public TrackedSource TrackedSource { get; set; } = null!;
}
