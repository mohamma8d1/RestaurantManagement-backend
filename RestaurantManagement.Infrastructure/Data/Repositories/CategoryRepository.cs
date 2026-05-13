using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Common.Interfaces;
using RestaurantManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Infrastructure.Data.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task AddAsync(Category category, CancellationToken cancellationToken)
        => await context.Categories.AddAsync(category, cancellationToken);

    public void Delete(Category category) => context.Categories.Remove(category);

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken)
        => await context.Categories.ToListAsync(cancellationToken);

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken) 
        => await context.Categories.FirstOrDefaultAsync(c => c.id == id, cancellationToken);

    public void Update(Category category) => context.Categories.Update(category);
}
