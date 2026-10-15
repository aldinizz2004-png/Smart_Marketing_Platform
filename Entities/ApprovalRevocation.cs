namespace SmartMarketing.Domain.Entities;

// Withdraws an approval without rewriting history (one-to-one with the approval).
public class ApprovalRevocation
{
    public Guid BusinessId { get; set; }

    public Guid ContentItemId { get; set; }

    public Guid RevisionId { get; set; }

    public Guid TenantId { get; set; }

    public Guid RevokedBy { get; set; }

    public string? Reason { get; set; }

    public DateTimeOffset RevokedAt { get; set; }

    public ApprovedContentRevision Approval { get; set; } = null!;

    public TenantMembership RevokedByMember { get; set; } = null!;
}
