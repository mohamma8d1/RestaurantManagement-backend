using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.DTOs.Auth;
using RestaurantManagement.Application.Features.Auth.Commands.Login;
using RestaurantManagement.Application.Features.Auth.Commands.Register;

namespace RestaurantManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await mediator.Send(new RegisterUserCommand(registerDto));
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await mediator.Send(new LoginUserCommand(loginDto));
            return Ok(result);
        }

    }
}
