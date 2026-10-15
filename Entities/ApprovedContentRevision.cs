namespace SmartMarketing.Domain.Entities;

// The approval token: a publication job can ONLY point to a row here.
// Primary key = (BusinessId, ContentItemId, RevisionId); one-to-one with the revision.
public class ApprovedContentRevision
{
    public Guid BusinessId { get; set; }

    public Guid ContentItemId { get; set; }

    public Guid RevisionId { get; set; }

    public Guid TenantId { get; set; }

    public Guid ReviewId { get; set; }

    public DateTimeOffset ApprovedAt { get; set; }

    public ContentReview Review { get; set; } = null!;

    public ApprovalRevocation? Revocation { get; set; }

    public ICollection<PublicationJob> PublicationJobs { get; set; } = new List<PublicationJob>();
}
