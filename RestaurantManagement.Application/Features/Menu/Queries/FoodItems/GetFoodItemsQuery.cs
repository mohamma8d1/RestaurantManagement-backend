using MediatR;
using RestaurantManagement.Application.DTOs.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.Menu.Queries.FoodItems;

public record GetFoodItemsQuery : IRequest<IReadOnlyList<FoodItemDto>>;

