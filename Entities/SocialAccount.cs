using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// A connected account of the business on a platform (its own page/handle).
public class SocialAccount : BusinessEntity
{
    public string PlatformCode { get; set; } = string.Empty;

    public string ExternalAccountId { get; set; } = string.Empty;

    public string? Handle { get; set; }

    // Reference into a secrets manager - never the token itself.
    public string? CredentialRef { get; set; }

    public SocialAccountStatus Status { get; set; } = SocialAccountStatus.Disconnected;

    public DateTimeOffset CreatedAt { get; set; }

    public Platform Platform { get; set; } = null!;

    public ICollection<WhatsAppTemplate> WhatsAppTemplates { get; set; } = new List<WhatsAppTemplate>();
}
