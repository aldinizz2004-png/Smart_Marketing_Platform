using SmartMarketing.Domain.Common;

namespace SmartMarketing.Domain.Entities;

public class AudienceSegment : BusinessEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string TargetingRulesJson { get; set; } = "{}";

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<CampaignAudience> CampaignAudiences { get; set; } = new List<CampaignAudience>();
}
