using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Orders, leads, messages, visits attributed to a campaign / publication.
public class ConversionEvent : BusinessEntity
{
    public Guid CampaignId { get; set; }

    public Guid? PublicationId { get; set; }

    public ConversionType EventType { get; set; }

    public ConversionSource Source { get; set; }

    public string? ExternalEventId { get; set; }

    public DateTimeOffset OccurredAt { get; set; }

    public decimal? RevenueAmount { get; set; }

    public string? RevenueCurrency { get; set; }

    public string? AttributionNote { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Campaign Campaign { get; set; } = null!;

    public Publication? Publication { get; set; }
}
