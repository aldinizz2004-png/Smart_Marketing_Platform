using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// A customer account. One tenant can own several businesses.
public class Tenant
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public TenantStatus Status { get; set; } = TenantStatus.Active;

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<TenantMembership> Memberships { get; set; } = new List<TenantMembership>();

    public ICollection<Business> Businesses { get; set; } = new List<Business>();

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
