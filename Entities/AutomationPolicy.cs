using SmartMarketing.Domain.Common;

namespace SmartMarketing.Domain.Entities;

// One-to-one with Business: the limits the OWNER sets for the platform.
public class AutomationPolicy : IHasUpdatedAt
{
    public Guid BusinessId { get; set; }

    public Guid TenantId { get; set; }

    // Owner kill switch: false = nothing is scheduled.
    public bool AutomationEnabled { get; set; } = true;

    public short MaxPostsPerDay { get; set; } = 3;

    // Business local time.
    public TimeOnly? PublishWindowStart { get; set; }

    public TimeOnly? PublishWindowEnd { get; set; }

    public decimal? MaxDiscountPercent { get; set; }

    public decimal? MonthlyAdBudget { get; set; }

    public string? BudgetCurrency { get; set; }

    // Four-eyes rule: the author of a revision cannot approve it.
    public bool RequireSeparateReviewer { get; set; }

    public Guid UpdatedBy { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public Business Business { get; set; } = null!;

    public TenantMembership UpdatedByMember { get; set; } = null!;

    // Many-to-many with Platform: where the platform may publish.
    public ICollection<AutomationPolicyPlatform> AllowedPlatforms { get; set; } = new List<AutomationPolicyPlatform>();

    // Recommendation types the AI may apply without asking.
    public ICollection<AutoApplyRule> AutoApplyRules { get; set; } = new List<AutoApplyRule>();
}
