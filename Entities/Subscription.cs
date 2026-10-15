using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

public class Subscription
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid PlanId { get; set; }

    public string? ProviderReference { get; set; }

    public SubscriptionStatus Status { get; set; }

    public DateTimeOffset PeriodStart { get; set; }

    public DateTimeOffset PeriodEnd { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public Plan Plan { get; set; } = null!;
}
