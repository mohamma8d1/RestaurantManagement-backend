using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Common.Interfaces;
using RestaurantManagement.Domain.Entities;
using System;

namespace RestaurantManagement.Infrastructure.Data.Repositories;

public class FoodItemRepository(AppDbContext context) : IFoodItemRepository
{
    public async Task AddAsync(FoodItem foodItem, CancellationToken cancellationToken)
        => await context.FoodItems.AddAsync(foodItem, cancellationToken);

    public void Delete(FoodItem foodItem) => context.FoodItems.Remove(foodItem);

    public async Task<IReadOnlyList<FoodItem>> GetAllAsync(CancellationToken cancellationToken)
        => await context.FoodItems.Include(f => f.Category).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<FoodItem>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
        => await context.FoodItems.Where(c => c.CategoryId == categoryId).Include(f => f.Category).ToListAsync(cancellationToken);

    public async Task<FoodItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.FoodItems.Include(f => f.Category).FirstOrDefaultAsync(f => f.id == id, cancellationToken);

    public void Update(FoodItem foodItem) => context.FoodItems.Update(foodItem);
}
