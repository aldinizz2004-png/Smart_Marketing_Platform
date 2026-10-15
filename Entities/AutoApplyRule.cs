using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// One row per recommendation type the owner lets the AI apply on its own.
public class AutoApplyRule
{
    public Guid BusinessId { get; set; }

    public RecommendationAction ActionType { get; set; }

    public Guid TenantId { get; set; }

    public AutomationPolicy Policy { get; set; } = null!;
}
