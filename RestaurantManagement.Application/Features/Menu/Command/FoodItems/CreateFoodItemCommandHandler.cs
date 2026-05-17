using AutoMapper;
using MediatR;
using RestaurantManagement.Application.Common.Exeption;
using RestaurantManagement.Application.Common.Interfaces;
using RestaurantManagement.Application.DTOs.Menu;
using RestaurantManagement.Domain.Entities;
using System;

namespace RestaurantManagement.Application.Features.Menu.Command.FoodItems;

public class CreateFoodItemCommandHandler(IUnitOfWork unitWork, IMapper mapper) : IRequestHandler<CreateFoodItemCommand, FoodItemDto>
{
    public async Task<FoodItemDto> Handle(CreateFoodItemCommand request, CancellationToken cancellationToken)
    {
        var category = await unitWork.Category.GetByIdAsync(request.Dto.CategoryId,cancellationToken);
        if (category == null)
            throw new ApiException("Category not found", 400);

        var foodItem = mapper.Map<FoodItem>(request.Dto);
        await unitWork.FoodItem.AddAsync(foodItem, cancellationToken);
        await unitWork.SaveChangesAsync(cancellationToken);

        var created = await unitWork.FoodItem.GetByIdAsync(foodItem.id, cancellationToken);
        return mapper.Map<FoodItemDto>(created);
    }
}
