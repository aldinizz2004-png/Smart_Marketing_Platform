namespace SmartMarketing.Domain.Entities;

// A login identity coming from the identity provider.
public class AppUser
{
    public Guid Id { get; set; }

    public string AuthSubject { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? FullName { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<TenantMembership> Memberships { get; set; } = new List<TenantMembership>();
}
