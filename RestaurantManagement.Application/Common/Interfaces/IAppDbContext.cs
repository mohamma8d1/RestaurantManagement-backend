using RestaurantManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace RestaurantManagement.Application.Common.Interfaces;

public interface IAppDbContext

{
    IQueryable<User> Users { get; }
    IQueryable<Category> Categories { get; }
    IQueryable<FoodItem> FoodItems { get; }
    IQueryable<Order> Orders { get; }
    IQueryable<OrderItem> OrderItems { get; }
    IQueryable<Reservation> Reservations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
