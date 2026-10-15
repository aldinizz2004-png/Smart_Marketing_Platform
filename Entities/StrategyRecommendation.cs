using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// A change the AI proposes (new content, other format, other time ...).
public class StrategyRecommendation : BusinessEntity
{
    public Guid StrategyRunId { get; set; }

    public Guid? CampaignId { get; set; }

    public RecommendationAction ActionType { get; set; }

    public string ProposedPayloadJson { get; set; } = "{}";

    public DateTimeOffset? ProposedPublishAt { get; set; }

    public string Rationale { get; set; } = string.Empty;

    public decimal? Confidence { get; set; }

    public DecisionStatus DecisionStatus { get; set; } = DecisionStatus.Pending;

    // true = applied by the AI because the owner allowed this action type.
    public bool AppliedAutomatically { get; set; }

    public Guid? DecidedBy { get; set; }

    public DateTimeOffset? DecidedAt { get; set; }

    public DateTimeOffset? AppliedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public StrategyRun StrategyRun { get; set; } = null!;

    public Campaign? Campaign { get; set; }

    public TenantMembership? DecidedByMember { get; set; }

    public ICollection<RecommendationEvidence> Evidence { get; set; } = new List<RecommendationEvidence>();
}
