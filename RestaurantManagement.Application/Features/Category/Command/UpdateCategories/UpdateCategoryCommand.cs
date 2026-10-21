using MediatR;
using RestaurantManagement.Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.Category.Command.UpdateCategories;

public record UpdateCategoryCommand(Guid id, UpdateCategoryDto dto) : IRequest<CategoryDto>;
