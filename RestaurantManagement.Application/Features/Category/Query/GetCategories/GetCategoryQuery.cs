using MediatR;
using RestaurantManagement.Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.Category.Query.GetCategory;

public record GetCategoryQuery : IRequest<IReadOnlyList<CategoryDto>>;