using Market.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Market.DataContext;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public override DbSet<ApplicationUser> Users { get; set; }
    
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                // Set "CreatedAt" to the current date on new records
                entity.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
            }

            // Set "UpdatedAt" to the current date whenever any fields change
            entity.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    
}
