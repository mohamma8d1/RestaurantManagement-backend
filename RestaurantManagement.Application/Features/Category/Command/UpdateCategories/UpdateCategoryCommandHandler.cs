using AutoMapper;
using MediatR;
using RestaurantManagement.Application.Common.Interfaces;
using RestaurantManagement.Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.Category.Command.UpdateCategories;

public class UpdateCategoryCommandHandler(IUnitOfWork unitWork, IMapper mapper) : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await unitWork.Category.GetByIdAsync(request.id, cancellationToken);

        if(category is null)
            throw new KeyNotFoundException($"Category with id {request.id} was not found.");

        mapper.Map(request.dto, category);
        category.UpdateTime = DateTime.UtcNow;

        unitWork.Category.Update(category);
        await unitWork.SaveChangesAsync(cancellationToken);

        return mapper.Map<CategoryDto>(category);

    }
}
