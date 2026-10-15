using SmartMarketing.Domain.Common;
using SmartMarketing.Domain.Enums;

namespace SmartMarketing.Domain.Entities;

// Metadata of a file kept in object storage (the file itself is not in the DB).
public class MediaAsset : BusinessEntity
{
    public Guid? ProductId { get; set; }

    public MediaKind Kind { get; set; }

    public string ObjectKey { get; set; } = string.Empty;

    public string MimeType { get; set; } = string.Empty;

    public long ByteSize { get; set; }

    public string? Sha256 { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Product? Product { get; set; }
}
