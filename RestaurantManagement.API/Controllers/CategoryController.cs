using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.DTOs.Category;
using RestaurantManagement.Application.Features.Category.Command.CreateCategory;
using RestaurantManagement.Application.Features.Category.Command.UpdateCategories;
using RestaurantManagement.Application.Features.Category.Query.GetCategory;

namespace RestaurantManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(IMediator mediator) : ControllerBase
{

    [HttpGet("category")]
    public async Task<IActionResult> GetAllCategory()
    {
        var result = await mediator.Send(new GetCategoryQuery());

        return Ok(result);
    }

    [HttpPost("category")]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
    {
        var result  = await mediator.Send(new CreateCategoryCommand(dto));

        return Ok(result);
    }
    [HttpPut("category/{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryDto dto)
    {
        var result = await mediator.Send(new UpdateCategoryCommand(id, dto));
        return Ok(result);
    }
}
