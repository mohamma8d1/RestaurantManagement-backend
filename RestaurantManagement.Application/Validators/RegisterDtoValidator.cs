using FluentValidation;
using RestaurantManagement.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().WithMessage("Full name is Required").MinimumLength(6).WithMessage("Full name must be at least 6 character");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is Required").EmailAddress().WithMessage("Email address format invalid");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is Required").Matches(@"^09[0-9]{9}$").WithMessage("Invalid phone number format");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is Required").MinimumLength(8).WithMessage("Password must be at least 8 character");
        RuleFor(x => x.Role).Must(role => new[] { "Customer", "Admin", "Chef", "Waiter"  }.Contains(role)).WithMessage("Role must be Customer, Admin, Chef, or Waiter");
    }
}
