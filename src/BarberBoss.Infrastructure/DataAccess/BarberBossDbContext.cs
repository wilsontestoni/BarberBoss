namespace BarberBoss.Infrastructure.DataAccess;

using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;

internal class BarberBossDbContext : DbContext
{
    public BarberBossDbContext(DbContextOptions options) : base(options) { }
    public DbSet<Billing> Billings { get; set; }
}

