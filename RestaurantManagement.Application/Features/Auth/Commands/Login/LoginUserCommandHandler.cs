using MediatR;
using RestaurantManagement.Application.Common.Interfaces;
using RestaurantManagement.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.Auth.Commands.Login;

public class LoginUserCommandHandler(IUnitOfWork unitWork, IJwtService jwtService) : IRequestHandler<LoginUserCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await unitWork.Users.GetByEmailAsync(request.LoginDto.Email, cancellationToken);
        if (user is null || !VerifyPassword(request.LoginDto.Password, user.PasswordHash))
            throw new Exception("Invalid Email or Password");

        var token = jwtService.GenerateToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Email = user.Email,
            FullName = user.FullName,
            Roles = new List<string> { user.Role },
        };

    }
    private bool VerifyPassword(string plainPassword, string hashedPassword)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(plainPassword);
        var hash = Convert.ToBase64String(sha256.ComputeHash(bytes));
        return hash == hashedPassword;
    }
}
