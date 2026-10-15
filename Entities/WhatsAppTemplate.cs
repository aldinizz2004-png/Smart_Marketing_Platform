using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Message template registered with WhatsApp Business and its approval state.
public class WhatsAppTemplate : BusinessEntity
{
    public Guid SocialAccountId { get; set; }

    // Always "whatsapp"; part of the FK so the account must be a WhatsApp account.
    public string PlatformCode { get; set; } = "whatsapp";

    public string TemplateName { get; set; } = string.Empty;

    public string LanguageCode { get; set; } = string.Empty;

    public TemplateCategory Category { get; set; }

    public TemplateStatus ProviderStatus { get; set; } = TemplateStatus.Pending;

    public string? BodyPreview { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public SocialAccount SocialAccount { get; set; } = null!;
}
