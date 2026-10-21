using AutoMapper;
using MediatR;
using RestaurantManagement.Application.Common.Interfaces;
using RestaurantManagement.Application.DTOs.Category;
using RestaurantManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.Category.Query.GetCategory;

public class GetCategoryQueryHandler(IUnitOfWork unitWork, IMapper mapper) : IRequestHandler<GetCategoryQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var categories = await unitWork.Category.GetAllAsync(cancellationToken);
        return mapper.Map<IReadOnlyList<CategoryDto>>(categories);
    }

}
