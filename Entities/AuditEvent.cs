namespace SmartMarketing.Domain.Entities;

// Append-only log of who did what. ActorUserId = NULL means system / AI.
public class AuditEvent
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid? BusinessId { get; set; }

    public Guid? ActorUserId { get; set; }

    public string EventType { get; set; } = string.Empty;

    public string ObjectType { get; set; } = string.Empty;

    public Guid? ObjectId { get; set; }

    public string DetailsJson { get; set; } = "{}";

    public DateTimeOffset CreatedAt { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public Business? Business { get; set; }

    public AppUser? ActorUser { get; set; }
}
