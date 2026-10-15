using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// A customer reachable on WhatsApp. Messaging requires explicit opt-in.
public class CustomerContact : BusinessEntity, IHasUpdatedAt
{
    public Guid? AudienceSegmentId { get; set; }

    public string PhoneE164 { get; set; } = string.Empty;

    public string? DisplayName { get; set; }

    public bool WhatsAppOptIn { get; set; }

    public OptInSource? OptInSource { get; set; }

    public DateTimeOffset? OptedInAt { get; set; }

    public DateTimeOffset? OptedOutAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public AudienceSegment? AudienceSegment { get; set; }
}
