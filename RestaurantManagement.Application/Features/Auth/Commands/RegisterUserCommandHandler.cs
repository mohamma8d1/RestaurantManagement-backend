using AutoMapper;
using MediatR;
using RestaurantManagement.Application.Common.Interfaces;
using RestaurantManagement.Application.DTOs.Auth;
using RestaurantManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.Auth.Commands;

public class RegisterUserCommandHandler(IUnitOfWork unitOfWorkflow, IMapper mapper, IJwtService jwtService) : IRequestHandler<RegisterUserCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var isEmailUnniqe = await unitOfWorkflow.Users.IsEmailUniqueAsync(request.RegisterDto.Email, cancellationToken);

        if (!isEmailUnniqe)
            throw new Exception("Email already exist!!");

        var user = mapper.Map<User>(request.RegisterDto);
        user.PasswordHash = HashPassword(request.RegisterDto.Password);

        await unitOfWorkflow.Users.AddAsync(user, cancellationToken);
        await unitOfWorkflow.SaveChangesAsync(cancellationToken);

        var token = jwtService.GenerateToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Email = user.Email,
            FullName = user.FullName,
            Roles = new List<string> { user.Role }
        };

    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
