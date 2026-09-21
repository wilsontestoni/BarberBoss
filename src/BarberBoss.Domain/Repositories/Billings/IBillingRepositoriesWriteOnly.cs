using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;

public interface IBillingRepositoriesWriteOnly
{
    Task Add(Billing billing);
}
