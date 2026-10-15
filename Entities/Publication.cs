using SmartMarketing.Domain.Common;

namespace SmartMarketing.Domain.Entities;

// Result of a successful job (one-to-one with PublicationJob).
public class Publication : BusinessEntity
{
    public Guid PublicationJobId { get; set; }

    public Guid SocialAccountId { get; set; }

    public string? ExternalPostId { get; set; }

    public DateTimeOffset PublishedAt { get; set; }

    public string? ExternalUrl { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public PublicationJob PublicationJob { get; set; } = null!;

    public ICollection<MetricObservation> Metrics { get; set; } = new List<MetricObservation>();

    public ICollection<ConversionEvent> Conversions { get; set; } = new List<ConversionEvent>();
}
