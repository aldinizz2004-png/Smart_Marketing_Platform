namespace SmartMarketing.Domain.Entities;

// Join entity: Campaign <-> AudienceSegment
public class CampaignAudience
{
    public Guid BusinessId { get; set; }

    public Guid CampaignId { get; set; }

    public Guid AudienceSegmentId { get; set; }

    public Guid TenantId { get; set; }

    public Campaign Campaign { get; set; } = null!;

    public AudienceSegment AudienceSegment { get; set; } = null!;
}
