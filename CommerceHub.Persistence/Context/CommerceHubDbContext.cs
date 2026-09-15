using CommerceHub.Persistence.Models;
using CommerceHub.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommerceHub.Persistence.Context;

public class CommerceHubDbContext(DbContextOptions<CommerceHubDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(CommerceHubDbContext).Assembly);
    }
}