using RestaurantManagement.Domain.Entities;
using System;

namespace RestaurantManagement.Application.Common.Interfaces;

public interface IFoodItemRepository
{
    Task<FoodItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<FoodItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<FoodItem>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken);
    Task AddAsync(FoodItem foodItem, CancellationToken cancellationToken);
    void Update(FoodItem foodItem);
    void Delete(FoodItem foodItem);
}
