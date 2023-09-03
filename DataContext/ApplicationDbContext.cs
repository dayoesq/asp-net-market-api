using Market.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Market.DataContext;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductsImages { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Size> Sizes { get; set; }
    public override DbSet<ApplicationUser> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId);

        }
    
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow;

        var entities = ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entity in entities)
        {
            if (entity.Entity is BaseEntity baseEntity)
            {
                switch (entity.State)
                {
                    case EntityState.Added:
                        baseEntity.CreatedAt = currentTime;
                        break;
                    case EntityState.Detached: 
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Modified: 
                        baseEntity.UpdatedAt = currentTime;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                baseEntity.UpdatedAt = currentTime;
            }
            else if(entity.Entity is ApplicationUser)
            {
                switch (entity.State)
                {
                    case EntityState.Added:
                        entity.Property("CreatedAt").CurrentValue = currentTime;
                        break;
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Modified:
                        entity.Property("UpdatedAt").CurrentValue = currentTime;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                entity.Property("UpdatedAt").CurrentValue = currentTime;
            }
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    
}
