using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Smooth.Shop.Domain.Models;

namespace Smooth.Shop.Infrastructure.Data;

public class SmoothWebDbContext : DbContext
{
    public SmoothWebDbContext(DbContextOptions<SmoothWebDbContext> options) : base(options)
    {
    }

    public DbSet<NewMedium> NewMedia { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmoothWebDbContext).Assembly);

        modelBuilder.Entity<NewMedium>().Property(m => m.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<NewMedium>().Property(m => m.CreatedAt).ValueGeneratedOnAdd();
    }


    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries<NewMedium>())
        {
            if (entry.State == EntityState.Modified)
            {
                throw new InvalidOperationException("Updates are not allowed for NewMedium.");
            }
        }
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<NewMedium>())
        {
            if (entry.State == EntityState.Modified)
            {
                throw new InvalidOperationException("Updates are not allowed for NewMedium.");
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}