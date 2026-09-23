using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;

internal class BillingRepository : IBillingRepositoriesWriteOnly, IBillingRepositoriesReadOnly
{
    private readonly BarberBossDbContext _dbContext;

    public BillingRepository(BarberBossDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Billing billing)
    {
        await _dbContext.Billings.AddAsync(billing);
    }

    public async Task<Billing?> GetById(Guid id)
    {
        // await _dbContext.Billings.FindAsync(billing => billing.Id == id);

        return await _dbContext.Billings.AsNoTracking().FirstOrDefaultAsync(billing => billing.Id == id);
    }
}
