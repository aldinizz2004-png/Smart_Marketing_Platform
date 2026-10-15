using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// The owner's goal (e.g. "more orders this month") and its target.
public class Campaign : BusinessEntity, IHasUpdatedAt
{
    public Guid CreatedBy { get; set; }

    public string Name { get; set; } = string.Empty;

    public CampaignObjective Objective { get; set; }

    public string GoalStatement { get; set; } = string.Empty;

    public string? TargetMetric { get; set; }

    public decimal? TargetValue { get; set; }

    public decimal? Budget { get; set; }

    public string? BudgetCurrency { get; set; }

    public DateTimeOffset? StartsAt { get; set; }

    public DateTimeOffset? EndsAt { get; set; }

    public CampaignStatus Status { get; set; } = CampaignStatus.Draft;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public TenantMembership CreatedByMember { get; set; } = null!;

    public ICollection<CampaignPlan> Plans { get; set; } = new List<CampaignPlan>();

    public ICollection<ContentItem> ContentItems { get; set; } = new List<ContentItem>();

    // Many-to-many through explicit join entities
    public ICollection<CampaignProduct> CampaignProducts { get; set; } = new List<CampaignProduct>();

    public ICollection<CampaignAudience> CampaignAudiences { get; set; } = new List<CampaignAudience>();

    public ICollection<CampaignOffer> CampaignOffers { get; set; } = new List<CampaignOffer>();
}
