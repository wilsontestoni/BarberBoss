namespace BarberBoss.Infrastructure.DataAccess;

using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;

internal class BarberBossDbContext : DbContext
{
    public BarberBossDbContext(DbContextOptions options) : base(options) { }
    public DbSet<Billing> Billings { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var modifiedBillings = ChangeTracker.Entries<Billing>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in modifiedBillings)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

