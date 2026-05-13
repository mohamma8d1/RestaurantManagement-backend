using System;
using RestaurantManagement.Application.Common.Interfaces;
using RestaurantManagement.Infrastructure.Data.Repositories;

namespace RestaurantManagement.Infrastructure.Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IUserRepository? userRepository;
    private ICategoryRepository? categoryRepository;
    private IFoodItemRepository? foodItemRepository;

    public IUserRepository Users => userRepository ??= new UserRepository(context);

    public ICategoryRepository Category => categoryRepository ??= new CategoryRepository(context);

    public IFoodItemRepository FoodItem => foodItemRepository ??= new FoodItemRepository(context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
