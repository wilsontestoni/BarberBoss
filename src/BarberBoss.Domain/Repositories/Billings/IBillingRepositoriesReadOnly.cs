using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings.Filters;

namespace BarberBoss.Domain.Repositories.Billings;

public interface IBillingRepositoriesReadOnly
{
    Task<Billing?> GetById(Guid id);
    Task<(List<Billing> Items, int TotalCount)> GetAll(BillingFilter filter);
}
