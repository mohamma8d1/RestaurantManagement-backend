using System;
using RestaurantManagement.Application.Common.Interfaces;
using RestaurantManagement.Infrastructure.Data.Repositories;

namespace RestaurantManagement.Infrastructure.Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWorkflow
{
    private IUserRepository? userRepository;
    public IUserRepository Users => userRepository ??= new UserRepository(context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
    // just for save 
}
