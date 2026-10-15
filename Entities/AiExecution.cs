using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Every AI model call, kept for traceability and cost.
public class AiExecution : BusinessEntity
{
    public AiTaskType TaskType { get; set; }

    public string ModelName { get; set; } = string.Empty;

    public string PromptVersion { get; set; } = string.Empty;

    public ExecutionStatus Status { get; set; }

    public string InputSnapshotJson { get; set; } = "{}";

    public string? OutputSnapshotJson { get; set; }

    public int? InputTokens { get; set; }

    public int? OutputTokens { get; set; }

    public decimal? EstimatedCostUsd { get; set; }

    public string? ErrorCode { get; set; }

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
