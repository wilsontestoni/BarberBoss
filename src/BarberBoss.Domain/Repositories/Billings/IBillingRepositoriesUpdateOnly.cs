using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;

public interface IBillingRepositoriesUpdateOnly
{
    Task<Billing?> GetById(Guid id);
    void Update(Billing Billing);
}
