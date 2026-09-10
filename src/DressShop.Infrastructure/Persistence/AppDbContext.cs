using DressShop.Application.Abstractions;
using DressShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DressShop.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Applies every IEntityTypeConfiguration<T> found in this assembly,
        // so entity configuration stays next to the DbContext instead of
        // being hand-registered one by one as the model grows.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    } 
}
