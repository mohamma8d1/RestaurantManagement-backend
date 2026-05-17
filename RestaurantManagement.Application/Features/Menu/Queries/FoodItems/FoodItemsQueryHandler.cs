using AutoMapper;
using MediatR;
using RestaurantManagement.Application.Common.Interfaces;
using RestaurantManagement.Application.DTOs.Menu;
using System;

namespace RestaurantManagement.Application.Features.Menu.Queries.FoodItems;

public class FoodItemsQueryHandler(IUnitOfWork unitWork, IMapper mapper) : IRequestHandler<FoodItemsQuery, IReadOnlyList<FoodItemDto>>
{
    public async Task<IReadOnlyList<FoodItemDto>> Handle(FoodItemsQuery request, CancellationToken cancellationToken)
    {
        var foodItems = await unitWork.FoodItem.GetAllAsync(cancellationToken);
        return mapper.Map<IReadOnlyList<FoodItemDto>>(foodItems);
    }
}
