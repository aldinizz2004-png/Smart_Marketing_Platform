using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// What the AI learned from tracked sources, e.g. "competitor X gets 3x more
// engagement on reels posted 20:00-22:00". TrackedSourceId = null means the
// insight comes from all tracked sources together.
public class CompetitorInsight : BusinessEntity
{
    public Guid? TrackedSourceId { get; set; }

    public Guid? StrategyRunId { get; set; }

    public InsightType InsightType { get; set; }

    public string Summary { get; set; } = string.Empty;

    public string DetailsJson { get; set; } = "{}";

    public decimal? Confidence { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public TrackedSource? TrackedSource { get; set; }

    public StrategyRun? StrategyRun { get; set; }
}
