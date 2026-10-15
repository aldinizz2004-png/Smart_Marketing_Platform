using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Cumulative metric snapshot of one of OUR publications.
public class MetricObservation : BusinessEntity
{
    public Guid PublicationId { get; set; }

    public MetricCode MetricCode { get; set; }

    public decimal MetricValue { get; set; }

    public DateTimeOffset ObservedAt { get; set; }

    public MetricSource Source { get; set; }

    public string? RawPayloadJson { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Publication Publication { get; set; } = null!;
}
