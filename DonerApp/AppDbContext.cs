using Microsoft.EntityFrameworkCore;
using DonerApp.Models;

namespace DonerApp
{
    public class AppDbContext : DbContext
    {
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductIngredient> ProductIngredients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<RestaurantTable> RestaurantTables { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=doner.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable("order");

            modelBuilder.Entity<RestaurantTable>()
                .HasOne(t => t.ActiveOrder)
                .WithMany()
                .HasForeignKey(t => t.ActiveOrderId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}