using SmartMarketing.Domain.Common;

namespace SmartMarketing.Domain.Entities;

// Why a recommendation was made. Exactly ONE of the three evidence links is
// set: one of our own metrics, a competitor post, or a competitor insight.
public class RecommendationEvidence : BusinessEntity
{
    public Guid RecommendationId { get; set; }

    public Guid? MetricObservationId { get; set; }

    public Guid? SourcePostId { get; set; }

    public Guid? CompetitorInsightId { get; set; }

    public string? Interpretation { get; set; }

    public StrategyRecommendation Recommendation { get; set; } = null!;

    public MetricObservation? MetricObservation { get; set; }

    public SourcePost? SourcePost { get; set; }

    public CompetitorInsight? CompetitorInsight { get; set; }
}
