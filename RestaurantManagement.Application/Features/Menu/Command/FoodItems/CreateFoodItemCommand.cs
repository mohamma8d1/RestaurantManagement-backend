using MediatR;
using RestaurantManagement.Application.DTOs.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestaurantManagement.Application.Features.Menu.Command.FoodItems;

public record CreateFoodItemCommand(CreateFoodItemDto Dto) : IRequest<FoodItemDto>;