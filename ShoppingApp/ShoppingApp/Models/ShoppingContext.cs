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
        }
    }
}
