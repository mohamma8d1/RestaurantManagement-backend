using AutoMapper;
using MediatR;
using RestaurantManagement.Application.Common.Exeption;
using RestaurantManagement.Application.Common.Interfaces;
using RestaurantManagement.Application.DTOs.Menu;
using RestaurantManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.Menu.Command.UpdateFoodItems;

public class UpdateFoodItemCommandHandler(IUnitOfWork unitWork, IMapper mapper) : IRequestHandler<UpdateFoodItemCommand, FoodItemDto>
{
    public async Task<FoodItemDto> Handle(UpdateFoodItemCommand request, CancellationToken cancellationToken)
    {
        var foodItem = await unitWork.FoodItem.GetByIdAsync(request.dto.id, cancellationToken);
        if (foodItem == null)
            throw new ApiException("FoodItem not found", 400);

        var category = await unitWork.Category.GetByIdAsync(request.dto.CategoryId, cancellationToken);
        if (foodItem == null)
            throw new ApiException("Category not found", 400);

        mapper.Map(request.dto, foodItem);

        foodItem.UpdateTime = DateTime.UtcNow;
        
        await unitWork.SaveChangesAsync(cancellationToken);

        var updated = await unitWork.FoodItem.GetByIdAsync(request.dto.id, cancellationToken);

        return mapper.Map<FoodItemDto>(updated);
    }
}
