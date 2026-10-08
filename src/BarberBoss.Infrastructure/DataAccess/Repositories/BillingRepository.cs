using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Repositories.Billings.Filters;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;

internal class BillingRepository : IBillingRepositoriesWriteOnly, IBillingRepositoriesReadOnly, IBillingRepositoriesUpdateOnly
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

    public async Task<bool> Delete(Guid id)
    {
        var result = await _dbContext.Billings.FirstOrDefaultAsync(billing => billing.Id == id);

        if(result == null)
        {
            return false;
        }

        _dbContext.Billings.Remove(result);

        return true;

    }

    public async Task<(List<Billing> Items, int TotalCount)> GetAll(BillingFilter filter)
    {
        var query = _dbContext.Billings.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.BarberName))
            query = query.Where(b => b.BarberName.Contains(filter.BarberName));

        if (!string.IsNullOrWhiteSpace(filter.ClientName))
            query = query.Where(b => b.ClientName.Contains(filter.ClientName));

        if (!string.IsNullOrWhiteSpace(filter.ServiceName))
            query = query.Where(b => b.ServiceName.Contains(filter.ServiceName));

        if (filter.Status.HasValue)
            query = query.Where(b => b.Status == filter.Status.Value);

        if (filter.MinAmount.HasValue)
            query = query.Where(b => b.Amount >= filter.MinAmount.Value);

        if (filter.MaxAmount.HasValue)
            query = query.Where(b => b.Amount <= filter.MaxAmount.Value);

        if (filter.StartDate.HasValue)
            query = query.Where(b => b.Date >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
        {
            var endExclusive = filter.EndDate.Value.Date.AddDays(1);
            query = query.Where(b => b.Date < endExclusive);
        }

        var totalCount = await query.CountAsync();

        query = filter.SortDir == SortDir.Asc
           ? query.OrderBy(b => b.Date)
           : query.OrderByDescending(b => b.Date);

        var items = await query
                   .Skip((filter.Page - 1) * filter.PageSize)
                   .Take(filter.PageSize)
                   .ToListAsync();

        return (items, totalCount);
    }

    async Task<Billing?> IBillingRepositoriesReadOnly.GetById(Guid id)
    {
        return await _dbContext.Billings.AsNoTracking().FirstOrDefaultAsync(billing => billing.Id == id);
    }

    async Task<Billing?> IBillingRepositoriesUpdateOnly.GetById(Guid id)
    {
        return await _dbContext.Billings.FirstOrDefaultAsync(billing => billing.Id == id);
    }

    public void Update(Billing Billing)
    {
        _dbContext.Billings.Update(Billing);
    }
}
