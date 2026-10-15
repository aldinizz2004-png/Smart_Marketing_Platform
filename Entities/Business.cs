using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// A shop or small company that the platform markets.
public class Business : IHasUpdatedAt
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? CountryCode { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }

    // Windows time zone id (e.g. "West Bank Standard Time"); SQL Server's
    // AT TIME ZONE uses it to check publishing hours in local time.
    public string TimeZoneId { get; set; } = string.Empty;

    public string DefaultCurrency { get; set; } = "USD";

    public string? ContactPhone { get; set; }

    public string? Description { get; set; }

    public BusinessStatus Status { get; set; } = BusinessStatus.Active;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public Tenant Tenant { get; set; } = null!;

    // One-to-one
    public BrandProfile? BrandProfile { get; set; }

    public AutomationPolicy? AutomationPolicy { get; set; }

    // One-to-many
    public ICollection<BusinessHour> Hours { get; set; } = new List<BusinessHour>();

    public ICollection<Product> Products { get; set; } = new List<Product>();

    public ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public ICollection<AudienceSegment> AudienceSegments { get; set; } = new List<AudienceSegment>();

    public ICollection<SocialAccount> SocialAccounts { get; set; } = new List<SocialAccount>();

    public ICollection<TrackedSource> TrackedSources { get; set; } = new List<TrackedSource>();

    public ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
}
