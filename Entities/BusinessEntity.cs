using SmartMarketing.Domain.Entities;

namespace SmartMarketing.Domain.Common;

// Base class for every row that belongs to one business (shop / company).
// TenantId + BusinessId are copied on every row so that:
//   * every query can filter by tenant cheaply, and
//   * every foreign key can include BusinessId, which makes it impossible
//     to link a row to data that belongs to another business or tenant.
public abstract class BusinessEntity
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid BusinessId { get; set; }

    public Business Business { get; set; } = null!;
}
