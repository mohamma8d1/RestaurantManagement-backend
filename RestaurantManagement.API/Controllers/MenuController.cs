using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.DTOs.Menu;
using RestaurantManagement.Application.Features.Menu.Command.DeleteFoodItems;
using RestaurantManagement.Application.Features.Menu.Command.FoodItems;
using RestaurantManagement.Application.Features.Menu.Command.UpdateFoodItems;
using RestaurantManagement.Application.Features.Menu.Queries.FoodItems;

namespace RestaurantManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MenuController(IMediator mediator) : ControllerBase
{

    [HttpGet("fooditems")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllFoodItems()
    {
        var result = await mediator.Send(new GetFoodItemsQuery());
        return Ok(result);
    }

    [HttpPost("fooditems")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateFoodItem(CreateFoodItemDto dto)
    {
        var result = await mediator.Send(new CreateFoodItemCommand(dto));
        return Ok(result);
    }

    [HttpPut("fooditems")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFoodItem(UpdateFoodItemDto dto)
    {
        var result = await mediator.Send(new UpdateFoodItemCommand(dto));
        return Ok(result);
    }

    [HttpDelete("fooditems")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFoodItem(Guid id)
    {
        var result = await mediator.Send(new DeleteFoodItemCommand(id));
        if (result)
            return NoContent();
        return BadRequest();
    }

}
