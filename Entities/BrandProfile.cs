using SmartMarketing.Domain.Common;

namespace SmartMarketing.Domain.Entities;

// One-to-one with Business: the shop's tone of voice.
// BusinessId is both the primary key and the foreign key.
public class BrandProfile : IHasUpdatedAt
{
    public Guid BusinessId { get; set; }

    public Guid TenantId { get; set; }

    public string Dialect { get; set; } = string.Empty;

    public string Tone { get; set; } = string.Empty;

    // 0 = no emoji ... 3 = lots of emoji
    public byte EmojiLevel { get; set; } = 1;

    public string? Guidelines { get; set; }

    public string ColorPaletteJson { get; set; } = "[]";

    public DateTimeOffset UpdatedAt { get; set; }

    public Business Business { get; set; } = null!;

    public ICollection<BrandTerm> Terms { get; set; } = new List<BrandTerm>();
}
