using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Many-to-many between Tenant and AppUser, with a role on the link.
public class TenantMembership
{
    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    public MembershipRole Role { get; set; }

    public MembershipStatus Status { get; set; } = MembershipStatus.Active;

    public DateTimeOffset JoinedAt { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public AppUser User { get; set; } = null!;
}
