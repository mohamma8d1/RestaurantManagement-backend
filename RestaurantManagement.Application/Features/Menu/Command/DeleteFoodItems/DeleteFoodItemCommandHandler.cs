using AutoMapper;
using MediatR;
using RestaurantManagement.Application.Common.Exeption;
using RestaurantManagement.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.Menu.Command.DeleteFoodItems;

public class DeleteFoodItemCommandHandler(IUnitOfWork unitWork) : IRequestHandler<DeleteFoodItemCommand, bool>
{
    public async Task<bool> Handle(DeleteFoodItemCommand request, CancellationToken cancellationToken)
    {
        var foodItem = await unitWork.FoodItem.GetByIdAsync(request.id, cancellationToken);
        if (foodItem == null)
            throw new ApiException("FoodItem not found", 400);

        foodItem.IsDeleted = true;
        foodItem.UpdateTime = DateTime.UtcNow;

        await unitWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
