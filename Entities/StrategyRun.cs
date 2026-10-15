using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// One analysis over a results window (own results + competitor data).
public class StrategyRun : BusinessEntity
{
    public DateTimeOffset WindowStart { get; set; }

    public DateTimeOffset WindowEnd { get; set; }

    public string AlgorithmVersion { get; set; } = string.Empty;

    public RunStatus Status { get; set; }

    public string InputSummaryJson { get; set; } = "{}";

    public Guid? AiExecutionId { get; set; }

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public AiExecution? AiExecution { get; set; }

    public ICollection<StrategyRecommendation> Recommendations { get; set; } = new List<StrategyRecommendation>();
}
