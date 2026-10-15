namespace SmartMarketing.Domain.Entities;

// Join entity: AutomationPolicy <-> Platform (allowed platforms).
public class AutomationPolicyPlatform
{
    public Guid BusinessId { get; set; }

    public string PlatformCode { get; set; } = string.Empty;

    public Guid TenantId { get; set; }

    public AutomationPolicy Policy { get; set; } = null!;

    public Platform Platform { get; set; } = null!;
}
