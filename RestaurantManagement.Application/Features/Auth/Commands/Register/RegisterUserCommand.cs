using MediatR;
using RestaurantManagement.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.Auth.Commands.Register;

public record RegisterUserCommand(RegisterDto RegisterDto) : IRequest<AuthResponseDto>;
