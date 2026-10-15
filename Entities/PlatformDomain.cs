namespace SmartMarketing.Domain.Entities;

// Maps a host name (instagram.com, wa.me, youtu.be ...) to a platform.
// When the owner pastes a competitor URL, its host is looked up here.
public class PlatformDomain
{
    public string Domain { get; set; } = string.Empty;

    public string PlatformCode { get; set; } = string.Empty;

    public Platform Platform { get; set; } = null!;
}
