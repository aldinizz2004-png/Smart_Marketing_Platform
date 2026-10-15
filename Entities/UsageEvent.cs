using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Credit / cost ledger. Idempotent per (TenantId, IdempotencyKey).
public class UsageEvent
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid? BusinessId { get; set; }

    public Guid? AiExecutionId { get; set; }

    public UsageEventType EventType { get; set; }

    public decimal Units { get; set; }

    public decimal CreditDelta { get; set; }

    public decimal EstimatedCostUsd { get; set; }

    public string IdempotencyKey { get; set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public Business? Business { get; set; }

    public AiExecution? AiExecution { get; set; }
}
