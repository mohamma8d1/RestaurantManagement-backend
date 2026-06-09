// ============================================================
//  RestaurantManagement.Tests
//  xUnit + Moq + FluentAssertions
// ============================================================
//
//  packages:
//    dotnet add package Moq
//    dotnet add package FluentAssertions
//    dotnet add package Microsoft.Extensions.Configuration
//    dotnet add package System.IdentityModel.Tokens.Jwt
//    dotnet add package Microsoft.IdentityModel.Tokens
// ============================================================

using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using RestaurantManagement.Application.Common.Exeption;
using RestaurantManagement.Application.Common.Interfaces;
using RestaurantManagement.Application.DTOs.Auth;
using RestaurantManagement.Application.DTOs.Menu;
using RestaurantManagement.Application.Features.Auth.Commands.Login;
using RestaurantManagement.Application.Features.Auth.Commands.Register;
using RestaurantManagement.Application.Features.Menu.Command.FoodItems;
using RestaurantManagement.Application.Features.Menu.Queries.FoodItems;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Services;
using System.Security.Cryptography;
using System.Text;

namespace RestaurantManagement.Tests;


// ============================================================
//  HELPERS
// ============================================================

internal static class PasswordHelper
{
    public static string Hash(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(sha256.ComputeHash(bytes));
    }
}


// ============================================================
//  1. LoginUserCommandHandler Tests
// ============================================================

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IJwtService> _jwtService = new();
    private readonly Mock<IUserRepository> _userRepo = new();

    private LoginUserCommandHandler CreateHandler()
    {
        _unitOfWork.Setup(u => u.Users).Returns(_userRepo.Object);
        return new LoginUserCommandHandler(_unitOfWork.Object, _jwtService.Object);
    }

    // ----------------------------------------------------------
    //  Login_Success
    // ----------------------------------------------------------
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        const string plainPassword = "Test1234";
        var user = new User
        {
            id = Guid.NewGuid(),
            Email = "test@test.com",
            FullName = "Test User",
            Role = "User",
            PasswordHash = PasswordHelper.Hash(plainPassword)
        };

        _userRepo
            .Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _jwtService.Setup(j => j.GenerateToken(user)).Returns("mock-token");
        _jwtService.Setup(j => j.GenerateRefreshToken()).Returns("mock-refresh");

        var command = new LoginUserCommand(new LoginDto
        {
            Email = user.Email,
            Password = plainPassword
        });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("mock-token");
        result.RefreshToken.Should().Be("mock-refresh");
        result.Email.Should().Be(user.Email);
    }

    // ----------------------------------------------------------
    //  Login_InvalidPassword
    // ----------------------------------------------------------
    [Fact]
    public async Task Login_WithWrongPassword_ThrowsApiException()
    {
        // Arrange
        var user = new User
        {
            id = Guid.NewGuid(),
            Email = "test@test.com",
            FullName = "Test User",
            Role = "User",
            PasswordHash = PasswordHelper.Hash("CorrectPassword")
        };

        _userRepo
            .Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new LoginUserCommand(new LoginDto
        {
            Email = user.Email,
            Password = "WrongPassword"
        });

        var handler = CreateHandler();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApiException>()
            .WithMessage("*Invalid Email or Password*");
    }

    // ----------------------------------------------------------
    //  Login_UserNotFound
    // ----------------------------------------------------------
    [Fact]
    public async Task Login_WithUnknownEmail_ThrowsApiException()
    {
        // Arrange
        _userRepo
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new LoginUserCommand(new LoginDto
        {
            Email = "noone@test.com",
            Password = "SomePassword"
        });

        var handler = CreateHandler();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApiException>();
    }
}


// ============================================================
//  2. RegisterUserCommandHandler Tests
// ============================================================

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IJwtService> _jwtService = new();
    private readonly Mock<IUserRepository> _userRepo = new();

    private RegisterUserCommandHandler CreateHandler()
    {
        _unitOfWork.Setup(u => u.Users).Returns(_userRepo.Object);
        return new RegisterUserCommandHandler(_unitOfWork.Object, _mapper.Object, _jwtService.Object);
    }

    // ----------------------------------------------------------
    //  Register_Success
    // ----------------------------------------------------------
    [Fact]
    public async Task Register_WithUniqueEmail_CreatesUserAndReturnsToken()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Email = "new@test.com",
            Password = "Pass1234",
            FullName = "New User"
        };

        var user = new User
        {
            id = Guid.NewGuid(),
            Email = dto.Email,
            FullName = dto.FullName,
            Role = "User"
        };

        _userRepo
            .Setup(r => r.IsEmailUniqueAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapper.Setup(m => m.Map<User>(dto)).Returns(user);

        _userRepo
            .Setup(r => r.AddAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _jwtService.Setup(j => j.GenerateToken(user)).Returns("mock-token");
        _jwtService.Setup(j => j.GenerateRefreshToken()).Returns("mock-refresh");

        var command = new RegisterUserCommand(dto);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Token.Should().Be("mock-token");
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ----------------------------------------------------------
    //  Register_EmailAlreadyExists
    // ----------------------------------------------------------
    [Fact]
    public async Task Register_WithDuplicateEmail_ThrowsApiException()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Email = "existing@test.com",
            Password = "Pass1234",
            FullName = "Existing User"
        };

        _userRepo
            .Setup(r => r.IsEmailUniqueAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new RegisterUserCommand(dto);
        var handler = CreateHandler();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApiException>()
            .WithMessage("*Email already exist*");
    }
}


// ============================================================
//  3. CreateFoodItemCommandHandler Tests
// ============================================================

public class CreateFoodItemCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ICategoryRepository> _categoryRepo = new();
    private readonly Mock<IFoodItemRepository> _foodItemRepo = new();

    private CreateFoodItemCommandHandler CreateHandler()
    {
        _unitOfWork.Setup(u => u.Category).Returns(_categoryRepo.Object);
        _unitOfWork.Setup(u => u.FoodItem).Returns(_foodItemRepo.Object);
        return new CreateFoodItemCommandHandler(_unitOfWork.Object, _mapper.Object);
    }

    // ----------------------------------------------------------
    //  CreateFoodItem_Success
    // ----------------------------------------------------------
    [Fact]
    public async Task CreateFoodItem_WithValidCategory_SavesAndReturnsDtoCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var dto = new CreateFoodItemDto { CategoryId = categoryId, Name = "Pizza" };
        var foodItem = new FoodItem { id = Guid.NewGuid(), Name = "Pizza" };
        var resultDto = new FoodItemDto { Name = "Pizza" };

        _categoryRepo
            .Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category { id = categoryId });

        _mapper.Setup(m => m.Map<FoodItem>(dto)).Returns(foodItem);

        _foodItemRepo
            .Setup(r => r.AddAsync(foodItem, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _foodItemRepo
            .Setup(r => r.GetByIdAsync(foodItem.id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(foodItem);

        _mapper.Setup(m => m.Map<FoodItemDto>(foodItem)).Returns(resultDto);

        var command = new CreateFoodItemCommand(dto);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Pizza");
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ----------------------------------------------------------
    //  CreateFoodItem_CategoryNotFound
    // ----------------------------------------------------------
    [Fact]
    public async Task CreateFoodItem_WithInvalidCategory_ThrowsApiException()
    {
        // Arrange
        var dto = new CreateFoodItemDto { CategoryId = Guid.NewGuid(), Name = "Burger" };

        _categoryRepo
            .Setup(r => r.GetByIdAsync(dto.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);   // ← دسته‌بندی پیدا نشد

        var command = new CreateFoodItemCommand(dto);
        var handler = CreateHandler();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApiException>()
            .WithMessage("*Category not found*");
    }
}


// ============================================================
//  4. GetFoodItemsQueryHandler Tests
// ============================================================

public class GetFoodItemsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IFoodItemRepository> _foodItemRepo = new();

    private GetFoodItemsQueryHandler CreateHandler()
    {
        _unitOfWork.Setup(u => u.FoodItem).Returns(_foodItemRepo.Object);
        return new GetFoodItemsQueryHandler(_unitOfWork.Object, _mapper.Object);
    }

    // ----------------------------------------------------------
    //  GetFoodItems_ReturnList
    // ----------------------------------------------------------
    [Fact]
    public async Task GetFoodItems_WhenItemsExist_ReturnsMappedList()
    {
        // Arrange
        var foodItems = new List<FoodItem>
        {
            new() { id = Guid.NewGuid(), Name = "Pizza" },
            new() { id = Guid.NewGuid(), Name = "Burger" }
        };

        var dtos = new List<FoodItemDto>
        {
            new() { Name = "Pizza" },
            new() { Name = "Burger" }
        };

        _foodItemRepo
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(foodItems);

        _mapper
            .Setup(m => m.Map<IReadOnlyList<FoodItemDto>>(foodItems))
            .Returns(dtos.AsReadOnly());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetFoodItemsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Pizza");
    }

    // ----------------------------------------------------------
    //  GetFoodItems_EmptyList
    // ----------------------------------------------------------
    [Fact]
    public async Task GetFoodItems_WhenNoItems_ReturnsEmptyList()
    {
        // Arrange
        _foodItemRepo
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FoodItem>());

        _mapper
            .Setup(m => m.Map<IReadOnlyList<FoodItemDto>>(It.IsAny<IEnumerable<FoodItem>>()))
            .Returns(new List<FoodItemDto>().AsReadOnly());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(new GetFoodItemsQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}


// ============================================================
//  5. JwtService Tests
// ============================================================

public class JwtServiceTests
{
    private JwtService CreateService()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT:Secret"] = "SuperSecretKey1234567890ABCDEF!!",
                ["JWT:ValidIssuer"] = "TestIssuer",
                ["JWT:ValidAudience"] = "TestAudience",
                ["JWT:ExpireMinutes"] = "60"
            })
            .Build();

        return new JwtService(config);
    }

    // ----------------------------------------------------------
    //  GenerateToken_ReturnsNonEmptyToken
    // ----------------------------------------------------------
    [Fact]
    public void GenerateToken_ForValidUser_ReturnsNonEmptyJwt()
    {
        // Arrange
        var user = new User
        {
            id = Guid.NewGuid(),
            Email = "jwt@test.com",
            FullName = "JWT Tester",
            Role = "Admin"
        };

        var service = CreateService();

        // Act
        var token = service.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3);   // JWT = header.payload.signature
    }

    // ----------------------------------------------------------
    //  GenerateRefreshToken_ReturnsUniqueTokens
    // ----------------------------------------------------------
    [Fact]
    public void GenerateRefreshToken_CalledTwice_ReturnsDifferentValues()
    {
        // Arrange
        var service = CreateService();

        // Act
        var token1 = service.GenerateRefreshToken();
        var token2 = service.GenerateRefreshToken();

        // Assert
        token1.Should().NotBeNullOrEmpty();
        token1.Should().NotBe(token2);  
    }
}