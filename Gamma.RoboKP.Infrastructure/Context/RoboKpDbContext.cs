using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Infrastructure.Configurations;
using Gamma.RoboKP.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gamma.RoboKP.Infrastructure.Context;

public class RoboKpDbContext : IdentityDbContext<AppUser, IdentityRoleEntity, long>
{
    public RoboKpDbContext(DbContextOptions<RoboKpDbContext> options) : base(options)
    {
    }
    
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<SubCategory> SubCategories { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartProduct> CartProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AppUserConfiguration());
        modelBuilder.ApplyConfiguration(new CartProductConfiguration());

        modelBuilder.Entity<AppUser>()
            .Property(u => u.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Product>()
            .HasIndex(u => u.Price)
            .HasDatabaseName("IX_Products_Price");
    }
}