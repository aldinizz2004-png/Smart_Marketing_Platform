using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Menus, websites, past posts ... that the AI reads to learn the business.
public class KnowledgeSource : BusinessEntity
{
    public Guid? AssetId { get; set; }

    public KnowledgeSourceType SourceType { get; set; }

    public string? SourceUrl { get; set; }

    public string? ExtractedText { get; set; }

    public ProcessingStatus Status { get; set; } = ProcessingStatus.Pending;

    public DateTimeOffset? ProcessedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public MediaAsset? Asset { get; set; }
}
