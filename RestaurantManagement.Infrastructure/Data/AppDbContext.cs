using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<FoodItem> FoodItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<Order>().HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey( o => o.UserId ).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>().HasOne(oi => oi.Order).WithMany(o => o.OrderItems).HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>().HasOne(oi => oi.FoodItem).WithMany(f => f.OrderItems).HasForeignKey(oi => oi.FoodItemId);

        modelBuilder.Entity<FoodItem>().HasOne(f => f.Category).WithMany(c => c.FoodItem).HasForeignKey(f => f.CategoryId);

        modelBuilder.Entity<FoodItem>().Property(f => f.Price).HasPrecision(18, 2); 

        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(18, 2);

    }
}
