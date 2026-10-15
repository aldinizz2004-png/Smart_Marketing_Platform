using SmartMarketing.Domain.Common;

namespace SmartMarketing.Domain.Entities;

// Versioned AI marketing plan for a campaign.
public class CampaignPlan : BusinessEntity
{
    public Guid CampaignId { get; set; }

    public int VersionNo { get; set; }

    public string PlanJson { get; set; } = "{}";

    public Guid? GeneratedByExecutionId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Campaign Campaign { get; set; } = null!;

    public AiExecution? GeneratedByExecution { get; set; }
}
