using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// One WhatsApp recipient of a WhatsApp publication job.
public class MessageDelivery : BusinessEntity, IHasUpdatedAt
{
    public Guid PublicationJobId { get; set; }

    public Guid ContactId { get; set; }

    public DeliveryStatus Status { get; set; } = DeliveryStatus.Queued;

    public string? ExternalMessageId { get; set; }

    public string? LastError { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public PublicationJob PublicationJob { get; set; } = null!;

    public CustomerContact Contact { get; set; } = null!;
}
