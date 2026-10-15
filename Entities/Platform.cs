namespace SmartMarketing.Domain.Entities;

// Social platforms are DATA, not code: adding TikTok, Threads or a new
// network is a new row here, not a code change or a new CHECK constraint.
public class Platform
{
    // Short stable code used as the key, e.g. "instagram".
    public string Code { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public bool SupportsPublishing { get; set; }

    public bool SupportsMessaging { get; set; }

    public bool SupportsCompetitorTracking { get; set; }

    public bool IsActive { get; set; } = true;

    // Host names used to detect the platform from any pasted URL.
    public ICollection<PlatformDomain> Domains { get; set; } = new List<PlatformDomain>();
}
