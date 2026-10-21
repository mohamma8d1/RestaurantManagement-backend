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
using CategoryEntity = RestaurantManagement.Domain.Entities.Category;

namespace RestaurantManagement.Application.Features.Category.Command.CreateCategory;

public class CreateCategoryCommandHandler(IUnitOfWork unitWork, IMapper mapper) : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {

        var category = mapper.Map<CategoryEntity>(request.dto);

        await unitWork.Category.AddAsync(category, cancellationToken);
        await unitWork.SaveChangesAsync(cancellationToken);


        return mapper.Map<CategoryDto>(category);

    }
}
