namespace SmartMarketing.Domain.Entities;

// SaaS subscription plan (Basic, Pro, ...) and its quotas.
public class Plan
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal MonthlyPrice { get; set; }

    public string Currency { get; set; } = "USD";

    // Quotas as JSON, e.g. {"postsPerMonth":120,"trackedSources":10}
    public string LimitsJson { get; set; } = "{}";

    public bool IsActive { get; set; } = true;

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
