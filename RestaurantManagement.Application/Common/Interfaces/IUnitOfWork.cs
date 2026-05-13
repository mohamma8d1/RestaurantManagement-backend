using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    ICategoryRepository Category { get; }
    IFoodItemRepository FoodItem { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
