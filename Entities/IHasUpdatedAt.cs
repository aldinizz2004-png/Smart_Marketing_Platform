namespace SmartMarketing.Domain.Common;

// Entities with this interface get UpdatedAt refreshed automatically in SaveChanges.
public interface IHasUpdatedAt
{
    DateTimeOffset UpdatedAt { get; set; }
}
