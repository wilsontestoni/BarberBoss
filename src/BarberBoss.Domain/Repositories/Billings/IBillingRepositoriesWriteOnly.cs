using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings.Filters;

namespace BarberBoss.Domain.Repositories.Billings;

public interface IBillingRepositoriesWriteOnly
{
    Task Add(Billing billing);
    Task<bool> Delete(Guid id);
}
