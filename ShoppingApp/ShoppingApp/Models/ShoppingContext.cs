using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models.Entities;
using System.Reflection.Emit;

namespace ShoppingApp.Models
{
    public class ShoppingContext: DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        public ShoppingContext(DbContextOptions<ShoppingContext> options) : base(options)
        {
           
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Product>().HasOne(b => b.Category)
                                      .WithMany(c => c.Products)
                                      .HasForeignKey(bc => bc.CategoryId);
            modelBuilder.Entity<Product>().HasOne(b => b.Owner)
                                       .WithMany(a => a.Products)
                                       .HasForeignKey(b => b.OwnerId);
            // many to many relation between Order and Product
            modelBuilder.Entity<OrderDetail>().HasKey("ProductId", "OrderId");

            modelBuilder.Entity<Order>().HasMany(o => o.Products)
                                        .WithOne(od => od.Order)
                                        .HasForeignKey(od => od.OrderId)
                                        .OnDelete(DeleteBehavior.NoAction); ;
            modelBuilder.Entity<Product>().HasMany(o => o.Orders)
                                        .WithOne(od => od.Product)
                                        .HasForeignKey(od => od.ProductId)
                                        .OnDelete(DeleteBehavior.NoAction);
            // 1 to many relation between Customer and Order
            modelBuilder.Entity<Order>().HasOne(o => o.Customer)
                                        .WithMany(c => c.Orders)
                                        .HasForeignKey(o => o.CustomerId);
        }
    }
}
