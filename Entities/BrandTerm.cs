using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Words the AI should prefer or never use.
public class BrandTerm : BusinessEntity
{
    public string Term { get; set; } = string.Empty;

    public BrandTermRule Rule { get; set; }

    public string? Note { get; set; }

    public BrandProfile BrandProfile { get; set; } = null!;
}
