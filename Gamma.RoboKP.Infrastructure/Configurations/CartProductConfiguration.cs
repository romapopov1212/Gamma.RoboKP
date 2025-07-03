using Gamma.RoboKP.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gamma.RoboKP.Infrastructure.Configurations;

public class CartProductConfiguration : IEntityTypeConfiguration<CartProduct>
{
    public void Configure(EntityTypeBuilder<CartProduct> builder)
    {
        builder.HasKey(c => new { c.CartId, c.ProductId });
        builder.HasOne(c => c.Cart).WithMany(c => c.CartProducts)
            .HasForeignKey(c => c.CartId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(c => c.Product).WithMany(c => c.CartProducts)
            .HasForeignKey(c => c.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}