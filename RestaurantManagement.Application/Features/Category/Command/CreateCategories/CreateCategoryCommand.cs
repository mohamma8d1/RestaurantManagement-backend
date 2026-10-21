using MediatR;
using RestaurantManagement.Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestaurantManagement.Application.Features.Category.Command.CreateCategory;

public record CreateCategoryCommand(CreateCategoryDto dto) : IRequest<CategoryDto>;