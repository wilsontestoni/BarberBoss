using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;

public interface IBillingRepositoriesReadOnly
{
    Task<Billing?> GetById(Guid id);
}
